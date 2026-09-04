using PilotApi.Shared.Contracts.Configuration.Base;

namespace PilotApi.Shared.Contracts.Configuration
{
	/// <summary>
	/// Configuration for Security authentication/authorization settings.
	/// </summary>
	/// <example>
	/// Example configuration:
	/// <code>
	/// {
	///		"Active": true,
	///		"BaseUrl": "http://local-keycloak:8080",
	///		"PublicBaseUrl": "http://localhost:55001",
	///		"Realm": "local-realm",
	///		"ClientId": "local-client",
	///		"RequireHttpsMetadata": false,
	///		"ClockSkewSeconds": 60
	/// }
	/// </code>
	/// </example>
	public interface ISecurityConfiguration : IConfigurationBase
	{
		/// <summary>
		/// Gets or sets the base URL this API uses to reach the Security server directly
		/// (e.g. "http://local-keycloak:8080" when both run in the same Docker network).
		/// Used to fetch signing keys/metadata, so it must be reachable from this API's host.
		/// </summary>
		string? BaseUrl { get; set; }

		/// <summary>
		/// Gets or sets the externally-visible base URL of the Security server, i.e. the one
		/// clients use to obtain tokens (e.g. "http://localhost:55001"). Tokens carry this value
		/// as their issuer, so it is what <see cref="PublicAuthority"/> validates against. Falls
		/// back to <see cref="BaseUrl"/> when not set, which is correct whenever this API and its
		/// clients reach the Security server the same way (e.g. local development).
		/// </summary>
		string? PublicBaseUrl { get; set; }

		/// <summary>
		/// Gets or sets the Security realm (e.g. "local-realm").
		/// </summary>
		string? Realm { get; set; }

		/// <summary>
		/// Gets or sets the Security client Id (e.g. "local-client").
		/// </summary>
		string? ClientId { get; set; }

		/// <summary>
		/// Gets or sets a flag that indicates whether HTTPS is required for the Security metadata endpoint.
		/// </summary>
		bool RequireHttpsMetadata { get; set; }

		/// <summary>
		/// Gets or sets the clock skew tolerance, in seconds, applied to token expiry validation.
		/// </summary>
		int ClockSkewSeconds { get; set; }

		/// <summary>
		/// Gets the computed Security authority URL, based on <see cref="BaseUrl"/> and <see cref="Realm"/>.
		/// Used to reach the Security server directly (metadata/signing-key retrieval), not for
		/// issuer validation - see <see cref="PublicAuthority"/> for that.
		/// </summary>
		string Authority { get; }

		/// <summary>
		/// Gets the computed, externally-visible Security authority (issuer) URL, based on
		/// <see cref="PublicBaseUrl"/> (or <see cref="BaseUrl"/> when unset) and <see cref="Realm"/>.
		/// This is the value tokens carry as their issuer, so it is what token validation checks against.
		/// </summary>
		string PublicAuthority { get; }
	}
}
