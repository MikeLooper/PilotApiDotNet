using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PilotApi.Shared.Api.Filters
{
	/// <summary>
	/// Logs the beginning and end of every controller endpoint call.
	/// </summary>
	/// <remarks>
	/// This is a resource filter so that it wraps model binding, validation, the action and the
	/// result execution; the logged status code is therefore the one returned to the caller.
	/// The correlation ID, request ID and caller name are attached to each log event by the
	/// <see cref="Middleware.RequestSettingsMiddleware"/> (Serilog log context) and the
	/// OpenTelemetry log processor.
	/// </remarks>
	public sealed class EndpointLoggingFilter : IAsyncResourceFilter
	{
		private readonly ILogger<EndpointLoggingFilter> logger;

		/// <summary>
		/// Create a new <see cref="EndpointLoggingFilter"/> instance.
		/// </summary>
		/// <param name="logger">
		/// A logger object.
		/// </param>
		public EndpointLoggingFilter(ILogger<EndpointLoggingFilter> logger)
		{
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc/>
		public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
		{
			var endpointName = GetEndpointName(context);
			var request = context.HttpContext.Request;

			this.logger.LogInformation(
				"Endpoint start: {EndpointName} ({HttpMethod} {RequestPath})",
				endpointName,
				request.Method,
				request.Path.Value);

			var stopwatch = Stopwatch.StartNew();
			var executedContext = await next();
			stopwatch.Stop();

			if (executedContext.Exception != null && !executedContext.ExceptionHandled)
			{
				// the exception itself is logged by the UnhandledExceptionMiddleware
				this.logger.LogWarning(
					"Endpoint end: {EndpointName} ({HttpMethod} {RequestPath}) failed with {ExceptionType} in {ElapsedMilliseconds} ms",
					endpointName,
					request.Method,
					request.Path.Value,
					executedContext.Exception.GetType().Name,
					stopwatch.ElapsedMilliseconds);

				return;
			}

			this.logger.LogInformation(
				"Endpoint end: {EndpointName} ({HttpMethod} {RequestPath}) returned {StatusCode} in {ElapsedMilliseconds} ms",
				endpointName,
				request.Method,
				request.Path.Value,
				context.HttpContext.Response.StatusCode,
				stopwatch.ElapsedMilliseconds);
		}

		private static string GetEndpointName(ResourceExecutingContext context)
		{
			if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
			{
				return $"{controllerActionDescriptor.ControllerName}.{controllerActionDescriptor.ActionName}";
			}

			return context.ActionDescriptor.DisplayName ?? context.HttpContext.Request.Path.Value ?? string.Empty;
		}
	}
}
