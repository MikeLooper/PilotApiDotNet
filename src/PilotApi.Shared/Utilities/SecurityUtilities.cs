using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTelemetry.Resources;
using PilotApi.Shared.Constants;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace PilotApi.Shared.Utilities
{
	/// <summary>
	/// Utility methods used with security processes.
	/// </summary>
	public static class SecurityUtilities
	{
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
		public static string BearerTokenClean(string? authorizationHeaderValue, int edgeInclusions = 4)
		{
			if (string.IsNullOrWhiteSpace(authorizationHeaderValue))
			{
				return authorizationHeaderValue ?? string.Empty;
			}

			if (!authorizationHeaderValue.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
			{
				return StringConstants.Redacted;
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
		public static string CleanJwtToken(string? authorizationString)
		{
			if (string.IsNullOrWhiteSpace(authorizationString) ||
				!authorizationString.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
			{
				return StringConstants.Redacted;
			}

			var jwt = authorizationString["Bearer ".Length..].Trim();
			if (string.IsNullOrWhiteSpace(jwt))
			{
				return StringConstants.Redacted;
			}

			var handler = new JwtSecurityTokenHandler();
			if (!handler.CanReadToken(jwt))
			{
				return StringConstants.Redacted;
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
		/// Determine if the supplied claim name is considered sensitive and should be redacted from logs.
		/// </summary>
		/// <param name="claimName">
		/// The name of the claim to check.
		/// </param>
		/// <returns>
		/// True if the claim is considered sensitive; otherwise, false.
		/// </returns>
		private static bool IsSensitiveJwtClaim(string claimName)
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
		/// Extract claims, roles, and scopes from the supplied bearer token Authorization header value.
		/// </summary>
		/// <param name="authorizationString">
		/// The Authorization header value to inspect.
		/// </param>
		/// <returns>
		/// The token claims projected into normal <see cref="Claim"/> instances.
		/// </returns>
		public static IEnumerable<Claim> GetJwtClaims(string? authorizationString)
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
		public static ClaimsPrincipal GetJwtPrincipal(ClaimsPrincipal? principal, string? authorizationString)
		{
			var baseIdentity = principal?.Identities.OfType<ClaimsIdentity>().FirstOrDefault();
			var enrichedIdentity = baseIdentity == null
				? new ClaimsIdentity()
				: new ClaimsIdentity(
					baseIdentity.Claims,
					baseIdentity.AuthenticationType,
					baseIdentity.NameClaimType,
					baseIdentity.RoleClaimType);

			foreach (var claim in GetJwtClaims(authorizationString))
			{
				if (!enrichedIdentity.HasClaim(claim.Type, claim.Value))
				{
					enrichedIdentity.AddClaim(claim);
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
		/// Add realm roles to the claims collection from the supplied realm_access object.
		/// This structure is typically found in Keycloak-issued JWT tokens.
		/// </summary>
		/// <param name="claims">
		/// The collection of claims to add to.
		/// </param>
		/// <param name="realmAccess">
		/// The realm_access object containing roles.
		/// </param>
		private static void AddRealmRoles(ICollection<Claim> claims, JObject? realmAccess)
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
		private static void AddResourceRoles(ICollection<Claim> claims, JObject? resourceAccess)
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
		private static void AddScopes(ICollection<Claim> claims, JToken scopeToken)
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
		/// Clean and return the supplied ConnectionString.
		/// </summary>
		/// <param name="connectionString">
		/// The ConnectionString to clean.
		/// </param>
		/// <returns>
		///  A cleaned ConnectionString.
		/// </returns>
		public static string ConnectionStringClean(string connectionString)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				return connectionString;
			}

			var connectionStringCleaned = new StringBuilder();
			var connectionParts = connectionString.Split(";", System.StringSplitOptions.RemoveEmptyEntries);
			foreach ( var part in connectionParts )
			{
				var partParts = part.Split("=", System.StringSplitOptions.RemoveEmptyEntries);
				if (partParts.Length < 2)
				{
					continue;
				}

				if (partParts[0].Equals("Password", System.StringComparison.OrdinalIgnoreCase))
				{
					partParts[1] = StringConstants.Redacted;
				}

				if (connectionStringCleaned.Length > 0)
				{
					connectionStringCleaned.Append(";");
				}

				connectionStringCleaned.Append($"{partParts[0]}={partParts[1]}");
			}

			return connectionStringCleaned.ToString();
		}
	}
}
