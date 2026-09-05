using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PilotApi.Domain.Contracts.Services;
using PilotApi.Domain.Models.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Web.Controllers.V1
{
	/// <summary>
	/// A controller for accessing and manipulating Shippers data in the data store.
	/// </summary>
	[ApiVersion("1.0")]

	public class ShippersController : SimpleControllerBase
	{
		/// <summary>
		/// Instantiate a <see cref="ShippersController"/> object.
		/// </summary>
		/// <param name="service">
		/// A service object.
		/// </param>
		public ShippersController(IShippersService service)
		{
			this.Service = service;
		}

		/// <summary>
		/// Gets the service that implements CRUD operations for the given DTO type.
		/// </summary>
		protected IShippersService Service { get; }

		/// <summary>
		/// Gets all DTO objects from the shipper table.
		/// </summary>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A read only list of all DTO objects from the shipper table, or null if no objects exist.
		/// </returns>
		[HttpGet]
		[Route("get-all")]
		[ProducesResponseType<IList<ShippersDto>>(StatusCodes.Status200OK)]
		public async Task<IActionResult?> GetAll(
			[FromQuery] int page = 0,
			[FromQuery] int pageSize = 20,
			CancellationToken cancellationToken = default)
		{
			var retrieveResponse = await this.Service.GetAllAsync(page, pageSize, cancellationToken);
			if (retrieveResponse.IsError)
			{
				this.Response.Headers["Warning"] = retrieveResponse.ErrorMessage;
				return this.BadRequest();
			}

			if (retrieveResponse.Result == null)
			{
				return this.NotFound();
			}

			return this.Ok(retrieveResponse.Result
				.ToList()
				.AsReadOnly());
		}

		/// <summary>
		/// Gets a DTO object of a shipper record by its ID.
		/// </summary>
		/// <param name="shipperId">
		/// The ID of the shipper record to retrieve.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A DTO object of the shipper record with the specified ID, or null if no such object exists.
		/// </returns>
		[HttpGet]
		[Route("get/{shipperId}")]
		[ProducesResponseType<ShippersDto>(StatusCodes.Status200OK)]
		public async Task<IActionResult?> GetById(
			[Required][FromRoute] int shipperId,
			CancellationToken cancellationToken)
		{
			var retrieveResponse = await this.Service.GetByIdAsync(new[] { shipperId }, cancellationToken);
			if (retrieveResponse.IsError)
			{
				this.Response.Headers["Warning"] = retrieveResponse.ErrorMessage;
				return this.BadRequest();
			}

			if (retrieveResponse.Result == null)
			{
				return this.NotFound();
			}

			return this.Ok(retrieveResponse.Result);
		}

		/// <summary>
		/// Adds a new shipper record.
		/// </summary>
		/// <param name="model">
		/// A DTO object of the shipper record to add.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// </returns>
		[HttpPost]
		[Route("add")]
		[ProducesResponseType<AddResponseInt>(StatusCodes.Status200OK)]
		public async Task<IActionResult> Add(
			[Required][FromBody] ShippersDto model,
			CancellationToken cancellationToken)
		{
			var retrieveResponse = await this.Service.InsertAsync<int>(model, cancellationToken);
			if (retrieveResponse.IsError)
			{
				this.Response.Headers["Warning"] = retrieveResponse.ErrorMessage;
				return this.BadRequest();
			}

			if (retrieveResponse.Result <= 0)
			{
				return this.BadRequest();
			}

			return this.CreatedAtAction(
				nameof(this.GetById), 
				new { shipperId = retrieveResponse.Result }, 
				new AddResponseInt(retrieveResponse.Result));
		}

		/// <summary>
		/// Updates an existing shipper record.
		/// </summary>
		/// <param name="model">
		/// A DTO object of the shipper record to update.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// </returns>
		[HttpPut]
		[Route("update")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Update(
			[Required][FromBody] ShippersDto model,
			CancellationToken cancellationToken)
		{
			var retrieveResponse = await this.Service.UpdateAsync(model, cancellationToken);
			if (retrieveResponse.IsError)
			{
				this.Response.Headers["Warning"] = retrieveResponse.ErrorMessage;
				return this.BadRequest();
			}

			return this.NoContent();
		}

		/// <summary>
		/// Deletes a shipper record by its ID.
		/// </summary>
		/// <param name="shipperId">
		/// An integer representing the ID of the shipper record to delete.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// </returns>
		[HttpDelete]
		[Route("delete/{shipperId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Delete(
			[Required][FromRoute] int shipperId,
			CancellationToken cancellationToken)
		{
			var retrieveResponse = await this.Service.DeleteAsync(new[] { shipperId }, cancellationToken);
			if (retrieveResponse.IsError)
			{
				this.Response.Headers["Warning"] = retrieveResponse.ErrorMessage;
				return this.BadRequest();
			}

			return this.NoContent();
		}
	}
}
