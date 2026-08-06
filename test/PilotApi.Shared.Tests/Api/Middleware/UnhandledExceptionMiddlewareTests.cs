using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PilotApi.Shared.Api.Middleware;
using PilotApi.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

			// Act
			var exception = Assert.ThrowsAsync<UserException>(async () => await middleware.InvokeAsync(context));

			// Assert
			Assert.NotNull(exception);
			Assert.That(loggerProvider.Entries, Has.Count.EqualTo(1));

			var logEntry = loggerProvider.Entries[0];
			Assert.That(logEntry.Exception, Is.SameAs(originalException));
			Assert.That(logEntry.Message, Does.StartWith("An error occurred."));

			var match = Regex.Match(exception!.Message, @"correlation ID: ([0-9a-fA-F-]+)");
			Assert.That(match.Success, Is.True);
			Assert.That(logEntry.Message, Does.Contain(match.Groups[1].Value));
		}

		[Test]
		public async Task UnhandledExceptionMiddleware_InvokeAsync_WhenNextThrows_ShouldThrowUserException_Test()
		{
			// Arrange
			var originalException = new InvalidOperationException("Unhandled failure");
			RequestDelegate next = context => throw originalException;
			var loggerProvider = new CapturingLoggerProvider();
			using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
			var logger = loggerFactory.CreateLogger<UnhandledExceptionMiddleware>();
			var middleware = new UnhandledExceptionMiddleware(next, logger);
			var context = new DefaultHttpContext();

			// Act
			var exception = Assert.ThrowsAsync<UserException>(async () => await middleware.InvokeAsync(context));

			// Assert
			Assert.NotNull(exception);
			Assert.That(exception?.Message, Does.StartWith("An error occurred. The details can be found in the log with the following correlation ID: "));
			Assert.That(exception?.InnerException, Is.SameAs(originalException));
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
	}
}
