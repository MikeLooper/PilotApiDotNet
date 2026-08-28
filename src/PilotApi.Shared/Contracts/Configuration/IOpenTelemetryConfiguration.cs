using PilotApi.Shared.Contracts.Configuration.Base;

namespace PilotApi.Shared.Contracts.Configuration
{
	/// <summary>
	/// Configuration for OpenTelemetry settings.
	/// </summary>
	/// <example>
	/// Example configuration:
	/// <code>
	/// {
	///		"Server": "localhost",
	///		"Port": 4318
	/// }
	/// </code>
	/// </example>
	public interface IOpenTelemetryConfiguration : IConfigurationBase
	{
		/// <summary>
		/// Gets or sets the OpenTelemetry Server.
		/// </summary>
		string? Server { get; set; }

		/// <summary>
		/// Gets or sets the OpenTelemetry Port.
		/// </summary>
		int? Port { get; set; }
	}
}
