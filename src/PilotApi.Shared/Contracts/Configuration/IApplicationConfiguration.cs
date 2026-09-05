using PilotApi.Shared.Configuration;
using PilotApi.Shared.Contracts.Configuration.Base;
using System.Collections.Generic;

namespace PilotApi.Shared.Contracts.Configuration
{
	/// <summary>
	/// Configuration for the application.
	/// </summary>
	/// <example>
	/// Example configuration:
	/// <code>
	/// {
	///		"DataConnections": [
	///			{
	///				"ConnectTimeout": 0,
	///				"DataSourceName": "SampleDataSource",
	///				"Host": "localhost",
	///				"Password": "[.........]",
	///				"Port": 0,
	///				"UserName": "SampleUser"
	/// 		}
	///		],
	///		"DataSources": [
	///			{
	///				"Active": true,
	///				"DataSourceName": "SampleDataSource",
	///				"DataSource": "NorthWind",
	///				"DataSourceType": "SqlServer",
	///				"Schema": "dbo"
	///			}
	///		],
	///		"Security": {
	///			"Active": true,
	///			"BaseUrl": "http://localhost:55001",
	///			"Realm": "local-realm",
	///			"ClientId": "local-client",
	///			"RequireHttpsMetadata": false,
	///			"ClockSkewSeconds": 60
	///		},
	///		"OpenApi": {
	///			"Title": "PilotApi",
	///			"Contact": {
	///				"Email": "MikelLooper@gmail.com",
	///				"Name": "Michael Looper",
	///				"URL": "https://github.com/MikeLooper/PilotApi"
	///			},
	///			"Description": "A proof of concept API to explore best-practices and new ideas",
	///			"License": "MIT",
	///			"Summary": "Proof of concept API",
	///			"Version":  "0.1.1"
	///		},
	///		"OpenTelemetry": {
	///			"Server": "localhost",
	///			"Port": 4318
	///		}
	/// }
	/// </code>
	/// </example>
	public interface IApplicationConfiguration : IConfigurationBase
	{
		/// <summary>
		/// Gets or sets the list of data connection configurations for the application.
		/// </summary>
		List<DataConnectionConfiguration>? DataConnections { get; set; }

		/// <summary>
		/// Gets or sets the list of data source configurations for the application.
		/// </summary>
		List<DataSourceConfiguration>? DataSources { get; set; }

		/// <summary>
		/// Gets or sets an object with Security settings.
		/// </summary>
		SecurityConfiguration? Security { get; set; }

		/// <summary>
		/// Gets or sets an object with OpenApi settings.
		/// </summary>
		OpenApiConfiguration? OpenApi { get; set; }

		/// <summary>
		/// Gets or sets an object with Open Telemetry settings.
		/// </summary>
		OpenTelemetryConfiguration? OpenTelemetry { get; set; }

		/// <summary>
		/// Return a <see cref="DataSourceConfiguration"/> object for the supplied data source name.
		/// </summary>
		/// <param name="dataSourceName">
		/// The name of the desired data source section.
		/// </param>
		/// <returns>
		/// A <see cref="DataSourceConfiguration"/> object.
		/// </returns>
		DataSourceConfiguration? GetDataSource(string dataSourceName);
	}
}
