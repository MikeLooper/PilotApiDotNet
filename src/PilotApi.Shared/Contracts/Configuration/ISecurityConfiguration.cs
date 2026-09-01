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
	///		"BaseUrl": "http://localhost:55001",
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
		/// Gets or sets the base URL of the Security server (e.g. "http://localhost:55001").
		/// </summary>
		string? BaseUrl { get; set; }

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
		/// Gets the computed Security authority (issuer) URL, based on <see cref="BaseUrl"/> and <see cref="Realm"/>.
		/// </summary>
		string Authority { get; }
	}
}
