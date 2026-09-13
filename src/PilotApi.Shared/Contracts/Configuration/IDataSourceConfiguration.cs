using PilotApi.Shared.Constants;
using PilotApi.Shared.Contracts.Configuration.Base;

namespace PilotApi.Shared.Contracts.Configuration
{
	/// <summary>
	/// Configuration for a data source.
	/// </summary>
	/// <example>
	/// Example configuration:
	/// <code>
	/// {
	///		"Active": true,
	///		"DataSourceName": "SampleDataSource",
	///		"DataSource": "NorthWind",
	///		"DataSourceType": "SqlServer",
	///		"Schema": "dbo"
	/// }
	/// </code>
	/// </example>

	public interface IDataSourceConfiguration : IConfigurationBase
	{
		/// <summary>
		/// Gets or sets the name of the data source (aka: a database).
		/// </summary>
		string? DataSource { get; set; }

		/// <summary>
		/// Gets or sets the data source type.
		/// This will one of the <see cref="DataSourceTypes"/> options.
		/// </summary>
		DataSourceTypes DataSourceEnum { get; set; }

		/// <summary>
		/// Gets or sets the name of the current data source settings section.
		/// This value will match an Application.DataConnections[???].DataSourceName setting.
		/// </summary>
		string? DataSourceName { get; set; }

		/// <summary>
		/// Gets or sets the data source type (e.g., "SqlServer", "PostgreSQL").
		/// This value must match one of the <see cref="DataSourceTypes"/> options, in string form.
		/// </summary>
		string? DataSourceType { get; set; }

		/// <summary>
		/// Gets or sets the data source schema (e.g., "dbo")
		/// For best results, do not include delimiters ([,],") on this value.
		/// </summary>
		string? Schema { get; set; }
	}
}
