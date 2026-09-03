using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PilotApi.Web.Controllers
{
	/// <summary>
	/// A base for API controllers that implement minimal common settings.
	/// </summary>
	/// <example>
	/// Example usage:
	/// <code>
	///	[ApiVersion("1.0")]
	///	[Route("Categories")]
	///	public class CategoriesController : SimpleControllerBase
	///	{
	///		public CategoriesController(ICategoriesService service)
	///		{
	///			this.Service = service;
	///		}
	///
	///		protected ICategoriesService Service { get; }
	///
	///		[HttpGet]
	///		[Route("get/{id:int}")]
	///		[ProducesResponseType&lt;CategoriesDto&gt;(StatusCodes.Status200OK)]
	///		public async Task&lt;IActionResult?&gt; GetById(
	///			[Required][FromRoute] int id)
	///		{
	///			var result = await this.Service.GetByIdAsync(id);
	///			if (result == null)
	///			{
	///				return this.NotFound();
	///			}
	///
	///			return this.Ok(result);
	///		}
	///	}
	/// </code>
	/// </example>
	[ApiController]
	[Produces("application/json")]
	[Consumes("application/json")]
	[Route("v{version:apiVersion}/[controller]")]
	[ProducesResponseType<string>(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType<string>(StatusCodes.Status403Forbidden)]
	[ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
	public class SimpleControllerBase : Controller
	{
		/// <summary>
		/// Gets or sets the API version to apply to an operation.
		/// </summary>
		[FromHeader(Name = "ApiVersion")]
		public string? ApiVersion { get; set; }
	}
}
