using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using NUnit.Framework;
using PilotApi.Shared.Api.Middleware;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Models;
using PilotApi.Shared.Tests.Testing.Doubles;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PilotApi.Shared.Tests.Api.Middleware
{
	[TestFixture]
	public class RequestSettingsMiddlewareTests : TestBase
	{
		[Test]
		public async Task RequestSettingsMiddleware_InvokeAsync_WithoutCorrelationHeader_ShouldWriteGeneratedIdentifiersToResponseHeaders_Test()
		{
			// Arrange
			var (context, onStartingCallbacks) = CreateHttpContextWithResponseCallbacks();
			var requestSettings = new RequestSettings();
			var middleware = new RequestSettingsMiddleware(_ => Task.CompletedTask);

			// Act
			await middleware.InvokeAsync(context, requestSettings);
			await RunOnStartingCallbacksAsync(onStartingCallbacks);

			// Assert
			Assert.That(Guid.TryParse(context.Response.Headers[HeaderConstants.CorrelationIdHeaderName].ToString(), out _), Is.True);
			Assert.That(Guid.TryParse(context.Response.Headers[HeaderConstants.RequestIdHeaderName].ToString(), out _), Is.True);
			Assert.That(context.Response.Headers[HeaderConstants.CorrelationIdHeaderName].ToString(), Is.EqualTo(requestSettings.CorrelationId));
			Assert.That(context.Response.Headers[HeaderConstants.RequestIdHeaderName].ToString(), Is.EqualTo(requestSettings.RequestId));
		}

		[Test]
		public async Task RequestSettingsMiddleware_InvokeAsync_WithCorrelationHeader_ShouldSetCorrelationIdBeforeNextDelegate_Test()
		{
			// Arrange
			var context = new DefaultHttpContext();
			var correlationId = "caller-correlation-id";
			context.Request.Headers[HeaderConstants.CorrelationIdHeaderName] = correlationId;
			var requestSettings = new RequestSettings();
			string? correlationIdSeenByNext = null;
			var middleware = new RequestSettingsMiddleware(_ =>
			{
				correlationIdSeenByNext = requestSettings.CorrelationId;
				return Task.CompletedTask;
			});

			// Act
			await middleware.InvokeAsync(context, requestSettings);

			// Assert
			Assert.That(requestSettings.CorrelationId, Is.EqualTo(correlationId));
			Assert.That(correlationIdSeenByNext, Is.EqualTo(correlationId));
		}

		[Test]
		public async Task RequestSettingsMiddleware_InvokeAsync_OnResponseStarting_ShouldWriteBothResponseHeaders_Test()
		{
			// Arrange
			var (context, onStartingCallbacks) = CreateHttpContextWithResponseCallbacks();
			var requestSettings = new RequestSettings
			{
				CorrelationId = "correlation-id",
				RequestId = "request-id"
			};
			var middleware = new RequestSettingsMiddleware(_ => Task.CompletedTask);

			// Act
			await middleware.InvokeAsync(context, requestSettings);
			await RunOnStartingCallbacksAsync(onStartingCallbacks);

			// Assert
			Assert.That(context.Response.Headers[HeaderConstants.CorrelationIdHeaderName].ToString(), Is.EqualTo("correlation-id"));
			Assert.That(context.Response.Headers[HeaderConstants.RequestIdHeaderName].ToString(), Is.EqualTo("request-id"));
		}

		[Test]
		public async Task RequestSettingsMiddleware_InvokeAsync_ShouldPassContextToNextDelegate_Test()
		{
			// Arrange
			var context = new DefaultHttpContext();
			HttpContext? contextSeenByNext = null;
			var middleware = new RequestSettingsMiddleware(nextContext =>
			{
				contextSeenByNext = nextContext;
				return Task.CompletedTask;
			});

			// Act
			await middleware.InvokeAsync(context, new RequestSettings());

			// Assert
			Assert.That(contextSeenByNext, Is.SameAs(context));
		}

		[Test]
		public async Task RequestSettingsMiddleware_InvokeAsync_WithCallerNameHeader_ShouldSetCallerName_Test()
		{
			// Arrange
			var context = new DefaultHttpContext();
			context.Request.Headers[HeaderConstants.CallerNameHeaderName] = "PilotAngularUi";
			var requestSettings = new RequestSettings();
			var middleware = new RequestSettingsMiddleware(_ => Task.CompletedTask);

			// Act
			await middleware.InvokeAsync(context, requestSettings);

			// Assert
			Assert.That(requestSettings.CallerName, Is.EqualTo("PilotAngularUi"));
		}

		[Test]
		public async Task RequestSettingsMiddleware_InvokeAsync_WithoutCallerNameHeader_ShouldLeaveCallerNameNull_Test()
		{
			// Arrange
			var context = new DefaultHttpContext();
			var requestSettings = new RequestSettings();
			var middleware = new RequestSettingsMiddleware(_ => Task.CompletedTask);

			// Act
			await middleware.InvokeAsync(context, requestSettings);

			// Assert
			Assert.That(requestSettings.CallerName, Is.Null);
		}

		[Test]
		public async Task RequestSettingsMiddleware_InvokeAsync_ShouldPushRequestPropertiesOntoLogContextForNextDelegate_Test()
		{
			// Arrange
			var sink = new CapturingLogEventSink();
			using var logger = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.WriteTo.Sink(sink)
				.CreateLogger();
			var context = new DefaultHttpContext();
			context.Request.Headers[HeaderConstants.CallerNameHeaderName] = "PilotAngularUi";
			var requestSettings = new RequestSettings
			{
				CorrelationId = "correlation-id",
				RequestId = "request-id"
			};
			var middleware = new RequestSettingsMiddleware(_ =>
			{
				logger.Information("Inside the request");
				return Task.CompletedTask;
			});

			// Act
			await middleware.InvokeAsync(context, requestSettings);
			logger.Information("After the request");

			// Assert
			var insideEvent = sink.Events[0];
			Assert.That(insideEvent.Properties[LogContextPropertyConstants.CorrelationId].ToString(), Is.EqualTo("\"correlation-id\""));
			Assert.That(insideEvent.Properties[LogContextPropertyConstants.RequestId].ToString(), Is.EqualTo("\"request-id\""));
			Assert.That(insideEvent.Properties[LogContextPropertyConstants.CallerName].ToString(), Is.EqualTo("\"PilotAngularUi\""));

			var afterEvent = sink.Events[1];
			Assert.That(afterEvent.Properties.ContainsKey(LogContextPropertyConstants.CorrelationId), Is.False);
		}

		private static (HttpContext Context, List<(Func<object, Task> Callback, object State)> Callbacks) CreateHttpContextWithResponseCallbacks()
		{
			var context = new DefaultHttpContext();
			var callbacks = new List<(Func<object, Task> Callback, object State)>();
			var responseFeature = new Mock<IHttpResponseFeature>();
			responseFeature.SetupGet(feature => feature.Headers).Returns(new HeaderDictionary());
			responseFeature
				.Setup(feature => feature.OnStarting(It.IsAny<Func<object, Task>>(), It.IsAny<object>()))
				.Callback<Func<object, Task>, object>((callback, state) => callbacks.Add((callback, state)));
			context.Features.Set(responseFeature.Object);

			return (context, callbacks);
		}

		private static async Task RunOnStartingCallbacksAsync(List<(Func<object, Task> Callback, object State)> callbacks)
		{
			foreach (var (callback, state) in callbacks)
			{
				await callback(state);
			}
		}
	}
}
