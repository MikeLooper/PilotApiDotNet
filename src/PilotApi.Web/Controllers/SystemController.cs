using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PilotApi.Domain.Models.Dto;
using PilotApi.Shared.Configuration;
using PilotApi.Shared.Contracts.Configuration;
using PilotApi.Shared.Utilities;
using System;
using System.Threading.Tasks;

namespace PilotApi.Web.Controllers
{
	/// <summary>
	/// A controller for system processing.
	/// </summary>
	[ApiVersionNeutral]
	[AllowAnonymous]

	public class SystemController : SimpleControllerBase
	{
		/// <summary>
		/// Instantiate a <see cref="SystemController"/> object.
		/// </summary>
		/// <param name="applicationConfiguration">
		/// A configuration object.
		/// </param>
		public SystemController(IApplicationConfiguration applicationConfiguration)
		{
			this.ApplicationConfiguration = applicationConfiguration;
		}

		/// <summary>
		/// Gets the application configuration object.
		/// </summary>
		protected IApplicationConfiguration ApplicationConfiguration { get; }

		/// <summary>
		/// Return an OK.
		/// </summary>
		/// <returns>
		/// A read only list of all DTO objects from the category table, or null if no objects exist.
		/// </returns>
		[HttpGet]
		[Route("healthcheck")]
		[ProducesResponseType<string>(StatusCodes.Status200OK)]
		public async Task<IActionResult?> GetAll()
		{
			return this.Ok("OK");
		}

		/// <summary>
		/// Returns application metadata and optional configuration details.
		/// </summary>
		/// <param name="showDetails">
		/// A boolean value indicating whether configuration details should be included.
		/// </param>
		/// <returns>
		/// A response containing application metadata.
		/// </returns>
		[HttpGet]
		[Route("about")]
		[ProducesResponseType<AboutResponse>(StatusCodes.Status200OK)]
		public IActionResult About(
			[FromQuery(Name = "show-details")] bool showDetails = false)
		{
			var name = this.ApplicationConfiguration.OpenApi.Title;
			var appVersion = this.ApplicationConfiguration.OpenApi.Version;
			var buildVersion = FileUtilities.GetApplicationVersion();
			var deployDate = Environment.GetEnvironmentVariable("APP_DEPLOY_DATE");

			var aboutResponse  = new AboutResponse
			{
				Name = name,
				ApiVersion = appVersion,
				BuildVersion = buildVersion,
				DeployDate = deployDate,
				ApplicationConfiguration = showDetails ? new ApplicationConfiguration(this.ApplicationConfiguration, true) : null
			};

			return this.Ok(aboutResponse);
		}
	}
}
