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
	/// A controller for accessing and manipulating Products data in the data store.
	/// </summary>
	[ApiVersion("1.0")]

	public class ProductsController : SimpleControllerBase
	{
		/// <summary>
		/// Instantiate a <see cref="ProductsController"/> object.
		/// </summary>
		/// <param name="service">
		/// A service object.
		/// </param>
		public ProductsController(IProductsService service)
		{
			this.Service = service;
		}

		/// <summary>
		/// Gets the service that implements CRUD operations for the given DTO type.
		/// </summary>
		protected IProductsService Service { get; }

		/// <summary>
		/// Gets all DTO objects from the product table.
		/// </summary>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A read only list of all DTO objects from the product table, or null if no objects exist.
		/// </returns>
		[HttpGet]
		[Route("get-all")]
		[ProducesResponseType<IList<ProductsDto>>(StatusCodes.Status200OK)]
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
		/// Gets a DTO object of the product record by its ID.
		/// </summary>
		/// <param name="productId">
		/// The ID of the product record to retrieve.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A DTO object of the product record with the specified ID, or null if no such object exists.
		/// </returns>
		[HttpGet]
		[Route("get/{productId}")]
		[ProducesResponseType<ProductsDto>(StatusCodes.Status200OK)]
		public async Task<IActionResult?> GetById(
			[Required][FromRoute] int productId,
			CancellationToken cancellationToken)
		{
			var retrieveResponse = await this.Service.GetByIdAsync(new[] { productId }, cancellationToken);
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
		/// Adds a new product record.
		/// </summary>
		/// <param name="model">
		/// A DTO object of the product record to add.
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
			[Required][FromBody] ProductsDto model,
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
				new { productId = retrieveResponse.Result }, 
				new AddResponseInt(retrieveResponse.Result));
		}

		/// <summary>
		/// Updates an existing product record.
		/// </summary>
		/// <param name="model">
		/// A DTO object of the product record to update.
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
			[Required][FromBody] ProductsDto model,
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
		/// Deletes a product record by its ID.
		/// </summary>
		/// <param name="productId">
		/// An integer representing the ID of the product record to delete.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// </returns>
		[HttpDelete]
		[Route("delete/{productId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Delete(
			[Required][FromRoute] int productId,
			CancellationToken cancellationToken)
		{
			var retrieveResponse = await this.Service.DeleteAsync(new[] { productId }, cancellationToken);
			if (retrieveResponse.IsError)
			{
				this.Response.Headers["Warning"] = retrieveResponse.ErrorMessage;
				return this.BadRequest();
			}

			return this.NoContent();
		}
	}
}
