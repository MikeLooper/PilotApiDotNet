using PilotApi.Shared.Contracts.Configuration;
using System.Text.Json.Serialization;

namespace PilotApi.Domain.Models.Dto
{
	/// <summary>
	/// A result for an about request.
	/// </summary>
	public class AboutResponse
	{
		/// <summary>
		/// Gets or sets the API version of the application.
		/// Default = Null.
		/// </summary>
		public string? ApiVersion { get; set; }

		/// <summary>
		/// Gets or sets the application configuration object.
		/// </summary>
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public IApplicationConfiguration? ApplicationConfiguration { get; set; }

		/// <summary>
		/// Gets or sets the build version of the application.
		/// Default = Null.
		/// </summary>
		public string? BuildVersion { get; set; }

		/// <summary>
		/// Gets or sets the deploy date of the application.
		/// Default = Null.
		/// </summary>
		public string? DeployDate { get; set; }

		/// <summary>
		/// Gets or sets the name of the application.
		/// Default = Null.
		/// </summary>
		public string? Name { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.Name)}={this.Name}, " +
				$"{nameof(this.ApiVersion)}={this.ApiVersion}, " +
				$"{nameof(this.BuildVersion)}={this.BuildVersion}, " +
				$"{nameof(this.DeployDate)}={this.DeployDate}, " +
				$"{nameof(this.ApplicationConfiguration)}=[{this.ApplicationConfiguration}]";
		}
	}
}
