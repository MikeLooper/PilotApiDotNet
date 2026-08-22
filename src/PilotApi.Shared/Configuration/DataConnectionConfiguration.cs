using Newtonsoft.Json;
using PilotApi.Shared.Configuration.Base;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Contracts.Configuration;
using PilotApi.Shared.Exceptions;
using System;
using System.Collections.Generic;

namespace PilotApi.Shared.Configuration
{
	/// <summary>
	/// Configuration for a data source connection.
	/// </summary>
	/// <example>
	/// Example SQL Server connection string:
	/// <code>
	/// Data Source=localhost;Initial Catalog=SampleDatabase;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;
	/// </code>
	/// Example PostGreSQL connection string:
	/// <code>
	/// Host=localhost;Port=5432;Database=eCommerceUsers;Username=postgres;Password=admin;
	/// </code>
	/// Example configuration:
	/// <code>
	/// {
	///		"Active": true,
	///		"ConnectTimeout": 0,
	///		"DataSourceName": "SampleDatabase",
	///		"Host": "localhost",
	///		"Password": "sedrt^FLKNR434",
	///		"Port": 0,
	///		"UserName": "SampleUser"
	/// }
	/// </code>
	/// </example>
	[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]

	public class DataConnectionConfiguration : ConfigurationBase, IDataConnectionConfiguration
	{
		/// <summary>
		/// Instatiate a <see cref="DataConnectionConfiguration"/> object.
		/// </summary>
		public DataConnectionConfiguration()
		{
		}

		/// <summary>
		/// Instantiate a <see cref="DataConnectionConfiguration"/> object.
		/// </summary>
		/// <param name="sourceConfiguration">
		/// A source configuration object to copy values from.
		/// </param>
		/// <param name="suppressSensitiveValues">
		/// A flag that indicates whether sensitive values should be suppressed when copying values from the source configuration.
		/// </param>
		public DataConnectionConfiguration(
			DataConnectionConfiguration sourceConfiguration,
			bool suppressSensitiveValues = false)
			: this()
		{
			this.Initialize(sourceConfiguration, suppressSensitiveValues);
		}

		/// <inheritdoc/>>
		[JsonProperty]
		public int ConnectTimeout { get; set; } = 0;

		/// <inheritdoc/>>
		[JsonProperty]
		public string? DataSourceName { get; set; }

		/// <inheritdoc/>>
		[JsonProperty]
		public string? Host { get; set; }

		/// <inheritdoc/>>
		[JsonProperty]
		public string? Password { get; set; }

		/// <inheritdoc/>>
		[JsonProperty]
		public int? Port { get; set; }

		/// <inheritdoc/>>
		[JsonProperty]
		public string? UserName { get; set; }

		/// <inheritdoc/>>
		public override string ToString()
		{
			return $"{nameof(this.Active)}={this.Active}, " +
				$"{nameof(this.ConnectTimeout)}={this.ConnectTimeout}, " +
				$"{nameof(this.DataSourceName)}={this.DataSourceName}, " +
				$"{nameof(this.Host)}={this.Host}, " +
				$"{nameof(this.Password)}={(string.IsNullOrWhiteSpace(Password) ? StringConstants.LogEmpty : StringConstants.Redacted)}, " +
				$"{nameof(this.Port)}={this.Port}, " +
				$"{nameof(this.UserName)}={this.UserName}";
		}

		/// <inheritdoc/>>
		public override void Validate(ref List<Exception> exceptions)
		{
			if (exceptions == null)
			{
				throw new ArgumentException($"The {nameof(exceptions)} argument is invalid ({this.GetType().Name})");
			}

			if (string.IsNullOrWhiteSpace(this.DataSourceName))
			{
				exceptions.Add(
					new ConfigurationException($"The {nameof(this.DataSourceName)} value is required and cannot be null or empty ({this.GetType().Name})"));
			}

			if (string.IsNullOrWhiteSpace(this.Host))
			{
				exceptions.Add(
					new ConfigurationException($"The {nameof(this.Host)} value is required and cannot be null or empty ({this.GetType().Name})"));
			}

			if (string.IsNullOrWhiteSpace(this.Password))
			{
				exceptions.Add(
					new ConfigurationException($"The {nameof(this.Password)} value is required and cannot be null or empty ({this.GetType().Name})"));
			}

			if (string.IsNullOrWhiteSpace(this.UserName))
			{
				exceptions.Add(
					new ConfigurationException($"The {nameof(this.UserName)} value is required and cannot be null or empty ({this.GetType().Name})"));
			}
		}

		/// <summary>
		/// Initialize the current object with values from the source configuration.
		/// </summary>
		/// <param name="sourceConfiguration">
		/// The source <see cref="DataConnectionConfiguration"/> to copy values from.
		/// </param>
		/// <param name="suppressSensitiveValues">
		/// A flag that indicates whether sensitive values should be suppressed when copying values from the source configuration.
		/// </param>
		protected void Initialize(
			DataConnectionConfiguration sourceConfiguration,
			bool suppressSensitiveValues = false)
		{
			if (sourceConfiguration == null)
			{
				throw new ArgumentException($"Invalid argument: {nameof(sourceConfiguration)}");
			}

			this.Active = sourceConfiguration.Active;
			this.ConnectTimeout = sourceConfiguration.ConnectTimeout;
			this.DataSourceName = sourceConfiguration.DataSourceName;
			this.Host = sourceConfiguration.Host;
			this.Password = suppressSensitiveValues ? StringConstants.Redacted : sourceConfiguration.Password;
			this.Port = sourceConfiguration.Port;
			this.UserName = sourceConfiguration.UserName;
		}
	}
}
