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
	public class SecurityConfiguration : ConfigurationBase, ISecurityConfiguration
	{
		/// <summary>
		/// Instatiate a <see cref="SecurityConfiguration"/> object.
		/// </summary>
		public SecurityConfiguration()
		{
			this.ClockSkewSeconds = 60;
		}

		/// <summary>
		/// Instantiate a <see cref="SecurityConfiguration"/> object.
		/// </summary>
		/// <param name="sourceConfiguration">
		/// A source configuration object to copy values from.
		/// </param>
		public SecurityConfiguration(SecurityConfiguration sourceConfiguration)
			: this()
		{
			this.Initialize(sourceConfiguration);
		}

		/// <inheritdoc/>
		[JsonProperty]
		public string? BaseUrl { get; set; }

		/// <inheritdoc/>
		[JsonProperty]
		public string? Realm { get; set; }

		/// <inheritdoc/>
		[JsonProperty]
		public string? ClientId { get; set; }

		/// <inheritdoc/>
		[JsonProperty]
		public bool RequireHttpsMetadata { get; set; }

		/// <inheritdoc/>
		[JsonProperty]
		public int ClockSkewSeconds { get; set; }

		/// <inheritdoc/>
		public string Authority => $"{this.BaseUrl?.TrimEnd('/')}/realms/{this.Realm}";

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.Active)}={this.Active}, " +
				$"{nameof(this.BaseUrl)}={this.BaseUrl}, " +
				$"{nameof(this.Realm)}={this.Realm}, " +
				$"{nameof(this.ClientId)}={this.ClientId}, " +
				$"{nameof(this.RequireHttpsMetadata)}={this.RequireHttpsMetadata}, " +
				$"{nameof(this.ClockSkewSeconds)}={this.ClockSkewSeconds}";
		}

		/// <inheritdoc/>
		public override void Validate(ref List<Exception> exceptions)
		{
			if (exceptions == null)
			{
				throw new ArgumentException($"The {nameof(exceptions)} argument is invalid ({this.GetType().Name})");
			}

			if (string.IsNullOrWhiteSpace(this.BaseUrl))
			{
				exceptions.Add(
					new ConfigurationException($"The {nameof(this.BaseUrl)} value is required and cannot be null or empty ({this.GetType().Name})"));
			}

			if (string.IsNullOrWhiteSpace(this.Realm))
			{
				exceptions.Add(
					new ConfigurationException($"The {nameof(this.Realm)} value is required and cannot be null or empty ({this.GetType().Name})"));
			}

			if (string.IsNullOrWhiteSpace(this.ClientId))
			{
				exceptions.Add(
					new ConfigurationException($"The {nameof(this.ClientId)} value is required and cannot be null or empty ({this.GetType().Name})"));
			}

			if (this.ClockSkewSeconds < 0)
			{
				exceptions.Add(
					new ConfigurationException($"The {nameof(this.ClockSkewSeconds)} value cannot be negative ({this.GetType().Name})"));
			}
		}

		/// <summary>
		/// Initialize the current object with values from the source configuration.
		/// </summary>
		/// <param name="sourceConfiguration">
		/// The source <see cref="SecurityConfiguration"/> to copy values from.
		/// </param>
		protected void Initialize(SecurityConfiguration sourceConfiguration)
		{
			if (sourceConfiguration == null)
			{
				throw new ArgumentException($"Invalid argument: {nameof(sourceConfiguration)}");
			}

			this.Active = sourceConfiguration.Active;
			this.BaseUrl = sourceConfiguration.BaseUrl;
			this.Realm = sourceConfiguration.Realm;
			this.ClientId = sourceConfiguration.ClientId;
			this.RequireHttpsMetadata = sourceConfiguration.RequireHttpsMetadata;
			this.ClockSkewSeconds = sourceConfiguration.ClockSkewSeconds;
		}
	}
}
