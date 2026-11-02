using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Models;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace PilotApi.Shared.Api.Middleware
{
	/// <summary>
	/// Initializes the request-scoped <see cref="RequestSettings"/> object at the start
	/// of a request and assigns the correlation/request IDs to the response headers at
	/// the end of the request.
	/// </summary>
	public sealed class RequestSettingsMiddleware
	{
		private readonly RequestDelegate next;

		/// <summary>
		/// Create a new <see cref="RequestSettingsMiddleware"/> instance.
		/// </summary>
		/// <param name="next">
		/// The next middleware in the pipeline.
		/// </param>
		public RequestSettingsMiddleware(RequestDelegate next)
		{
			this.next = next ?? throw new ArgumentNullException(nameof(next));
		}

		/// <summary>
		/// Invoke the middleware.
		/// </summary>
		/// <param name="context">
		/// The current HTTP context.
		/// </param>
		/// <param name="requestSettings">
		/// The request-scoped <see cref="RequestSettings"/> object.
		/// </param>
		/// <returns>
		/// A task that represents the asynchronous operation.
		/// </returns>
		public async Task InvokeAsync(HttpContext context, RequestSettings requestSettings)
		{
			// at the request start, read/generate a new request ID and assign it to the request settings
			if (context.Request.Headers.TryGetValue(HeaderConstants.CorrelationIdHeaderName, out StringValues correlationId)
				&& !StringValues.IsNullOrEmpty(correlationId))
			{
				requestSettings.CorrelationId = correlationId.ToString();
			}

			if (context.Request.Headers.TryGetValue(HeaderConstants.CallerNameHeaderName, out StringValues callerName)
				&& !StringValues.IsNullOrEmpty(callerName))
			{
				requestSettings.CallerName = callerName.ToString();
			}

			// when the request ends, assign the correlation/request IDs to the response headers
			context.Response.OnStarting(() =>
			{
				context.Response.Headers[HeaderConstants.CorrelationIdHeaderName] = requestSettings.CorrelationId;
				context.Response.Headers[HeaderConstants.RequestIdHeaderName] = requestSettings.RequestId;

				return Task.CompletedTask;
			});

			// for the rest of the request, every Serilog event (endpoint, service and database logging)
			// carries the correlation/request IDs and caller name; the OpenTelemetry log processor adds the same values
			using (LogContext.PushProperty(LogContextPropertyConstants.CorrelationId, requestSettings.CorrelationId))
			using (LogContext.PushProperty(LogContextPropertyConstants.RequestId, requestSettings.RequestId))
			using (LogContext.PushProperty(LogContextPropertyConstants.CallerName, requestSettings.CallerName))
			{
				// call the next middleware in the pipeline
				await this.next(context);
			}
		}
	}
}
