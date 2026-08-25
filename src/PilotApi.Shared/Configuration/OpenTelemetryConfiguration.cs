using Newtonsoft.Json;
using PilotApi.Shared.Configuration.Base;
using PilotApi.Shared.Contracts.Configuration;
using PilotApi.Shared.Exceptions;
using System;
using System.Collections.Generic;

namespace PilotApi.Shared.Configuration
{
	/// <inheritdoc/>
	[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
	public class OpenTelemetryConfiguration : ConfigurationBase, IOpenTelemetryConfiguration
	{
		/// <summary>
		/// Instatiate a <see cref="OpenTelemetryConfiguration"/> object.
		/// </summary>
		public OpenTelemetryConfiguration()
		{
		}

		/// <summary>
		/// Instantiate a <see cref="OpenTelemetryConfiguration"/> object.
		/// </summary>
		/// <param name="sourceConfiguration">
		/// A source configuration object to copy values from.
		/// </param>
		public OpenTelemetryConfiguration(OpenTelemetryConfiguration sourceConfiguration)
			: this()
		{
			this.Initialize(sourceConfiguration);
		}

		/// <inheritdoc/>
		[JsonProperty]
		public int? Port { get; set; }

		/// <inheritdoc/>
		[JsonProperty]
		public string? Server { get; set; }
		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.Active)}={this.Active}, " +
				$"{nameof(this.Server)}={this.Server}, " +
				$"{nameof(this.Port)}={this.Port}";

		}

		/// <inheritdoc/>
		public override void Validate(ref List<Exception> exceptions)
		{
			if (exceptions == null)
			{
				throw new ArgumentException($"The {nameof(exceptions)} argument is invalid ({this.GetType().Name})");
			}

			if (string.IsNullOrWhiteSpace(this.Server))
			{
				exceptions.Add(
					new ConfigurationException($"The {nameof(this.Server)} value is required and cannot be null or empty ({this.GetType().Name})"));
			}

			if (this.Port <= 0)
			{
				exceptions.Add(
					new ConfigurationException($"The {nameof(this.Port)} value is required and must be greater than zero ({this.GetType().Name})"));
			}
		}

		/// <summary>
		/// Initialize the current object with values from the source configuration.
		/// </summary>
		/// <param name="sourceConfiguration">
		/// The source <see cref="OpenTelemetryConfiguration"/> to copy values from.
		/// </param>
		protected void Initialize(OpenTelemetryConfiguration sourceConfiguration)
		{
			if (sourceConfiguration == null)
			{
				throw new ArgumentException($"Invalid argument: {nameof(sourceConfiguration)}");
			}

			this.Active = sourceConfiguration.Active;
			this.Server = sourceConfiguration.Server;
			this.Port = sourceConfiguration.Port;
		}
	}
}
