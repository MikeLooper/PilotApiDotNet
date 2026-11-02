using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PilotApi.Shared.Api.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PilotApi.Shared.Tests.Api.Filters
{
	[TestFixture]
	public class EndpointLoggingFilterTests : TestBase
	{
		[Test]
		public void EndpointLoggingFilter_Constructor_WithNullLogger_Throws_Test()
		{
			Assert.Throws<ArgumentNullException>(() => new EndpointLoggingFilter(null!));
		}

		[Test]
		public async Task EndpointLoggingFilter_OnResourceExecutionAsync_LogsStartBeforeNextAndEndAfter_Test()
		{
			// Arrange
			var logger = new Mock<ILogger<EndpointLoggingFilter>>();
			var filter = new EndpointLoggingFilter(logger.Object);
			var executingContext = CreateExecutingContext();
			var logCountSeenByNext = -1;

			// Act
			await filter.OnResourceExecutionAsync(executingContext, () =>
			{
				logCountSeenByNext = GetLoggedStates(logger).Count;
				executingContext.HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
				return Task.FromResult(CreateExecutedContext(executingContext));
			});

			// Assert
			var states = GetLoggedStates(logger);
			Assert.That(logCountSeenByNext, Is.EqualTo(1));
			Assert.That(states, Has.Count.EqualTo(2));

			Assert.That(states[0].Level, Is.EqualTo(LogLevel.Information));
			Assert.That(states[0].Message, Does.StartWith("Endpoint start:"));
			Assert.That(GetValue(states[0], "EndpointName"), Is.EqualTo("Categories.GetAll"));
			Assert.That(GetValue(states[0], "HttpMethod"), Is.EqualTo("GET"));
			Assert.That(GetValue(states[0], "RequestPath"), Is.EqualTo("/v1/categories/get-all"));

			Assert.That(states[1].Level, Is.EqualTo(LogLevel.Information));
			Assert.That(states[1].Message, Does.StartWith("Endpoint end:"));
			Assert.That(GetValue(states[1], "EndpointName"), Is.EqualTo("Categories.GetAll"));
			Assert.That(GetValue(states[1], "StatusCode"), Is.EqualTo(StatusCodes.Status204NoContent));
			Assert.That(GetValue(states[1], "ElapsedMilliseconds"), Is.TypeOf<long>());
		}

		[Test]
		public async Task EndpointLoggingFilter_OnResourceExecutionAsync_WithUnhandledException_LogsEndAsWarning_Test()
		{
			// Arrange
			var logger = new Mock<ILogger<EndpointLoggingFilter>>();
			var filter = new EndpointLoggingFilter(logger.Object);
			var executingContext = CreateExecutingContext();

			// Act
			await filter.OnResourceExecutionAsync(executingContext, () =>
			{
				var executedContext = CreateExecutedContext(executingContext);
				executedContext.Exception = new InvalidOperationException("boom");
				return Task.FromResult(executedContext);
			});

			// Assert
			var states = GetLoggedStates(logger);
			Assert.That(states, Has.Count.EqualTo(2));
			Assert.That(states[1].Level, Is.EqualTo(LogLevel.Warning));
			Assert.That(states[1].Message, Does.StartWith("Endpoint end:"));
			Assert.That(GetValue(states[1], "ExceptionType"), Is.EqualTo(nameof(InvalidOperationException)));
		}

		[Test]
		public async Task EndpointLoggingFilter_OnResourceExecutionAsync_WithHandledException_LogsEndAsInformation_Test()
		{
			// Arrange
			var logger = new Mock<ILogger<EndpointLoggingFilter>>();
			var filter = new EndpointLoggingFilter(logger.Object);
			var executingContext = CreateExecutingContext();

			// Act
			await filter.OnResourceExecutionAsync(executingContext, () =>
			{
				var executedContext = CreateExecutedContext(executingContext);
				executedContext.Exception = new InvalidOperationException("boom");
				executedContext.ExceptionHandled = true;
				return Task.FromResult(executedContext);
			});

			// Assert
			var states = GetLoggedStates(logger);
			Assert.That(states[1].Level, Is.EqualTo(LogLevel.Information));
			Assert.That(GetValue(states[1], "StatusCode"), Is.EqualTo(StatusCodes.Status200OK));
		}

		[Test]
		public async Task EndpointLoggingFilter_OnResourceExecutionAsync_WithNonControllerAction_UsesDisplayName_Test()
		{
			// Arrange
			var logger = new Mock<ILogger<EndpointLoggingFilter>>();
			var filter = new EndpointLoggingFilter(logger.Object);
			var executingContext = CreateExecutingContext(new ActionDescriptor { DisplayName = "custom-endpoint" });

			// Act
			await filter.OnResourceExecutionAsync(executingContext, () => Task.FromResult(CreateExecutedContext(executingContext)));

			// Assert
			Assert.That(GetValue(GetLoggedStates(logger)[0], "EndpointName"), Is.EqualTo("custom-endpoint"));
		}

		private static ResourceExecutingContext CreateExecutingContext(ActionDescriptor? actionDescriptor = null)
		{
			var httpContext = new DefaultHttpContext();
			httpContext.Request.Method = HttpMethods.Get;
			httpContext.Request.Path = "/v1/categories/get-all";

			actionDescriptor ??= new ControllerActionDescriptor
			{
				ControllerName = "Categories",
				ActionName = "GetAll"
			};

			var actionContext = new ActionContext(httpContext, new RouteData(), actionDescriptor, new ModelStateDictionary());

			return new ResourceExecutingContext(actionContext, new List<IFilterMetadata>(), new List<IValueProviderFactory>());
		}

		private static ResourceExecutedContext CreateExecutedContext(ResourceExecutingContext executingContext)
		{
			return new ResourceExecutedContext(executingContext, executingContext.Filters);
		}

		private static List<(LogLevel Level, string Message, IReadOnlyList<KeyValuePair<string, object?>> Values)> GetLoggedStates(
			Mock<ILogger<EndpointLoggingFilter>> logger)
		{
			return logger.Invocations
				.Where(invocation => invocation.Method.Name == nameof(ILogger.Log))
				.Select(invocation => (
					(LogLevel)invocation.Arguments[0],
					invocation.Arguments[2].ToString() ?? string.Empty,
					(IReadOnlyList<KeyValuePair<string, object?>>)invocation.Arguments[2]))
				.ToList();
		}

		private static object? GetValue((LogLevel Level, string Message, IReadOnlyList<KeyValuePair<string, object?>> Values) state, string key)
		{
			return state.Values.Single(kvp => kvp.Key == key).Value;
		}
	}
}
