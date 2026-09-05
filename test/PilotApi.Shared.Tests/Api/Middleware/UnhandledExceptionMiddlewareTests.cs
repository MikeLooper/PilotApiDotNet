using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PilotApi.Shared.Api.Middleware;
using PilotApi.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PilotApi.Web.Tests.Middleware
{
	[TestFixture]
	public class UnhandledExceptionMiddlewareTests
	{
		[Test]
		public async Task UnhandledExceptionMiddleware_InvokeAsync_WhenNextThrows_ShouldLogErrorWithCorrelationMessage_Test()
		{
			// Arrange
			var originalException = new InvalidOperationException("Unhandled failure");
			RequestDelegate next = context => throw originalException;
			var loggerProvider = new CapturingLoggerProvider();
			using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
			var logger = loggerFactory.CreateLogger<UnhandledExceptionMiddleware>();
			var middleware = new UnhandledExceptionMiddleware(next, logger);
			var context = new DefaultHttpContext();
			context.Response.Body = new MemoryStream();

			// Act
			await middleware.InvokeAsync(context);

			// Assert
			Assert.That(loggerProvider.Entries, Has.Count.EqualTo(1));

			var logEntry = loggerProvider.Entries[0];
			Assert.That(logEntry.Exception, Is.SameAs(originalException));
			Assert.That(logEntry.Message, Does.StartWith("An error occurred."));
		}

		[Test]
		public async Task UnhandledExceptionMiddleware_InvokeAsync_WhenNextThrows_ShouldWriteInternalServerErrorProblemDetails_Test()
		{
			// Arrange
			var originalException = new InvalidOperationException("Unhandled failure");
			RequestDelegate next = context => throw originalException;
			var loggerProvider = new CapturingLoggerProvider();
			using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
			var logger = loggerFactory.CreateLogger<UnhandledExceptionMiddleware>();
			var middleware = new UnhandledExceptionMiddleware(next, logger);
			var context = new DefaultHttpContext();
			context.Request.Path = "/test/path";
			context.Response.Body = new MemoryStream();

			// Act
			await middleware.InvokeAsync(context);

			// Assert
			Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
			Assert.That(context.Response.ContentType, Does.StartWith("application/json"));

			context.Response.Body.Position = 0;
			using var reader = new StreamReader(context.Response.Body);
			var responseBody = await reader.ReadToEndAsync();
			using var document = JsonDocument.Parse(responseBody);
			var root = document.RootElement;
			Assert.That(root.GetProperty("title").GetString(), Is.EqualTo("Internal Server Error"));
			Assert.That(root.GetProperty("status").GetInt32(), Is.EqualTo(StatusCodes.Status500InternalServerError));
			Assert.That(root.GetProperty("detail").GetString(), Does.StartWith("An error occurred. The error details can be found in the log with the following correlation ID: "));
			Assert.That(root.GetProperty("instance").GetString(), Is.EqualTo("/test/path"));
		}

		[Test]
		public async Task UnhandledExceptionMiddleware_InvokeAsync_WhenUserExceptionAndResponseStarted_ShouldRethrowUserException_Test()
		{
			// Arrange
			var originalException = new UserException("User-facing failure");
			RequestDelegate next = context => throw originalException;
			var logger = LoggerFactory.Create(builder => builder.AddProvider(new CapturingLoggerProvider()))
				.CreateLogger<UnhandledExceptionMiddleware>();
			var middleware = new UnhandledExceptionMiddleware(next, logger);
			var context = new DefaultHttpContext();
			context.Features.Set<IHttpResponseFeature>(new StartedHttpResponseFeature());

			// Act
			var exception = Assert.ThrowsAsync<UserException>(async () => await middleware.InvokeAsync(context));

			// Assert
			Assert.That(exception, Is.SameAs(originalException));
		}

		private sealed class CapturingLogger : ILogger
		{
			private readonly List<LogEntry> entries;

			public CapturingLogger(List<LogEntry> entries)
			{
				this.entries = entries;
			}

			public IDisposable BeginScope<TState>(TState state)
			{
				return NullScope.Instance;
			}

			public bool IsEnabled(LogLevel logLevel)
			{
				return true;
			}

			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
			{
				this.entries.Add(new LogEntry(logLevel, eventId, formatter(state, exception), exception));
			}
		}

		private sealed class CapturingLoggerProvider : ILoggerProvider
		{
			public List<LogEntry> Entries { get; } = new();

			public ILogger CreateLogger(string categoryName)
			{
				return new CapturingLogger(this.Entries);
			}

			public void Dispose()
			{
			}
		}

		private sealed class NullScope : IDisposable
		{
			public static NullScope Instance { get; } = new NullScope();

			public void Dispose()
			{
			}
		}

		private sealed record LogEntry(LogLevel Level, EventId EventId, string Message, Exception? Exception);

		private sealed class StartedHttpResponseFeature : IHttpResponseFeature
		{
			public Stream Body { get; set; } = new MemoryStream();
			public bool HasStarted { get; set; } = true;
			public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();
			public string ReasonPhrase { get; set; } = string.Empty;
			public int StatusCode { get; set; } = StatusCodes.Status200OK;

			public void OnCompleted(Func<object, Task> callback, object state)
			{
			}

			public void OnStarting(Func<object, Task> callback, object state)
			{
			}
		}
	}
}
