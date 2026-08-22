using Dapper;
using PilotApi.Domain.Models.Dto;
using PilotApi.Repositories.Models.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Repositories.Contracts.Repository.Base
{
	/// <summary>
	/// An interface for the repository base.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public interface IRepositoryBase<TEntity> where TEntity : EntityBase
	{
		/// <summary>
		/// Build and return a parameter object that would be supplied to a Dapper command, based upon the key columns property.
		/// </summary>
		/// <param name="keyColumnNames">
		/// A list of the column names for the source entity key(s).
		/// </param>
		/// <param name="ids">
		/// A list of Id values, which should be in the same order as the key columns property.
		/// </param>
		/// <returns>
		/// A <see cref="DynamicParameters"/> object.
		/// </returns>
		DynamicParameters BuildParameters<TType>(List<string> keyColumnNames, params TType[] ids);

		/// <summary>
		/// Return a count of all the records from the data source table.
		/// </summary>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// The number of records in the data source table.
		/// </returns>
		Task<RetrieveResponse<int>> CountAllAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Deletes a record from the data source table based on the specified ID.
		/// </summary>
		/// <param name="ids">
		/// A list of Id values, which should be in the same order as the key columns property.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A boolean value indicating whether the deletion was successful.
		/// Returns true if at least one record was affected (deleted); otherwise, returns false.
		/// </returns>
		Task<RetrieveResponse<bool>> DeleteAsync<TType>(TType[] ids, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets all records from the data source table.
		/// </summary>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// An List&lt;T&gt; containing all records from the data source table.
		/// </returns>
		Task<RetrieveResponse<List<TEntity>>?> GetAllAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets a record from the data source table based on the specified ID.
		/// </summary>
		/// <param name="ids">
		/// A list of Id values, which should be in the same order as the key columns property.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A Task&lt;T?&gt; representing the record retrieved from the data source table.
		/// </returns>
		Task<RetrieveResponse<TEntity>?> GetAsync<TType>(TType[] ids, CancellationToken cancellationToken = default);

		/// <summary>
		/// Inserts a new record into the data source table based on the provided model.
		/// </summary>
		/// <param name="model">
		/// An entity representing the new record to be inserted into the data source table.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// The ID of the newly inserted record in the data source table.
		/// </returns>
		Task<RetrieveResponse<TReturn>?> InsertAsync<TReturn>(TEntity model, CancellationToken cancellationToken = default);

		/// <summary>
		/// Executes a custom SQL query against the data source and returns the results as an IEnumerable&lt;T&gt;.
		/// </summary>
		/// <param name="querySql">
		/// A string representing the custom SQL query to be executed against the data source.
		/// </param>
		/// <param name="parameters">
		/// An optional object containing parameters to be used in the SQL query. This can be null if no parameters are needed.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A list of entities retrieved from the data source table.
		/// </returns>
		Task<RetrieveResponse<IEnumerable<TEntity>>?> QueryAsync(string querySql, object? parameters = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Executes a custom SQL query against the data source and returns the first result as a single instance of TEntity.
		/// </summary>
		/// <param name="querySql">
		/// A string representing the custom SQL query to be executed against the data source.
		/// </param>
		/// <param name="parameters">
		/// An optional object containing parameters to be used in the SQL query. This can be null if no parameters are needed.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// An entity retrieved from the data source table.
		/// </returns>
		Task<RetrieveResponse<TEntity>?> QueryFirstAsync(string querySql, object? parameters = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Executes a custom SQL query against the data source and returns a single result as an instance of TMethodType.
		/// </summary>
		/// <param name="querySql">
		/// A string representing the custom SQL query to be executed against the data source.
		/// </param>
		/// <param name="parameters">
		/// An optional object containing parameters to be used in the SQL query. This can be null if no parameters are needed.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A value retrieved from the data source table.
		/// </returns>
		Task<RetrieveResponse<TMethodType>?> QuerySingleAsync<TMethodType>(string querySql, object? parameters = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Executes a custom SQL query against the data source and returns a single result as an instance of T.
		/// </summary>
		/// <param name="querySql">
		/// A string representing the custom SQL query to be executed against the data source.
		/// </param>
		/// <param name="parameters">
		/// An optional object containing parameters to be used in the SQL query. This can be null if no parameters are needed.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// An entity retrieved from the data source table.
		/// </returns>
		Task<RetrieveResponse<TEntity>?> QuerySingleAsync(string querySql, object? parameters = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Updates an existing record in the data source table based on the provided model.
		/// </summary>
		/// <param name="model">
		/// An entity of the updated record to be saved in the data source table.
		/// </param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A boolean value indicating whether the update was successful.
		/// </returns>
		Task<RetrieveResponse<bool>> UpdateAsync(TEntity model, CancellationToken cancellationToken = default);
	}
}
