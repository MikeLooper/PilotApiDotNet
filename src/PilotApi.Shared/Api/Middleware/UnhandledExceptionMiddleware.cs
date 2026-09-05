using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
				if (context.Response.HasStarted)
				{
					throw;
				}

				var problemDetails = new ProblemDetails
				{
					Status = StatusCodes.Status500InternalServerError,
					Title = "Internal Server Error",
					Detail = uExc.Message,
					Instance = context.Request.Path
				};
		
				context.Response.ContentType = "application/json";
				context.Response.StatusCode = problemDetails.Status.Value;

				await context.Response.WriteAsJsonAsync(problemDetails);
			}
			catch (Exception exception)
			{
				// unhandled and unlogged - log and respond with a generic error message for the user
				var loggingCorrelation = LoggingUtilities.GetLoggingCorrelation();
				this.logger.LogError(exception, "{UserMessage}", loggingCorrelation.UserMessage);

				var problemDetails = new ProblemDetails
				{
					Status = StatusCodes.Status500InternalServerError,
					Title = "Internal Server Error",
					Detail = loggingCorrelation.UserMessage,
					Instance = context.Request.Path
				};
		
				context.Response.ContentType = "application/json";
				context.Response.StatusCode = problemDetails.Status.Value;

				await context.Response.WriteAsJsonAsync(problemDetails);
			}
		}
	}
}