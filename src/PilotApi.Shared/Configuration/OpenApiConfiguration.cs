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
	public class OpenApiConfiguration : ConfigurationBase, IOpenApiConfiguration
	{
		/// <summary>
		/// Instatiate a <see cref="OpenApiConfiguration"/> object.
		/// </summary>
		public OpenApiConfiguration()
		{
		}

		/// <summary>
		/// Instantiate a <see cref="OpenApiConfiguration"/> object.
		/// </summary>
		/// <param name="sourceConfiguration">
		/// A source configuration object to copy values from.
		/// </param>
		public OpenApiConfiguration(OpenApiConfiguration sourceConfiguration)
			: this()
		{
			this.Initialize(sourceConfiguration);
		}

		/// <inheritdoc/>
		[JsonProperty]
		public OpenApiContactConfiguration? Contact { get; set; }

		/// <inheritdoc/>
		[JsonProperty]
		public string? Description { get; set; }

		/// <inheritdoc/>
		[JsonProperty]
		public string? License { get; set; }

		/// <inheritdoc/>
		[JsonProperty]
		public string? Summary { get; set; }

		/// <inheritdoc/>
		[JsonProperty]
		public string? Title { get; set; }

		/// <inheritdoc/>
		[JsonProperty]
		public string? Version { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.Active)}={this.Active}, " +
				$"{nameof(this.Contact)}={this.Contact}, " +
				$"{nameof(this.Description)}={this.Description}, " +
				$"{nameof(this.License)}={this.License}, " +
				$"{nameof(this.Summary)}={this.Summary}, " +
				$"{nameof(this.Title)}={this.Title}, " +
				$"{nameof(this.Version)}={this.Version}";
		}

		/// <inheritdoc/>
		public override void Validate(ref List<Exception> exceptions)
		{
			if (exceptions == null)
			{
				throw new ArgumentException($"The {nameof(exceptions)} argument is invalid ({this.GetType().Name})");
			}

			if (this.Contact == null)
			{
				exceptions.Add(
					new ConfigurationException($"The {nameof(this.Contact)} value is required and cannot be null ({this.GetType().Name})"));
			}
			else
			{
				this.Contact.Validate(ref exceptions);
			}

			if (string.IsNullOrWhiteSpace(this.Description))
			{
				exceptions.Add(
					new ConfigurationException($"The {nameof(this.Description)} value is required and cannot be null or empty ({this.GetType().Name})"));
			}

			if (string.IsNullOrWhiteSpace(this.License))
			{
				exceptions.Add(
					new ConfigurationException($"The {nameof(this.License)} value is required and cannot be null or empty ({this.GetType().Name})"));
			}

			if (string.IsNullOrWhiteSpace(this.Summary))
			{
				exceptions.Add(
					new ConfigurationException($"The {nameof(this.Summary)} value is required and cannot be null or empty ({this.GetType().Name})"));
			}

			if (string.IsNullOrWhiteSpace(this.Title))
			{
				exceptions.Add(
					new ConfigurationException($"The {nameof(this.Title)} value is required and cannot be null or empty ({this.GetType().Name})"));
			}

			if (string.IsNullOrWhiteSpace(this.Version))
			{
				exceptions.Add(
					new ConfigurationException($"The {nameof(this.Version)} value is required and cannot be null or empty ({this.GetType().Name})"));
			}
		}

		/// <summary>
		/// Initialize the current object with values from the source configuration.
		/// </summary>
		/// <param name="sourceConfiguration">
		/// The source <see cref="OpenApiConfiguration"/> to copy values from.
		/// </param>
		protected void Initialize(OpenApiConfiguration sourceConfiguration)
		{
			if (sourceConfiguration == null)
			{
				throw new ArgumentException($"Invalid argument: {nameof(sourceConfiguration)}");
			}

			this.Active = sourceConfiguration.Active;
			this.Contact = sourceConfiguration.Contact;
			this.Description = sourceConfiguration.Description;
			this.License = sourceConfiguration.License;
			this.Summary = sourceConfiguration.Summary;
			this.Title = sourceConfiguration.Title;
			this.Version = sourceConfiguration.Version;
		}
	}
}
