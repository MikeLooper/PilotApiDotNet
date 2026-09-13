using PilotApi.Shared.Configuration;
using PilotApi.Shared.Contracts.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;

namespace PilotApi.Domain.Contracts.DataSource
{
	/// <summary>
	/// Interface for the datastore context.
	/// </summary>
	public interface IDataSourceContext : IDisposable
	{
		/// <summary>
		/// Gets a data source connection string which has had sensitive information removed.
		/// </summary>
		string? ConnectionStringClean { get; }

		/// <summary>
		/// Gets a <see cref="IDataConnectionConfiguration"/> object.
		/// </summary>
		IDataConnectionConfiguration? DataConnectionConfiguration { get; }

		/// <summary>
		/// Gets a <see cref="IDataSourceConfiguration"/> object.
		/// </summary>
		IDataSourceConfiguration? DataSourceConfiguration { get; }

		/// <summary>
		/// Gets or sets a <see cref="IDbTransaction"/> object.
		/// </summary>
		IDbTransaction? DataSourceTransaction { get; set; }

		/// <summary>
		/// Commit the current transaction.
		/// </summary>
		Task CommitAsync();

		/// <summary>
		/// Build and return a <see cref="IDbConnection"/> object.
		/// Thhis method will also assign the connection object to the DatabaseConnection property.
		/// </summary>
		/// <param name="autoOpen">
		/// A flag that indicates whether the data source connection should automatically be opened when the connection is created.
		/// Default = True.
		/// </param>
		/// <returns>
		/// A <see cref="IDbConnection"/> object.
		/// </returns>
		IDbConnection GetConnection(bool autoOpen = true);

		/// <summary>
		/// Rollback the current transaction.
		/// </summary>
		Task RollbackAsync();
	}
}
