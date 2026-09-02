using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
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
	/// A controller for accessing and manipulating Customers data in the data store.
	/// </summary>
	[ApiVersion("1.0")]

	public class CustomersController : SimpleControllerBase
	{
		/// <summary>
		/// Instantiate a <see cref="CustomersController"/> object.
		/// </summary>
		/// <param name="service">
		/// A service object.
		/// </param>
		public CustomersController(ICustomersService service)
		{
			this.Service = service;
		}

		/// <summary>
		/// Gets the service that implements CRUD operations for the given DTO type.
		/// </summary>
		protected ICustomersService Service { get; }

		/// <summary>
		/// Gets all DTO objects of the customer table.
		/// </summary>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A read only list of all DTO objects from the customer table, or null if no objects exist.
		/// </returns>
		[HttpGet]
		[Route("get-all")]
		[ProducesResponseType<IList<CustomersDto>>(StatusCodes.Status200OK)]
		public async Task<IActionResult?> GetAll(
			CancellationToken cancellationToken)
		{
			var retrieveResponse = await this.Service.GetAllAsync(cancellationToken);
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
		/// Gets a customer record by its ID.
		/// </summary>
		/// <param name="customerId">
		/// The ID of the customer record to retrieve.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A DTO object of the customer record with the specified ID, or null if no such object exists.
		/// </returns>
		[HttpGet]
		[Route("get/{customerId}")]
		[ProducesResponseType<CustomersDto>(StatusCodes.Status200OK)]
		public async Task<IActionResult?> GetById(
			[Required][FromRoute] string customerId,
			CancellationToken cancellationToken)
		{
			var retrieveResponse = await this.Service.GetByIdAsync(new[] { customerId }, cancellationToken);
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
		/// Adds a new customer record.
		/// </summary>
		/// <param name="model">
		/// A DTO object of the customer record to add.
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
			[Required][FromBody] CustomersDto model,
			CancellationToken cancellationToken)
		{
			var retrieveResponse = await this.Service.InsertAsync<string>(model, cancellationToken);
			if (retrieveResponse.IsError)
			{
				this.Response.Headers["Warning"] = retrieveResponse.ErrorMessage;
				return this.BadRequest();
			}

			if (string.IsNullOrEmpty(retrieveResponse.Result))
			{
				return this.BadRequest();
			}

			return this.CreatedAtAction(
				nameof(this.GetById), 
				new { customerId = retrieveResponse.Result }, 
				new AddResponseString(retrieveResponse.Result));
		}

		/// <summary>
		/// Updates an existing customer record.
		/// </summary>
		/// <param name="model">
		/// A DTO object of the customer record to update.
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
			[Required][FromBody] CustomersDto model,
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
		/// Deletes a customer record by its ID.
		/// </summary>
		/// <param name="customerId">
		/// An integer representing the ID of the customer record to delete.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// </returns>
		[HttpDelete]
		[Route("delete/{customerId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Delete(
			[Required][FromRoute] string customerId,
			CancellationToken cancellationToken)
		{
			var retrieveResponse = await this.Service.DeleteAsync(new[] { customerId }, cancellationToken);
			if (retrieveResponse.IsError)
			{
				this.Response.Headers["Warning"] = retrieveResponse.ErrorMessage;
				return this.BadRequest();
			}

			return this.NoContent();
		}
	}
}
