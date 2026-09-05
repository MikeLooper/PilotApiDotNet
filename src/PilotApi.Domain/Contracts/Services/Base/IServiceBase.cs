using PilotApi.Domain.Contracts.Base;
using PilotApi.Domain.Models.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Domain.Contracts.Services.Base
{
	/// <summary>
	/// An interface for the base class for the service layer that provides CRUD operations for a given DTO type.
	/// </summary>
	/// <typeparam name="TDto"></typeparam>
	public interface IServiceBase<TDto> where TDto : IDtoBase
	{
		/// <summary>
		/// Deletes a DTO object of the given type by its ID.
		/// </summary>
		/// <param name="ids">
		/// A list of Id values, which should be in the same order as the key columns property.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A flag indicating whether the deletion was successful or not.
		/// </returns>
		Task<RetrieveResponse<bool>> DeleteAsync<TType>(TType[] ids, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets all DTO objects of the given type.
		/// </summary>
		/// <param name="page">
		/// The target page index. Use 0 to return all rows without paging.
		/// </param>
		/// <param name="pageSize">
		/// The number of items to return per page.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A collection of DTO objects of the given type, or null if no objects were found.
		/// </returns>
		Task<RetrieveResponse<List<TDto>>?> GetAllAsync(int page = 0, int pageSize = 20, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets a DTO object of the given type by its ID.
		/// </summary>
		/// <param name="ids">
		/// A list of Id values, which should be in the same order as the key columns property.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A DTO object of the given type, or null if no object was found with the specified ID.
		/// </returns>
		Task<RetrieveResponse<TDto>?> GetByIdAsync<TType>(TType[] ids, CancellationToken cancellationToken = default);

		/// <summary>
		/// Inserts a new DTO object of the given type into the data store.
		/// </summary>
		/// <param name="model">
		/// A DTO object of the given type to insert into the data store.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// The ID of the newly inserted DTO object, or 0 if the insertion failed.
		/// </returns>
		Task<RetrieveResponse<TReturn>?> InsertAsync<TReturn>(TDto model, CancellationToken cancellationToken = default);

		/// <summary>
		/// Executes a query against the data store and returns a collection of DTO objects of the given type that match the query criteria.
		/// </summary>
		/// <param name="query">
		/// The query string to execute against the data store.
		/// </param>
		/// <param name="parameters">
		/// An optional object containing parameters to pass to the query.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A collection of DTO objects of the given type that match the query criteria, or null if no objects were found.
		/// </returns>
		Task<RetrieveResponse<List<TDto>>?> QueryAsync(string query, object? parameters = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Executes a query against the data store and returns the first DTO object of the given type that matches the query criteria.
		/// </summary>
		/// <param name="query">
		/// The query string to execute against the data store.
		/// </param>
		/// <param name="parameters">
		/// An optional object containing parameters to pass to the query.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A DTO object of the given type that matches the query criteria, or null if no object was found.
		/// </returns>
		Task<RetrieveResponse<TDto>?> QueryFirstAsync(string query, object? parameters = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Executes a query against the data store and returns a single DTO object of the given type that matches the query criteria.
		/// </summary>
		/// <param name="query">
		/// The query string to execute against the data store.
		/// </param>
		/// <param name="parameters">
		/// An optional object containing parameters to pass to the query.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A DTO object of the given type that matches the query criteria, or null if no object was found.
		/// </returns>
		Task<RetrieveResponse<TDto>?> QuerySingleAsync(string query, object? parameters = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Updates an existing DTO object of the given type in the data store.
		/// </summary>
		/// <param name="model">
		/// A DTO object of the given type to update in the data store.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A flag indicating whether the update was successful or not.
		/// </returns>
		Task<RetrieveResponse<bool>> UpdateAsync(TDto model, CancellationToken cancellationToken = default);
	}
}
