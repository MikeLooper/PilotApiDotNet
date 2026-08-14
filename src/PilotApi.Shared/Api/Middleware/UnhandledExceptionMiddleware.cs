using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PilotApi.Shared.Exceptions;
using PilotApi.Shared.Logging;
using System;
using System.Threading.Tasks;

namespace PilotApi.Shared.Api.Middleware
{
	/// <summary>
	/// Handles unhandled exceptions for the application pipeline.
	/// </summary>
	public sealed class UnhandledExceptionMiddleware
	{
		private readonly ILogger<UnhandledExceptionMiddleware> logger;
		private readonly RequestDelegate next;

		/// <summary>
		/// Create a new <see cref="UnhandledExceptionMiddleware"/> instance.
		/// </summary>
		/// <param name="next">
		/// The next middleware in the pipeline.
		/// </param>
		/// <param name="logger">
		/// The logger used to record exception details.
		/// </param>
		public UnhandledExceptionMiddleware(RequestDelegate next, ILogger<UnhandledExceptionMiddleware> logger)
		{
			this.next = next ?? throw new ArgumentNullException(nameof(next));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Invoke the middleware.
		/// </summary>
		/// <param name="context">
		/// The current HTTP context.
		/// </param>
		/// <returns>
		/// A task that represents the asynchronous operation.
		/// </returns>
		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await this.next(context);
			}
			catch (UserException uExc)
			{
				// already logged, update response with error message for the user
				context.Response.ContentType = "application/json";
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;

				var errorPayload = new { message = uExc.Message };
				await context.Response.WriteAsJsonAsync(errorPayload);
			}
			catch (Exception exception)
			{
				var loggingCorrelation = LoggingUtilities.GetLoggingCorrelation();
				this.logger.LogError(exception, "{UserMessage}", loggingCorrelation.UserMessage);

				throw new UserException(
					$"An error occurred. The details can be found in the log with the following correlation ID: {loggingCorrelation.CorrelationId}",
					exception);
			}
		}
	}
}