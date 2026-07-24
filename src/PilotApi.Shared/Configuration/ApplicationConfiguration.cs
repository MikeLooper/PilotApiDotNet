using Newtonsoft.Json;
using PilotApi.Shared.Configuration.Base;
using PilotApi.Shared.Contracts.Configuration;
using PilotApi.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PilotApi.Shared.Configuration
{
	/// <inheritdoc/>
	[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
	public class ApplicationConfiguration : ConfigurationBase, IApplicationConfiguration
	{
		/// <summary>
		/// Instatiate a <see cref="ApplicationConfiguration"/> object.
		/// </summary>
		public ApplicationConfiguration()
		{
			this.DataConnections = new List<DataConnectionConfiguration>();
			this.DataSources = new List<DataSourceConfiguration>();
			this.OpenApi = new OpenApiConfiguration();
		}

		/// <summary>
		/// Instantiate a <see cref="ApplicationConfiguration"/> object.
		/// </summary>
		/// <param name="sourceConfiguration">
		/// A source configuration object to copy values from.
		/// </param>
		/// <param name="suppressSensitiveValues">
		/// A flag that indicates whether sensitive values should be suppressed when copying values from the source configuration.
		/// </param>
		public ApplicationConfiguration(
			IApplicationConfiguration sourceConfiguration,
			bool suppressSensitiveValues = false)
			: this()
		{
			this.Initialize(sourceConfiguration, suppressSensitiveValues);
		}

		/// <inheritdoc/>>
		[JsonProperty]
		public List<DataConnectionConfiguration>? DataConnections { get; set; }

		/// <inheritdoc/>>
		[JsonProperty]
		public List<DataSourceConfiguration>? DataSources { get; set; }

		/// <inheritdoc/>>
		public OpenApiConfiguration? OpenApi { get; set; }

		/// <inheritdoc/>>
		public DataSourceConfiguration? GetDataSource(string dataSourceName)
		{
			var dataSource = this.DataSources.FirstOrDefault(fod => fod.DataSourceName.Equals(dataSourceName, StringComparison.Ordinal));
			return dataSource;
		}

		/// <inheritdoc/>>
		public override string ToString()
		{
			return $"{nameof(this.Active)}={this.Active}, " +
				$"{nameof(this.DataConnections)}=[{this.DataConnections}], " +
				$"{nameof(this.DataSources)}=[{this.DataSources}], " +
				$"{nameof(this.OpenApi)}=[{this.OpenApi}]";
		}

		/// <inheritdoc/>
		public void Validate()
		{
			var exceptions = new List<Exception>();

			if (this.DataConnections == null ||
				this.DataConnections.Count == 0)
			{
				throw new ConfigurationException(
					$"The {nameof(this.DataConnections)} property is required and cannot be null or empty ({this.GetType().Name})");
			}
			else
			{
				foreach (var dataConnection in this.DataConnections)
				{
					dataConnection.Validate(ref exceptions);
				}

				var activeCount = this.DataConnections
					.Where(w => w.Active)
					.Count();
				if (activeCount != 1)
				{
					throw new ConfigurationException(
						$"The {nameof(this.DataConnections)} property should have one active item ({this.GetType().Name})");
				}
			}

			if (this.DataSources == null ||
				this.DataSources.Count == 0)
			{
				exceptions.Add(
					new ConfigurationException(
						$"The {nameof(this.DataSources)} property is required and cannot be null or empty ({this.GetType().Name})"));
			}
			else
			{
				foreach (var dataSource in this.DataSources)
				{
					dataSource.Validate(ref exceptions);
				}
			}

			if (exceptions.Count() == 0)
			{
				// verify relationships between sections (once other settings are valid)
				foreach (var dataConnection in this.DataConnections)
				{
					var foundMatch = this.DataSources
							.Any(w => w.Active &&
							w.DataSourceName.Equals(dataConnection.DataSourceName, StringComparison.Ordinal));
					if (!foundMatch)
					{
						exceptions.Add(
							new ConfigurationException(
								$"The {nameof(this.DataConnections)}[???].{nameof(dataConnection.DataSourceName)} " +
								$"value does not match a {nameof(this.DataSources)}[???].{nameof(dataConnection.DataSourceName)} value ({this.GetType().Name})"));
					}
				}
			}

			if (this.OpenApi == null)
			{
				exceptions.Add(
					new ConfigurationException(
						$"The {nameof(this.OpenApi)} property is required and cannot be null or empty ({this.GetType().Name})"));
			}

			if (exceptions.Count == 1)
			{
				throw exceptions.First();
			}
			else if (exceptions.Count > 1)
			{
				throw new AggregateException(exceptions);
			}
		}

		/// <inheritdoc/>
		[Obsolete("The Validate() method should NOT be used with this class")]
		public override void Validate(ref List<Exception> exceptions)
		{
		}

		/// <summary>
		/// Initialize the current object with values from the source configuration.
		/// </summary>
		/// <param name="sourceConfiguration">
		/// The source <see cref="ApplicationConfiguration"/> to copy values from.
		/// </param>
		/// <param name="suppressSensitiveValues">
		/// A flag that indicates whether sensitive values should be suppressed when copying values from the source configuration.
		/// </param>
		protected void Initialize(
			IApplicationConfiguration sourceConfiguration,
			bool suppressSensitiveValues = false)
		{
			if (sourceConfiguration == null)
			{
				throw new ArgumentException($"Invalid argument: {nameof(sourceConfiguration)}");
			}

			this.Active = sourceConfiguration.Active;
			this.DataConnections = sourceConfiguration.DataConnections
				?.Select(s => new DataConnectionConfiguration(s, suppressSensitiveValues))
				.ToList();
			this.DataSources = sourceConfiguration.DataSources
				?.Select(s => new DataSourceConfiguration(s, suppressSensitiveValues))
				.ToList();
			this.OpenApi = new OpenApiConfiguration(sourceConfiguration.OpenApi, suppressSensitiveValues);
		}
	}
}
