using Dapper;
using Microsoft.Extensions.Logging;
using PilotApi.Domain.Contracts.DataSource;
using PilotApi.Domain.Models.Dto;
using PilotApi.Repositories.Contracts.Repository.Base;
using PilotApi.Repositories.Handlers;
using PilotApi.Repositories.Models.Base;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Exceptions;
using PilotApi.Shared.Handlers;
using PilotApi.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Repositories.Repositories.Base
{
	/// <summary>
	/// A base class for repositories.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
	{
		/// <summary>
		/// Instantiates a new instance of the RepositoryBase class with the specified logger factory and data source context.
		/// </summary>
		/// <param name="loggerFactory">
		/// A factory for creating logger instances. This is used for logging information, warnings, and errors.
		/// </param>
		/// <param name="dataStoreContext">
		/// A context that provides access to the data source, including the data source connection and other data-related operations.
		/// </param>
		/// <param name="sqlBuilder">
		/// A SQL builder object.
		/// </param>
		/// <param name="entityUpdateHandler">
		/// An entity update handler object.
		/// </param>
		protected RepositoryBase(
			ILoggerFactory loggerFactory,
			IDataSourceContext dataStoreContext,
			ISqlBuilder sqlBuilder,
			IEntityUpdateHandler entityUpdateHandler)
		{
			this.Logger = loggerFactory.CreateLogger(GetType());
			this.DataSourceContext = dataStoreContext;
			this.SqlBuilder = sqlBuilder;
			this.EntityUpdateHandler = entityUpdateHandler;

			// default assignments
			this.KeyColumnDataTypes = new List<string>
			{
				KeyColumnDataTypeConstants.Int
			};
		}

		/// <summary>
		/// Gets a list of the column names for the current table. 
		/// The elements in this list should map one-to-one with the elements in the <see cref="EntityColumns"/> property.
		/// This property is used to construct SQL queries for inserting and updating records in the data source.
		/// For best results, do not include delimiters ([,],") on any of these items.
		/// </summary>
		protected abstract List<string> ColumnNames { get; }

		/// <summary>
		/// A flag that indicates whether new key value(s) should be created during insert.
		/// If False, the data in the supplied model will be used as-is, with the expectation that 
		/// the data provider supplied valid key values in the model.
		/// Default = True.
		/// </summary>
		protected bool CreateKey { get; set; }

		/// <summary>
		/// A flag that indicates whether this object has been validated.
		/// Default = False.
		/// </summary>
		protected bool WasValidated { get; set; }

		/// <summary>
		/// Gets the data source context, which provides access to the data source connection and other data-related operations.
		/// </summary>
		protected IDataSourceContext DataSourceContext { get; }

		/// <summary>
		/// Gets a list of the column names for the entity related to the current table.
		/// The elements in this list should map one-to-one with the elements in the <see cref="ColumnNames"/> property.
		/// This property is used to construct SQL queries for inserting and updating records in the data source.
		/// For best results, do not include delimiters ([,],") on any of these items.
		/// </summary>
		protected abstract List<string> EntityColumns { get; }

		/// <summary>
		/// Gets an entity update handler object.
		/// </summary>
		protected IEntityUpdateHandler EntityUpdateHandler { get; }

		/// <summary>
		/// Gets a list of the column names for the current table key(s).
		/// This property is used to construct SQL queries for deleting, getting, and updating records in the data source.
		/// The items in this list should directly relate to the items in the <see cref="KeyColumnDataTypes"/> property.
		/// For best results, do not include delimiters ([,],") on any of these items.
		/// </summary>
		protected abstract List<string> KeyColumnNames { get; }

		/// <summary>
		/// Gets a list of the column datatypes (as strings) for the current table key(s).
		/// The items in this list should directly relate to the items in the <see cref="KeyColumnNames"/> property.
		/// Available values can be found in the <see cref="KeyColumnDataTypeConstants"/> options.
		/// Default: <see cref="KeyColumnDataTypeConstants.Int"/>.
		/// </summary>
		protected List<string> KeyColumnDataTypes { get; set; }

		/// <summary>
		/// A flag that indicates whether the key in this table will auto-increment during insert.
		/// Default = True.
		/// </summary>
		protected bool KeyIsAutoIncrement { get; set; } = true;

		/// <summary>
		/// Gets the logger instance for logging information, warnings, and errors.
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Gets a SQL builder object.
		/// </summary>
		protected ISqlBuilder SqlBuilder { get; }

		/// <summary>
		/// Gets the name of the data source table associated with the entity.
		/// For best results, do not include delimiters ([,],") on this value.
		/// </summary>
		protected abstract string TableName { get; }

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
		public DynamicParameters BuildParameters<TType>(List<string> keyColumnNames, params TType[] ids)
		{
			if (keyColumnNames == null ||
				keyColumnNames.Count == 0)
			{
				throw new ArgumentException(
					$"The {nameof(keyColumnNames)} argument cannot be null or empty ({this.GetType().Name})");
			}

			if (keyColumnNames.Count != ids.Length)
			{
				throw new ArgumentException(
					$"The {nameof(keyColumnNames)} argument must have the same number of items as " +
					$"the {nameof(ids)} argument: {nameof(keyColumnNames)}={keyColumnNames.Count}, {nameof(ids)}={ids.Length} " +
					$"({this.GetType().Name})");
			}

			var dynamicParameters = new DynamicParameters();

			for (var keyIndex = 0; keyIndex < this.KeyColumnNames.Count; keyIndex++)
			{
				var keyColumnName = this.KeyColumnNames[keyIndex];
				var cleanColumnName = DataSourceUtilities.MinimizeName(keyColumnName);

				var columnValue = ids[keyIndex];

				dynamicParameters.Add(cleanColumnName, columnValue);
			}

			return dynamicParameters;
		}

		/// <inheritdoc/>
		public virtual async Task<RetrieveResponse<int>> CountAllAsync(CancellationToken cancellationToken = default)
		{
			this.Validate();

			// Construct the SQL query to select all records from the table.
			var querySql = this.SqlBuilder.BuildCount(this.TableName);

			// Open a data source connection.
			var connection = this.DataSourceContext.GetConnection();

			int result = -1;
			try
			{
				// Execute the query asynchronously and retrieve the results.
				var command = new CommandDefinition(
					querySql,
					transaction: this.DataSourceContext.DataSourceTransaction,
					cancellationToken: cancellationToken);
				result = await connection.ExecuteAsync(command);
			}
			catch (Exception exc)
			{
				this.Logger.LogError(exc, nameof(this.GetAllAsync));
				await this.DataSourceContext.Rollback();

				throw;
			}
			finally
			{
				// data source cleanup
				connection.Close();
			}

			// Return the retrieved count.
			return new RetrieveResponse<int>(result);
		}

		/// <inheritdoc/>
		public virtual async Task<RetrieveResponse<bool>> DeleteAsync<TType>(TType[] ids, CancellationToken cancellationToken = default)
		{
			this.Validate();

			if (ids.Length < 1)
			{
				throw new ArgumentException($"The supplied ids list ({ids} must contain at least one item ({this.GetType().Name})");
			}

			// Construct the SQL query to delete the record from the table.
			var querySql = this.SqlBuilder.BuildDelete(
				this.TableName,
				this.KeyColumnNames);
			var queryParameters = this.BuildParameters(this.KeyColumnNames, ids);

			// Open a data source connection.
			var connection = this.DataSourceContext.GetConnection();

			string? errorMessage = null;
			var success = false;
			try
			{
				// Execute the query asynchronously and retrieve the result.
				var command = new CommandDefinition(
					querySql,
					queryParameters,
					transaction: this.DataSourceContext.DataSourceTransaction,
					cancellationToken: cancellationToken);
				var result = await connection.ExecuteAsync(command);
				success = result > 0;
				if (success)
				{
					await this.DataSourceContext.Commit();
				}
				else
				{
					await this.DataSourceContext.Rollback();
					errorMessage = $"Zero rows were deleted: {querySql}";
				}
			}
			catch (Exception exc)
			{
				this.Logger.LogError(exc, nameof(this.DeleteAsync));
				await this.DataSourceContext.Rollback();

				throw;
			}
			finally
			{
				// data source cleanup
				connection.Close();
			}

			// Return true if at least one record was affected; otherwise, return false.
			return new RetrieveResponse<bool>(success, errorMessage);
		}

		/// <inheritdoc/>
		public virtual async Task<RetrieveResponse<List<TEntity>>?> GetAllAsync(CancellationToken cancellationToken = default)
		{
			this.Validate();

			// Construct the SQL query to select all records from the table.
			var querySql = this.SqlBuilder.BuildSelect(
				this.TableName,
				this.ColumnNames);

			// Open a data source connection.
			var connection = this.DataSourceContext.GetConnection();

			IEnumerable<TEntity>? result = null;
			try
			{
				// Execute the query asynchronously and retrieve the results.
				var command = new CommandDefinition(
					querySql,
					transaction: this.DataSourceContext.DataSourceTransaction,
					cancellationToken: cancellationToken);
				result = await connection.QueryAsync<TEntity>(command);
			}
			catch (Exception exc)
			{
				this.Logger.LogError(exc, nameof(this.GetAllAsync));
				await this.DataSourceContext.Rollback();

				throw;
			}
			finally
			{
				// data source cleanup
				connection.Close();
			}

			// Return the retrieved entities.
			return new RetrieveResponse<List<TEntity>>(result?.ToList());
		}

		/// <inheritdoc/>
		public virtual async Task<RetrieveResponse<TEntity>?> GetAsync<TType>(TType[] ids, CancellationToken cancellationToken = default)
		{
			this.Validate();

			if (ids.Length < 1)
			{
				throw new ArgumentException($"The supplied ids list ({ids} must contain at least one item ({this.GetType().Name})");
			}

			// Construct the SQL query to select a record by its ID from the table.
			var querySql = this.SqlBuilder.BuildSelect(
				this.TableName,
				this.ColumnNames,
				this.KeyColumnNames);
			var queryParameters = this.BuildParameters(this.KeyColumnNames, ids);

			// Open a data source connection.
			var connection = this.DataSourceContext.GetConnection();

			TEntity? result = null;
			try
			{
				// Execute the query asynchronously and retrieve the result.
				var command = new CommandDefinition(
					querySql,
					queryParameters,
					transaction: this.DataSourceContext.DataSourceTransaction,
					cancellationToken: cancellationToken);
				result = await connection.QuerySingleOrDefaultAsync<TEntity>(command);

			}
			catch (Exception exc)
			{
				this.Logger.LogError(exc, nameof(this.GetAsync));
				await this.DataSourceContext.Rollback();

				throw;
			}
			finally
			{
				// data source cleanup
				connection.Close();
			}

			// Return the retrieved entity (or null if not found).
			return new RetrieveResponse<TEntity>(result);
		}

		/// <inheritdoc/>
		public virtual async Task<RetrieveResponse<TReturn>?> InsertAsync<TReturn>(TEntity model, CancellationToken cancellationToken = default)
		{
			this.Validate();

			if (model == null)
			{
				throw new ArgumentException($"Invalid argument: {nameof(model)} ({this.GetType().Name})");
			}

			// Construct the SQL query to insert a new record into the table.
			var querySql = this.SqlBuilder.BuildInsert(
				this.TableName,
				this.ColumnNames,
				this.KeyColumnNames,
				this.EntityColumns,
				this.KeyIsAutoIncrement);

			// get alternate Ids for certain data source type/table/key-column conditions
			var nextIds = new Dictionary<string, object>();
			if (this.CreateKey)
			{
				nextIds = await this.GetNextIdsAsync(cancellationToken);
				if (nextIds != null &&
					nextIds.Keys.Count > 0)
				{
					// if nextIds are present, assign them to the related column(s) on the current model
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
					model = this.EntityUpdateHandler.Update(model, nextIds);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
				}
			}

			// Open a data source connection.
			var connection = this.DataSourceContext.GetConnection();

			TReturn? id = default;
			try
			{
				// Execute the query asynchronously and retrieve the inserted ID.
				var command = new CommandDefinition(
					querySql,
					model,
					transaction: this.DataSourceContext.DataSourceTransaction,
					cancellationToken: cancellationToken);
				id = await connection.QueryFirstOrDefaultAsync<TReturn>(command);

				var success = false;
				if (id is int)
				{
					var changedInt = Convert.ToInt32(id);
					success = changedInt > 0;
				}
				else if (id is string)
				{
					if (nextIds.Count > 0)
					{
						var newIds = nextIds.Select(x => x.Value.ToString()).ToList();
						id = (TReturn)Convert.ChangeType(string.Join(",", newIds), typeof(TReturn));
					}

					var changedString = id?.ToString();
					success = !string.IsNullOrEmpty(changedString);
				}
				else
				{
					throw new InvalidOperationException(
								$"The generic datatype ('{typeof(TReturn).Name}') was not handled: Method='{nameof(this.InsertAsync)}' " + 
								$"({this.GetType().Name})");

				}

				if (success)
				{
					await this.DataSourceContext.Commit();
				}
				else
				{
					await this.DataSourceContext.Rollback();
				}
			}
			catch (Exception exc)
			{
				this.Logger.LogError(exc, nameof(this.InsertAsync));
				await this.DataSourceContext.Rollback();

				throw;
			}
			finally
			{
				// data source cleanup
				connection.Close();
			}

			// Return the inserted ID.
			return new RetrieveResponse<TReturn>(id);
		}

		/// <inheritdoc/>
		public virtual async Task<RetrieveResponse<IEnumerable<TEntity>>?> QueryAsync(string querySql, object? parameters = null, CancellationToken cancellationToken = default)
		{
			this.Validate();

			if (string.IsNullOrWhiteSpace(querySql))
			{
				throw new ArgumentException($"Invalid argument: {nameof(querySql)} ({this.GetType().Name})");
			}

			// Open a data source connection.
			var connection = this.DataSourceContext.GetConnection();

			List<TEntity>? result = null;
			try
			{
				// Execute the query asynchronously
				var command = new CommandDefinition(
					querySql,
					parameters,
					transaction: this.DataSourceContext.DataSourceTransaction,
					cancellationToken: cancellationToken);
				var taskResult = await connection.QueryAsync<TEntity>(command);
				result = taskResult.ToList();
			}
			catch (Exception exc)
			{
				this.Logger.LogError(exc, nameof(this.QueryAsync));
				await this.DataSourceContext.Rollback();

				throw;
			}
			finally
			{
				// data source cleanup
				connection.Close();
			}

			return new RetrieveResponse<IEnumerable<TEntity>>(result);
		}

		/// <inheritdoc/>
		public virtual async Task<RetrieveResponse<TEntity>?> QueryFirstAsync(string querySql, object? parameters = null, CancellationToken cancellationToken = default)
		{
			this.Validate();

			if (string.IsNullOrWhiteSpace(querySql))
			{
				throw new ArgumentException($"Invalid argument: {nameof(querySql)} ({this.GetType().Name})");
			}

			// Open a data source connection.
			var connection = this.DataSourceContext.GetConnection();

			TEntity? result = null;
			try
			{
				// Execute the query asynchronously
				var command = new CommandDefinition(
					querySql,
					parameters,
					transaction: this.DataSourceContext.DataSourceTransaction,
					cancellationToken: cancellationToken);
				result = await connection.QueryFirstAsync<TEntity>(command);
			}
			catch (Exception exc)
			{
				this.Logger.LogError(exc, nameof(this.QueryFirstAsync));
				await this.DataSourceContext.Rollback();

				throw;
			}
			finally
			{
				// data source cleanup
				connection.Close();
			}

			return new RetrieveResponse<TEntity>(result);
		}

		/// <inheritdoc/>
		public virtual async Task<RetrieveResponse<TMethodType>?> QuerySingleAsync<TMethodType>(string querySql, object? parameters = null, CancellationToken cancellationToken = default)
		{
			this.Validate();

			if (string.IsNullOrWhiteSpace(querySql))
			{
				throw new ArgumentException($"Invalid argument: {nameof(querySql)} ({this.GetType().Name})");
			}

			// Open a data source connection.
			var connection = this.DataSourceContext.GetConnection();

			TMethodType? result = default;
			try
			{
				// Execute the query asynchronously
				var command = new CommandDefinition(
					querySql,
					parameters,
					transaction: this.DataSourceContext.DataSourceTransaction,
					cancellationToken: cancellationToken);
				result = await connection.QuerySingleAsync<TMethodType>(command);
			}
			catch (Exception exc)
			{
				this.Logger.LogError(exc, $"{nameof(this.QuerySingleAsync)}-TMethodType");
				await this.DataSourceContext.Rollback();

				throw;
			}
			finally
			{
				// data source cleanup
				connection.Close();
			}

			return new RetrieveResponse<TMethodType>(result);
		}

		/// <inheritdoc/>
		public virtual async Task<RetrieveResponse<TEntity>?> QuerySingleAsync(string querySql, object? parameters = null, CancellationToken cancellationToken = default)
		{
			this.Validate();

			if (string.IsNullOrWhiteSpace(querySql))
			{
				throw new ArgumentException($"Invalid argument: {nameof(querySql)} ({this.GetType().Name})");
			}

			// Open a data source connection.
			var connection = this.DataSourceContext.GetConnection();

			TEntity? result = null;
			try
			{
				// Execute the query asynchronously
				var command = new CommandDefinition(
					querySql,
					parameters,
					transaction: this.DataSourceContext.DataSourceTransaction,
					cancellationToken: cancellationToken);
				result = await connection.QuerySingleAsync<TEntity>(command);
			}
			catch (Exception exc)
			{
				this.Logger.LogError(exc, $"{nameof(this.QuerySingleAsync)}-TEntity");
				await this.DataSourceContext.Rollback();

				throw;
			}
			finally
			{
				// data source cleanup
				connection.Close();
			}

			return new RetrieveResponse<TEntity>(result);
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.ColumnNames)}={this.ColumnNames}, " +
				$"{nameof(this.KeyColumnNames)}={this.KeyColumnNames}, " +
				$"{nameof(this.DataSourceContext)}={this.DataSourceContext}";
		}

		/// <inheritdoc/>
		public virtual async Task<RetrieveResponse<bool>> UpdateAsync(TEntity model, CancellationToken cancellationToken = default)
		{
			this.Validate();

			if (model == null)
			{
				throw new ArgumentException($"Invalid argument: {nameof(model)} ({this.GetType().Name})");
			}

			// Construct the SQL query to update the record in the table.
			var querySql = this.SqlBuilder.BuildUpdate(
				this.TableName,
				this.ColumnNames,
				this.KeyColumnNames,
				this.EntityColumns,
				this.KeyIsAutoIncrement);

			// Open a data source connection.
			var connection = this.DataSourceContext.GetConnection();

			string? errorMessage = null;
			var success = false;
			try
			{
				// Execute the query asynchronously and retrieve the result.
				var command = new CommandDefinition(
					querySql,
					model,
					transaction: this.DataSourceContext.DataSourceTransaction,
					cancellationToken: cancellationToken);
				var result = await connection.ExecuteAsync(command);

				success = result > 0;
				if (success)
				{
					await this.DataSourceContext.Commit();
				}
				else
				{
					await this.DataSourceContext.Rollback();
					errorMessage = $"Zero rows were updated: {querySql}";
				}
			}
			catch (Exception exc)
			{
				this.Logger.LogError(exc, nameof(this.UpdateAsync));
				await this.DataSourceContext.Rollback();

				throw;
			}
			finally
			{
				// data source cleanup
				connection.Close();
			}

			// Return true if at least one record was affected; otherwise, return false.
			return new RetrieveResponse<bool>(success, errorMessage);
		}

		/// <summary>
		/// Get the next id values for the current table and key columns;
		/// </summary>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// A dictionary of: key column name and next id value.
		/// </returns>
		protected virtual async Task<Dictionary<string, object>> GetNextIdsAsync(CancellationToken cancellationToken = default)
		{
			var returnDict = new Dictionary<string, object>();

			if (this.DataSourceContext.DataSourceConfiguration.DataSourceEnum == DataSourceTypes.PostgreSQL)
			{
				// Open a data source connection.
				var connection = this.DataSourceContext.GetConnection();

				try
				{
					for (var keyIndex = 0; keyIndex < this.KeyColumnNames.Count; keyIndex++)
					{
						var keyColumnName = this.KeyColumnNames[keyIndex];
						var keyDataType = this.KeyColumnDataTypes[keyIndex];
						var querySql = this.SqlBuilder.BuildGetNextId(this.TableName, keyColumnName, keyDataType);

						switch (keyDataType)
						{
							case KeyColumnDataTypeConstants.Int:
								var queryResponseInt = await this.QuerySingleAsync<int>(querySql, cancellationToken: cancellationToken);
								if (queryResponseInt.Result > 0)
								{
									returnDict.Add(keyColumnName, queryResponseInt.Result);
								}
								else
								{
									throw new InvalidOperationException(
										$"The {nameof(QuerySingleAsync)} method returned an id value that was invalid: '{queryResponseInt.Result}' " + 
										$"({this.GetType().Name})");
								}

								break;
							case KeyColumnDataTypeConstants.String:
								var queryResponseString = await this.QuerySingleAsync<string>(querySql, cancellationToken: cancellationToken);
								if (!string.IsNullOrWhiteSpace(queryResponseString.Result))
								{
									returnDict.Add(keyColumnName, queryResponseString.Result);
								}
								else
								{
									throw new InvalidOperationException(
										$"The {nameof(QuerySingleAsync)} method returned an id value that was invalid: '{queryResponseString.Result}' " + 
										$"({this.GetType().Name})");
								}

								break;
							default:
								throw new InvalidOperationException(
									$"The key column datatype ('{keyDataType}') was not handled: Method='{nameof(this.GetNextIdsAsync)}' " + 
									$"({this.GetType().Name})");
						}
					}
				}
				catch (Exception exc)
				{
					this.Logger.LogError(exc, nameof(this.GetNextIdsAsync));
					await this.DataSourceContext.Rollback();

					throw;
				}
				finally
				{
					// data source cleanup
					connection.Close();
				}
			}

			return returnDict;
		}

		/// <summary>
		/// Validate the properties on this object.
		/// </summary>
		protected void Validate()
		{
			if (this.WasValidated)
			{
				return;
			}

			if (this.ColumnNames == null ||
				this.ColumnNames.Count == 0)
			{
				throw new ConfigurationException(
					$"The {nameof(this.ColumnNames)} property is required and cannot be null or empty ({this.GetType().Name})");
			}

			if (this.EntityColumns == null ||
				this.EntityColumns.Count == 0)
			{
				throw new ConfigurationException(
					$"The {nameof(this.EntityColumns)} property is required and cannot be null or empty ({this.GetType().Name})");
			}
			else
			{
				var columnCount = this.ColumnNames == null ? 0 : this.ColumnNames.Count;
				var entityCount = this.EntityColumns.Count;
				if (columnCount != entityCount)
				{ 
					throw new ConfigurationException(
						$"The number of items in the {nameof(this.ColumnNames)} and {nameof(this.EntityColumns)} properties " + 
						$"must be identical: {nameof(this.ColumnNames)}={columnCount} {nameof(this.EntityColumns)}={entityCount} ({this.GetType().Name})");
				}
			}

			if (this.KeyColumnNames == null ||
				this.KeyColumnNames.Count == 0)
			{
				throw new ConfigurationException(
					$"The {nameof(this.KeyColumnNames)} property is required and cannot be null or empty ({this.GetType().Name})");
			}

			if (this.KeyColumnDataTypes == null ||
				this.KeyColumnDataTypes.Count == 0)
			{
				throw new ConfigurationException(
					$"The {nameof(this.KeyColumnDataTypes)} property is required and cannot be null or empty ({this.GetType().Name})");
			}
			else
			{
				// validate types
				foreach (var dataType in this.KeyColumnDataTypes)
				{
					if (!KeyColumnDataTypeConstants.AvailableOptions.Contains(dataType))
					{
						throw new ConfigurationException(
							$"An item in the {nameof(this.KeyColumnDataTypes)} property is not valid.  " +
							$"Valid values include the following: '{string.Join("', '", KeyColumnDataTypeConstants.AvailableOptions)}' " +
							$"({this.GetType().Name})");
					}
				}

				// validate item counts
				var columnCount = this.KeyColumnNames == null ? 0 : this.KeyColumnNames.Count;
				var dataTypeCount = this.KeyColumnDataTypes.Count;
				if (columnCount != dataTypeCount)
				{ 
					throw new ConfigurationException(
						$"The number of items in the {nameof(this.KeyColumnNames)} and {nameof(this.KeyColumnDataTypes)} properties " + 
						$"must be identical: {nameof(this.KeyColumnNames)}={columnCount} {nameof(this.KeyColumnDataTypes)}={dataTypeCount} " + 
						$"({this.GetType().Name})");
				}
			}

			if (string.IsNullOrWhiteSpace(this.TableName))
			{
				throw new ConfigurationException(
					$"The {nameof(this.TableName)} property is required and cannot be null or empty ({this.GetType().Name})");
			}

			this.WasValidated = true;
		}
	}
}
