using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PilotApi.Shared.Api.Security;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Contracts.Configuration;
using PilotApi.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PilotApi.Shared.Helpers
{
	/// <summary>
	/// A helper for security-related functionality.
	/// </summary>
	public class SecurityHelper
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityHelper"/> class.
		/// </summary>
		/// <param name="services">
		/// A services collection.
		/// </param>
		public SecurityHelper(IServiceCollection services)
		{
			if (services == null)
			{
				throw new ArgumentException($"Invalid argument : {nameof(services)}. "
					+ $"A valid object type of: '{typeof(IServiceCollection)}' is needed to continue. ({nameof(SecurityHelper)})");
			}

			this.ServiceCollection = services;
			var serviceProvider = services.BuildServiceProvider();

			var applicationConfiguration = serviceProvider.GetRequiredService<IApplicationConfiguration>();
			this.SecurityConfiguration = applicationConfiguration.Security;
			this.Logger = serviceProvider.GetRequiredService<ILogger<SecurityHelper>>();
		}

		/// <summary>
		/// Gets a logger object.
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Gets the security configuration.
		/// </summary>
		protected ISecurityConfiguration SecurityConfiguration { get; }

		/// <summary>
		/// Gets a service collection.
		/// </summary>
		protected IServiceCollection ServiceCollection { get; }

		/// <summary>
		/// Adds security services to the service collection, including authentication and authorization.
		/// </summary>
		public void AddSecurity()
		{
			// The order of configuration matters (authentication before authorization)
			ConfigureAuthentication();
			ConfigureAuthorization();

			RegisterSecurityServices();
		}

		/// <summary>
		/// Add realm roles to the claims collection from the supplied realm_access object.
		/// This structure is typically found in Keycloak-issued JWT tokens.
		/// </summary>
		/// <param name="claims">
		/// The collection of claims to add to.
		/// </param>
		/// <param name="realmAccess">
		/// The realm_access object containing roles.
		/// </param>
		protected void AddRealmRoles(ICollection<Claim> claims, JObject? realmAccess)
		{
			if (realmAccess == null || realmAccess["roles"] is not JArray roles)
			{
				return;
			}

			foreach (var role in roles.Values<string>().Where(role => !string.IsNullOrWhiteSpace(role)))
			{
				claims.Add(new Claim(ClaimTypes.Role, role!));
			}
		}

		/// <summary>
		/// Add resource roles to the claims collection from the supplied resource_access object.
		/// This structure is typically found in Keycloak-issued JWT tokens.
		/// </summary>
		/// <param name="claims">
		/// The collection of claims to add to.
		/// </param>
		/// <param name="resourceAccess">
		/// The resource_access object containing roles.
		/// </param>
		protected void AddResourceRoles(ICollection<Claim> claims, JObject? resourceAccess)
		{
			if (resourceAccess == null)
			{
				return;
			}

			foreach (var resource in resourceAccess.Properties())
			{
				if (resource.Value is not JObject resourceDetails || resourceDetails["roles"] is not JArray roles)
				{
					continue;
				}

				foreach (var role in roles.Values<string>().Where(role => !string.IsNullOrWhiteSpace(role)))
				{
					claims.Add(new Claim(ClaimTypes.Role, role!));
				}
			}
		}

		/// <summary>
		/// Add scopes to the claims collection from the supplied scope token.
		/// </summary>
		/// <param name="claims">
		/// The collection of claims to add to.
		/// </param>
		/// <param name="scopeToken">
		/// The scope token containing scopes.
		/// </param>
		protected void AddScopes(ICollection<Claim> claims, JToken scopeToken)
		{
			var scopeValues = scopeToken.Type == JTokenType.String
				? scopeToken.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries)
				: scopeToken.Values<string>();

			foreach (var scope in scopeValues.Where(scope => !string.IsNullOrWhiteSpace(scope)))
			{
				claims.Add(new Claim("scope", scope!));
			}
		}

		/// <summary>
		/// Clean and return the supplied bearer token Authorization header value.
		/// </summary>
		/// <param name="authorizationHeaderValue">
		/// The Authorization header value to clean.
		/// </param>
		/// <param name="edgeInclusions">
		/// The number of characters to include at the edges of the token.
		/// Default = 4. If the token is shorter than (edgeInclusions * 2), the entire token will be redacted.
		/// </param>
		/// <returns>
		/// A cleaned Authorization header value with the token redacted.
		/// </returns>
		protected string BearerTokenClean(string? authorizationHeaderValue, int edgeInclusions = 4)
		{
			if (string.IsNullOrWhiteSpace(authorizationHeaderValue))
			{
				return StringConstants.LogEmpty;
			}

			if (!authorizationHeaderValue.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
			{
				return SecurityUtilities.Redact(authorizationHeaderValue, edgeInclusions);
			}

			var token = authorizationHeaderValue["Bearer ".Length..];
			if (edgeInclusions <= 0 || token.Length <= (edgeInclusions * 2))
			{
				return $"Bearer {StringConstants.Redacted}";
			}

			var tokenPrefix = token[..edgeInclusions];
			var tokenSuffix = token[^edgeInclusions..];
			return $"Bearer {tokenPrefix}{StringConstants.Redacted}{tokenSuffix}";
		}

		/// <summary>
		/// Clean and return the supplied JWT token from the Authorization header value.
		/// </summary>
		/// <param name="authorizationString">
		/// The Authorization header value to clean.
		/// </param>
		/// <returns>
		/// A cleaned JWT token.
		/// </returns>
		protected string CleanJwtToken(string? authorizationString)
		{
			if (string.IsNullOrWhiteSpace(authorizationString))
			{
				return StringConstants.LogEmpty;
			}

			if (!authorizationString.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
			{
				return SecurityUtilities.Redact(authorizationString);
			}

			var jwt = authorizationString["Bearer ".Length..].Trim();
			if (string.IsNullOrWhiteSpace(jwt))
			{
				return StringConstants.LogEmpty;
			}

			var handler = new JwtSecurityTokenHandler();
			if (!handler.CanReadToken(jwt))
			{
				return SecurityUtilities.Redact(jwt);
			}

			var token = handler.ReadJwtToken(jwt);
			var payload = JObject.Parse(token.Payload.SerializeToJson());

			//// Redact identifiers and personal data so logs cannot be used to correlate users or expose token-linked data.
			//foreach (var property in payload.Properties())
			//{
			//	if (IsSensitiveJwtClaim(property.Name))
			//	{
			//		property.Value = StringConstants.Redacted;
			//	}
			//}

			return payload.ToString(Formatting.None);
		}

		/// <summary>
		/// Configures JWT Bearer authentication scheme.
		/// </summary>
		protected void ConfigureAuthentication()
		{
			this.ServiceCollection.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.Authority = this.SecurityConfiguration.Authority;
				options.Audience = this.SecurityConfiguration.ClientId;
				options.RequireHttpsMetadata = this.SecurityConfiguration.RequireHttpsMetadata;
				options.MapInboundClaims = false; // keep raw "preferred_username" claim name
				options.TokenValidationParameters = ConfigureTokenValidationParameters();
				options.Events = ConfigureJwtBearerEvents();
			});
		}

		/// <summary>
		/// Configures authorization policy and fallback requirements.
		/// </summary>
		protected void ConfigureAuthorization()
		{
			this.ServiceCollection.AddAuthorization(options =>
			{
				options.FallbackPolicy = new AuthorizationPolicyBuilder()
					.RequireAuthenticatedUser()
					.AddRequirements(new HttpVerbRoleRequirement())
					.Build();
			});
		}

		/// <summary>
		/// Configures JWT Bearer authentication event handlers.
		/// </summary>
		/// <returns>
		/// Configured JwtBearerEvents.
		/// </returns>
		protected JwtBearerEvents ConfigureJwtBearerEvents()
		{
			return new JwtBearerEvents
			{
				OnMessageReceived = HandleOnMessageReceived,
				OnTokenValidated = HandleOnTokenValidated,
				OnAuthenticationFailed = HandleOnAuthenticationFailed
			};
		}

		/// <summary>
		/// Configures JWT token validation parameters.
		/// </summary>
		/// <returns>
		/// Configured TokenValidationParameters.
		/// </returns>
		protected TokenValidationParameters ConfigureTokenValidationParameters()
		{
			return new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = this.SecurityConfiguration.Authority,
				ValidateAudience = true,
				AudienceValidator = (audiences, token, validationParameters) =>
				{
					var audienceList = audiences?.ToArray() ?? [];
					var clientId = this.SecurityConfiguration.ClientId;

					return audienceList.Contains(clientId, StringComparer.OrdinalIgnoreCase)
						|| audienceList.Contains("account", StringComparer.OrdinalIgnoreCase);
				},
				ValidateLifetime = true,
				ClockSkew = TimeSpan.FromSeconds(this.SecurityConfiguration.ClockSkewSeconds),
				ValidateIssuerSigningKey = true,
				NameClaimType = SecurityConstants.PreferredUsernameClaimType
			};
		}

		/// <summary>
		/// Extract claims, roles, and scopes from the supplied bearer token Authorization header value.
		/// </summary>
		/// <param name="authorizationString">
		/// The Authorization header value to inspect.
		/// </param>
		/// <returns>
		/// The token claims projected into normal <see cref="Claim"/> instances.
		/// </returns>
		protected IEnumerable<Claim> GetJwtClaims(string? authorizationString)
		{
			if (string.IsNullOrWhiteSpace(authorizationString) ||
				!authorizationString.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
			{
				return Enumerable.Empty<Claim>();
			}

			var jwt = authorizationString["Bearer ".Length..].Trim();
			if (string.IsNullOrWhiteSpace(jwt))
			{
				return Enumerable.Empty<Claim>();
			}

			var handler = new JwtSecurityTokenHandler();
			if (!handler.CanReadToken(jwt))
			{
				return Enumerable.Empty<Claim>();
			}

			var token = handler.ReadJwtToken(jwt);
			var payload = JObject.Parse(token.Payload.SerializeToJson());
			var claims = new List<Claim>();

			foreach (var property in payload.Properties())
			{
				if (property.Name.Equals("realm_access", System.StringComparison.OrdinalIgnoreCase))
				{
					AddRealmRoles(claims, property.Value as JObject);
					claims.Add(new Claim(property.Name, property.Value.ToString(Formatting.None)));
					continue;
				}

				if (property.Name.Equals("resource_access", System.StringComparison.OrdinalIgnoreCase))
				{
					AddResourceRoles(claims, property.Value as JObject);
					claims.Add(new Claim(property.Name, property.Value.ToString(Formatting.None)));
					continue;
				}

				if (property.Name.Equals("scope", System.StringComparison.OrdinalIgnoreCase))
				{
					AddScopes(claims, property.Value.ToString());
					claims.Add(new Claim(property.Name, property.Value.ToString()));
					continue;
				}

				if (property.Name.Equals("scp", System.StringComparison.OrdinalIgnoreCase))
				{
					AddScopes(claims, property.Value);
					claims.Add(new Claim(property.Name, property.Value.ToString(Formatting.None)));
					continue;
				}

				claims.Add(new Claim(property.Name, property.Value.ToString(Formatting.None)));
			}

			return claims;
		}

		/// <summary>
		/// Build an enriched claims principal using claims extracted from the bearer token.
		/// </summary>
		/// <param name="principal">
		/// The existing principal created by authentication.
		/// </param>
		/// <param name="authorizationString">
		/// The Authorization header value to inspect.
		/// </param>
		/// <returns>
		/// A principal with the token-derived roles, claims, and scopes added.
		/// </returns>
		protected ClaimsPrincipal GetJwtPrincipal(ClaimsPrincipal? principal, string? authorizationString)
		{
			var baseIdentity = principal?.Identities.OfType<ClaimsIdentity>().FirstOrDefault();
			var enrichedIdentity = baseIdentity == null
				? new ClaimsIdentity()
				: new ClaimsIdentity(
					baseIdentity.Claims,
					baseIdentity.AuthenticationType,
					baseIdentity.NameClaimType,
					baseIdentity.RoleClaimType);

			if (!string.IsNullOrWhiteSpace(authorizationString))
			{
				foreach (var claim in GetJwtClaims(authorizationString))
				{
					if (!enrichedIdentity.HasClaim(claim.Type, claim.Value))
					{
						enrichedIdentity.AddClaim(claim);
					}
				}
			}

			var enrichedPrincipal = new ClaimsPrincipal(enrichedIdentity);
			if (principal != null)
			{
				foreach (var identity in principal.Identities.Where(identity => !ReferenceEquals(identity, baseIdentity)))
				{
					enrichedPrincipal.AddIdentity(identity);
				}
			}

			return enrichedPrincipal;
		}

		/// <summary>
		/// Handles the OnAuthenticationFailed event - logs authentication failures.
		/// </summary>
		protected Task HandleOnAuthenticationFailed(AuthenticationFailedContext context)
		{
			var isExpired = context.Exception is SecurityTokenExpiredException;
			var redactedAuthorization = this.BearerTokenClean(context.Request.Headers.Authorization.ToString());

			this.Logger.LogWarning(
				context.Exception,
				"{FailureReason} for {Method} {Path}. Authorization: {Authorization}",
				isExpired ? "Token expired" : "Token validation failed",
				context.Request.Method,
				context.Request.Path,
				redactedAuthorization);

			context.HttpContext.Items["AuthFailureReason"] = isExpired ? "Token expired." : "Missing or invalid bearer token.";

			return Task.CompletedTask;
		}

		/// <summary>
		/// Handles the OnMessageReceived event - logs token reception before validation.
		/// </summary>
		protected Task HandleOnMessageReceived(MessageReceivedContext context)
		{
			var authorizationString = context.Request.Headers.Authorization.ToString();
			var redactedAuthorization = this.BearerTokenClean(authorizationString);
			var redactedJwt = this.CleanJwtToken(authorizationString);

			this.Logger.LogInformation(
				"Authentication token received for {Method} {Path}. Authorization: {Authorization}. JWT: {Jwt}. RemoteIp: '{RemoteIp}'",
				context.Request.Method,
				context.Request.Path,
				redactedAuthorization,
				redactedJwt,
				context.HttpContext.Connection.RemoteIpAddress);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Handles the OnTokenValidated event - logs successful authentication.
		/// </summary>
		protected Task HandleOnTokenValidated(TokenValidatedContext context)
		{
			var authorizationString = context.Request.Headers.Authorization.ToString();
			context.Principal = this.GetJwtPrincipal(context.Principal, authorizationString);
			context.HttpContext.User = context.Principal ?? context.HttpContext.User;

			var userId = context.Principal?.FindFirst(SecurityConstants.PreferredUsernameClaimType)?.Value;

			this.Logger.LogInformation(
				"Authentication succeeded for UserId: '{UserId}' from RemoteIp: '{RemoteIp}'",
				userId ?? StringConstants.LogNull,
				context.HttpContext.Connection.RemoteIpAddress);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Determine if the supplied claim name is considered sensitive and should be redacted from logs.
		/// </summary>
		/// <param name="claimName">
		/// The name of the claim to check.
		/// </param>
		/// <returns>
		/// True if the claim is considered sensitive; otherwise, false.
		/// </returns>
		protected bool IsSensitiveJwtClaim(string claimName)
		{
			return claimName.Equals("sub", System.StringComparison.OrdinalIgnoreCase)
				|| claimName.Equals("sid", System.StringComparison.OrdinalIgnoreCase)
				|| claimName.Equals("jti", System.StringComparison.OrdinalIgnoreCase)
				|| claimName.Equals("name", System.StringComparison.OrdinalIgnoreCase)
				|| claimName.Equals("preferred_username", System.StringComparison.OrdinalIgnoreCase)
				|| claimName.Equals("given_name", System.StringComparison.OrdinalIgnoreCase)
				|| claimName.Equals("family_name", System.StringComparison.OrdinalIgnoreCase)
				|| claimName.Equals("email", System.StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Registers security-related singleton services.
		/// </summary>
		protected void RegisterSecurityServices()
		{
			this.ServiceCollection.AddSingleton(this.SecurityConfiguration);
			this.ServiceCollection.AddSingleton<IAuthorizationHandler, HttpVerbRoleAuthorizationHandler>();
			this.ServiceCollection.AddSingleton<IAuthorizationMiddlewareResultHandler, BypassOnInactiveAuthorizationMiddlewareResultHandler>();
		}
	}
}
