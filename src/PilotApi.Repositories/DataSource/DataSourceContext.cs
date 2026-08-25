using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Npgsql;
using PilotApi.Domain.Contracts.DataSource;
using PilotApi.Shared.Configuration;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Contracts.Configuration;
using PilotApi.Shared.Exceptions;
using PilotApi.Shared.Handlers;
using PilotApi.Shared.Utilities;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PilotApi.Repositories.DataSource
{
	/// <summary>
	/// The datastore context.
	/// The connection information for this context will be read from the first active ApplicationConfiguration.DataConnections settings.
	/// If the connection object in this object is open when this object is disposed, 
	/// it will automatically be closed with a rollback of any pending activity.
	/// Commits of pending activity must be explicitly executed by the calling code.
	/// </summary>
	public class DataSourceContext : IDataSourceContext
	{
		/// <summary>
		/// Instantiate a <see cref="DataSourceContext"/> object.
		/// </summary>
		/// <param name="loggerFactory">
		/// A logger factory object.
		/// </param>
		/// <param name="applicationConfiguration">
		/// An application configuration object.
		/// </param>
		/// <exception cref="ConfigurationException">
		/// An exception related to invalid values in the configuration.
		/// </exception>
		/// <param name="sqlBuilder">
		/// A SQL builder object.
		/// </param>
		public DataSourceContext(
			ILoggerFactory loggerFactory,
			IApplicationConfiguration applicationConfiguration,
			ISqlBuilder sqlBuilder)
		{
			this.Logger = loggerFactory.CreateLogger(GetType());
			this.ApplicationConfiguration = applicationConfiguration;
			this.SqlBuilder = sqlBuilder;

			this.DataConnectionConfiguration = applicationConfiguration.DataConnections
				.First(f => f.Active);

			if (this.DataConnectionConfiguration == null)
			{
				throw new ConfigurationException(
					$"The {nameof(IApplicationConfiguration)}.{nameof(applicationConfiguration.DataConnections)} " + 
					$"section should have at least one active setting ({this.GetType().Name})");
			}

			this.DataSourceConfiguration = applicationConfiguration.GetDataSource(this.DataConnectionConfiguration.DataSourceName);
		}

		/// <inheritdoc/>
		public string? ConnectionStringClean { get; private set; }

		/// <inheritdoc/>
		public IDataConnectionConfiguration? DataConnectionConfiguration { get; }

		/// <inheritdoc/>
		public IDataSourceConfiguration? DataSourceConfiguration { get; }

		/// <inheritdoc/>
		public IDbTransaction? DataSourceTransaction { get; set; }

		/// <summary>
		/// Gets an application configuration object.
		/// </summary>
		protected IApplicationConfiguration? ApplicationConfiguration { get; }

		/// <summary>
		/// Gets a data source connection string.
		/// </summary>
		protected string? ConnectionString { get; private set; }

		/// <summary>
		/// Gets or sets a <see cref="IDbConnection"/> object.
		/// This is the storage propert for the data source connection object.
		/// </summary>
		protected IDbConnection? DataSourceConnectionInternal { get; set; }

		/// <summary>
		/// Gets the logger instance for logging information, warnings, and errors.
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Gets a SQL builder object.
		/// </summary>
		protected ISqlBuilder SqlBuilder { get; }

		/// <inheritdoc/>
		public async Task Commit()
		{
			if (this.DataSourceTransaction != null)
			{
				this.DataSourceTransaction.Commit();
			}
		}

		/// <inheritdoc/>
		public IDbConnection GetConnection(bool autoOpen = true)
		{
			if (this.DataSourceConnectionInternal == null)
			{
				this.ConnectionString = this.SqlBuilder.BuildConnectionString(this.ApplicationConfiguration.OpenApi);

				if (this.DataSourceConfiguration.DataSourceEnum == DataSourceTypes.SqlServer)
				{
					// MS SQL Server
					this.DataSourceConnectionInternal = new SqlConnection(this.ConnectionString);
				}
				else if (this.DataSourceConfiguration.DataSourceEnum == DataSourceTypes.PostgreSQL)
				{
					// PostGreSQL
					this.DataSourceConnectionInternal = new NpgsqlConnection(this.ConnectionString);
				}
				else
				{
					throw new InvalidOperationException($"Unhandled data source type: '{this.DataSourceConfiguration.DataSourceEnum}' ({this.GetType().Name})");
				}

				this.ConnectionStringClean = SecurityUtilities.ConnectionStringClean(this.ConnectionString);
			}

			if (autoOpen)
			{
				if (this.DataSourceConnectionInternal.State != ConnectionState.Open)
				{
					this.DataSourceConnectionInternal.Open();
				}
			}

			if (this.DataSourceConnectionInternal.State == ConnectionState.Open)
			{
				if (this.DataSourceTransaction == null ||
					this.DataSourceTransaction.Connection == null)
				{
					this.DataSourceTransaction = this.DataSourceConnectionInternal.BeginTransaction();
				}
			}

			return this.DataSourceConnectionInternal;
		}

		/// <inheritdoc/>
		public async Task Rollback()
		{
			if (this.DataSourceTransaction != null &&
				this.DataSourceTransaction.Connection != null)
			{
				this.DataSourceTransaction.Rollback();
			}
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.ConnectionString)}={this.ConnectionStringClean}, " +
				$"{nameof(this.DataConnectionConfiguration)}={this.DataConnectionConfiguration}, " +
				$"{nameof(this.DataSourceTransaction)}={(this.DataSourceTransaction == null ? StringConstants.LogNull : StringConstants.HasContents)}";
		}

		#region Dispose

		private bool disposed;


		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc/>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					if (this.DataSourceConnectionInternal != null)
					{
						if (this.DataSourceConnectionInternal.State == ConnectionState.Open)
						{
							this.DataSourceConnectionInternal.Close();
							this.DataSourceTransaction.Dispose();
							this.DataSourceTransaction = null;
						}

						this.DataSourceConnectionInternal.Dispose();
					}
				}
			}

			disposed = true;
		}
		
		#endregion Dispose
	}
}
