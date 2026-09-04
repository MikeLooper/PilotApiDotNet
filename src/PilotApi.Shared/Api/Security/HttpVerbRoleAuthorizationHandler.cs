using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using PilotApi.Shared.Constants;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PilotApi.Shared.Api.Security
{
	/// <summary>
	/// An authorization handler that succeeds when the current user's role claim allows the current HTTP verb.
	/// </summary>
	/// <remarks>
	/// The verb→role mapping is cumulative: <see cref="RoleNames.ReadOnly"/> allows GET; <see cref="RoleNames.ReadWrite"/>
	/// allows GET, POST, PUT; <see cref="RoleNames.Admin"/> allows GET, POST, PUT, DELETE.
	/// </remarks>
	public sealed class HttpVerbRoleAuthorizationHandler : AuthorizationHandler<HttpVerbRoleRequirement>
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="HttpVerbRoleAuthorizationHandler"/> class.
		/// </summary>
		/// <param name="logger">
		/// A logger object.
		/// </param>
		public HttpVerbRoleAuthorizationHandler(ILogger<HttpVerbRoleAuthorizationHandler> logger)
		{
			this.Logger = logger;
		}

		private ILogger<HttpVerbRoleAuthorizationHandler> Logger { get; }

		/// <inheritdoc/>
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HttpVerbRoleRequirement requirement)
		{
			var httpContext = context.Resource as HttpContext
				?? (context.Resource as AuthorizationFilterContext)?.HttpContext;

			var authorizationString = httpContext.Request.Headers.Authorization.ToString();
			if (string.IsNullOrWhiteSpace(authorizationString))
			{
				context.Fail(new AuthorizationFailureReason(this, "The Authorization request header is empty."));
				return Task.CompletedTask;
			}
			
			if (httpContext == null)
			{
				context.Fail(new AuthorizationFailureReason(this, "Unable to resolve the current HTTP context."));
				return Task.CompletedTask;
			}

			var method = httpContext.Request.Method;
			var roles = context.User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(s => s.Value).ToList();

			if (roles.Count == 0)
			{
				this.Logger.LogWarning(
					"Authorization failed: no role claim was found for {Method} {Path} ({HandlerType})",
					method,
					httpContext.Request.Path,
					this.GetType().Name);

				context.Fail(new AuthorizationFailureReason(this, "No role has been assigned to the current user."));
				return Task.CompletedTask;
			}

			var allowed = roles.Any(role =>
				RoleNames.MapToVerbs.TryGetValue(role, out var allowedHttpMethods) &&
				allowedHttpMethods.Contains(method, StringComparer.OrdinalIgnoreCase));

			if (allowed)
			{
				context.Succeed(requirement);
			}
			else
			{
				this.Logger.LogWarning(
					"Authorization failed: Role(s) '{Roles}' do not allow HTTP verb '{Method}' for {Path} ({HandlerType})",
					string.Join(", ", roles),
					method,
					httpContext.Request.Path,
					this.GetType().Name);

				context.Fail(new AuthorizationFailureReason(this, $"The assigned role(s) ({string.Join(", ", roles)}) do not permit the '{method}' operation."));
			}

			return Task.CompletedTask;
		}
	}
}
