using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Contracts.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace PilotApi.Shared.Api.Security
{
	/// <summary>
	/// An <see cref="IAuthorizationMiddlewareResultHandler"/> that, when the Security configuration's Active flag
	/// is false, allows a request through despite a failed authentication/authorization result, while adding a
	/// <see cref="SecurityConstants.WarningHeaderName"/> response header describing what failed.
	/// </summary>
	public sealed class BypassOnInactiveAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
	{
		private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

		/// <summary>
		/// Instantiates a new instance of the <see cref="BypassOnInactiveAuthorizationMiddlewareResultHandler"/> class.
		/// </summary>
		/// <param name="securityConfiguration">
		/// A Security configuration object.
		/// </param>
		/// <param name="logger">
		/// A logger object.
		/// </param>
		public BypassOnInactiveAuthorizationMiddlewareResultHandler(
			ISecurityConfiguration securityConfiguration,
			ILogger<BypassOnInactiveAuthorizationMiddlewareResultHandler> logger)
		{
			this.SecurityConfiguration = securityConfiguration;
			this.Logger = logger;
		}

		private ISecurityConfiguration SecurityConfiguration { get; }

		private ILogger<BypassOnInactiveAuthorizationMiddlewareResultHandler> Logger { get; }

		/// <inheritdoc/>
		public async Task HandleAsync(
			RequestDelegate next,
			HttpContext context,
			AuthorizationPolicy policy,
			PolicyAuthorizationResult authorizeResult)
		{
			if (authorizeResult.Succeeded || this.SecurityConfiguration.Active)
			{
				await this.defaultHandler.HandleAsync(next, context, policy, authorizeResult);
				return;
			}

			// Security.Active == false AND (authentication or authorization) failed -> bypass, but warn
			var warningMessage = BuildWarningMessage(context, authorizeResult);
			context.Response.Headers[SecurityConstants.WarningHeaderName] = warningMessage;

			this.Logger.LogWarning(
				"Security bypass: allowing {Method} {Path} through despite a failed security check because Security.Active is false. Reason: {Reason}",
				context.Request.Method,
				context.Request.Path,
				warningMessage);

			await next(context);
		}

		/// <summary>
		/// Builds a warning message to include in the response header when bypassing security due to Security.Active being false.
		/// </summary>
		/// <param name="context">
		/// The HTTP context.
		/// </param>
		/// <param name="authorizeResult">
		/// The result of the authorization policy evaluation.
		/// </param>
		/// <returns>
		/// A warning message describing why the request is being allowed despite a failed security check.
		/// </returns>
		private static string BuildWarningMessage(HttpContext context, PolicyAuthorizationResult authorizeResult)
		{
			if (authorizeResult.Forbidden)
			{
				var reasons = authorizeResult.AuthorizationFailure?.FailureReasons.Select(s => s.Message) ?? Enumerable.Empty<string>();
				var reasonsList = reasons.ToList();
				return reasonsList.Count > 0
					? string.Join("; ", reasonsList)
					: "Insufficient role for the requested operation.";
			}

			// Challenged: AuthorizationFailure is null here; the real reason lives on the authentication feature.
			if (context.Items.TryGetValue("AuthFailureReason", out var reason) && reason is string reasonText)
			{
				return reasonText;
			}

			var authenticateResult = context.Features.Get<IAuthenticateResultFeature>()?.AuthenticateResult;
			return string.IsNullOrWhiteSpace(authenticateResult?.Failure?.Message)
				? "Missing or invalid bearer token."
				: authenticateResult.Failure.Message;
		}
	}
}
