using Microsoft.Extensions.Logging;
using PilotApi.Shared.Configuration;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Contracts.Configuration;
using PilotApi.Shared.Exceptions;
using PilotApi.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PilotApi.Shared.Handlers
{
	/// <summary>
	/// A handler for SQL building operations.
	/// </summary>
	public class SqlBuilder : ISqlBuilder
	{
		/// <summary>
		/// Instantiate a <see cref="SqlBuilder"/> object.
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
		public SqlBuilder(
			ILoggerFactory loggerFactory,
			IApplicationConfiguration applicationConfiguration)
		{
			this.Logger = loggerFactory.CreateLogger(GetType());

			this.Initialize(applicationConfiguration);
		}

		/// <summary>
		/// Gets a <see cref="IDataConnectionConfiguration"/> object.
		/// </summary>
		protected IDataConnectionConfiguration? DataConnectionConfiguration { get; private set; }

		/// <summary>
		/// Gets a <see cref="IDataSourceConfiguration"/> object.
		/// </summary>
		protected IDataSourceConfiguration? DataSourceConfiguration { get; private set; }

		/// <summary>
		/// Gets a flag that indicates whether the current data source is PostgreSQL.
		/// </summary>
		protected bool IsPostgreSQL { get; private set; }

		/// <summary>
		/// Gets a flag that indicates whether the current data source is SQL Server.
		/// </summary>
		protected bool IsSqlServer { get; private set; }

		/// <summary>
		/// Gets the logger instance for logging information, warnings, and errors.
		/// </summary>
		protected ILogger Logger { get; }

		/// <inheritdoc/>
		public string? BuildConnectionString(OpenApiConfiguration? openApi)
		{
			if (openApi == null)
			{
				throw new ArgumentException(
					$"The {nameof(openApi)} argument cannot be null or empty ({this.GetType().Name})");
			}

			this.Logger.LogInformation($"1 - Building connection string for OpenApi: {openApi.Title}");
			this.Logger.LogInformation($"2 - Building connection string for OpenApi: {openApi.Title}");

			string? connectionString = null;
			if (this.IsSqlServer)
			{
				var dataSource = this.DataConnectionConfiguration.Host;
				if (this.DataConnectionConfiguration.Port != null)
				{
					dataSource += "," + this.DataConnectionConfiguration.Port.ToString();
				}

				connectionString = $"Data Source={dataSource};Initial Catalog={this.DataSourceConfiguration.DataSource};" +
					$"User Id={this.DataConnectionConfiguration.UserName};Password={this.DataConnectionConfiguration.Password};" +
					$"Connect Timeout={this.DataConnectionConfiguration.ConnectTimeout};Trust Server Certificate=True;" +
					$"Application Name={openApi.Title}";
			}
			else if (this.IsPostgreSQL)
			{
				// PostGreSQL
				connectionString = $"Host={this.DataConnectionConfiguration.Host};Database={this.DataSourceConfiguration.DataSource};" +
					$"Username={this.DataConnectionConfiguration.UserName};Password={this.DataConnectionConfiguration.Password};" +
					$"Timeout={this.DataConnectionConfiguration.ConnectTimeout};SslMode=disable;TrustServerCertificate=true;" +
					$"Include Error Detail=True;Application Name={openApi.Title}";
				if (this.DataConnectionConfiguration.Port != null)
				{
					connectionString += $";Port={this.DataConnectionConfiguration.Port}";
				}
			}
			else
			{
				throw new InvalidOperationException($"Unhandled data source type: '{this.DataSourceConfiguration.DataSourceEnum}' ({this.GetType().Name})");
			}

			return connectionString;
		}

		/// <inheritdoc/>
		public string BuildCount(string tableName)
		{
			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentException(
					$"The {nameof(tableName)} argument cannot be null or empty ({this.GetType().Name})");
			}

			var delimitedSchemaName = DataSourceUtilities.DelimitName(this.DataSourceConfiguration.Schema, this.DataSourceConfiguration.DataSourceEnum);
			var delimitedTableName = DataSourceUtilities.DelimitName(tableName, this.DataSourceConfiguration.DataSourceEnum);

			var querySql = new StringBuilder("SELECT COUNT(*) FROM ");
			querySql.Append(delimitedSchemaName);
			querySql.Append(".");
			querySql.Append(delimitedTableName);

			return querySql.ToString();
		}

		/// <inheritdoc/>
		public string BuildDelete(
			string tableName,
			List<string> keyColumnNames)
		{
			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentException(
					$"The {nameof(tableName)} argument cannot be null or empty ({this.GetType().Name})");
			}

			if (keyColumnNames == null ||
				keyColumnNames.Count == 0)
			{
				throw new ArgumentException(
					$"The {nameof(keyColumnNames)} argument cannot be null or empty ({this.GetType().Name})");
			}

			var delimitedSchemaName = DataSourceUtilities.DelimitName(this.DataSourceConfiguration.Schema, this.DataSourceConfiguration.DataSourceEnum);
			var delimitedTableName = DataSourceUtilities.DelimitName(tableName, this.DataSourceConfiguration.DataSourceEnum);

			var querySql = new StringBuilder("DELETE FROM ");
			querySql.Append(delimitedSchemaName);
			querySql.Append(".");
			querySql.Append(delimitedTableName);

			querySql.Append(" ");

			var whereClause = this.BuildWhereClause(keyColumnNames);
			querySql.Append(whereClause);

			return querySql.ToString();
		}

		/// <inheritdoc/>
		public string BuildGetNextId(string tableName, string keyColumnName, string keyDataType)
		{
			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentException(
					$"The {nameof(tableName)} argument cannot be null or empty ({this.GetType().Name})");
			}

			if (string.IsNullOrWhiteSpace(keyColumnName))
			{
				throw new ArgumentException(
					$"The {nameof(keyColumnName)} argument cannot be null or empty ({this.GetType().Name})");
			}

			if (string.IsNullOrWhiteSpace(keyDataType))
			{
				throw new ArgumentException(
					$"The {nameof(keyDataType)} argument cannot be null or empty ({this.GetType().Name})");
			}

			// get next id value for key column
			var delimitedColumnName = DataSourceUtilities.DelimitName(keyColumnName, this.DataSourceConfiguration.DataSourceEnum);
			string? querySql = null;
			switch (keyDataType)
			{
				case KeyColumnDataTypeConstants.Int:
					var delimitedSchemaName = DataSourceUtilities.DelimitName(this.DataSourceConfiguration.Schema, this.DataSourceConfiguration.DataSourceEnum);
					var delimitedTableName = DataSourceUtilities.DelimitName(tableName, this.DataSourceConfiguration.DataSourceEnum);
					querySql = $"SELECT COALESCE(MAX({delimitedColumnName}), 0) + 1 FROM {delimitedSchemaName}.{delimitedTableName}";

					break;
				case KeyColumnDataTypeConstants.String:
					if (this.IsSqlServer)
					{
						querySql = $"DECLARE @ResultKey NVARCHAR(MAX); " + 
							$"EXEC dbo.GenerateUniqueKeyDynamic " + 
							$"@SchemaName = N'{this.DataSourceConfiguration.Schema}', " + 
							$"@TableName = N'{tableName}', " + 
							$"@ColumnName = N'{keyColumnName}', " + 
							$"@GeneratedKey = @ResultKey OUTPUT; " + 
							$"SELECT @ResultKey AS GeneratedKey;";
					}
					else if (this.IsPostgreSQL)
					{
						querySql = $"SELECT generate_unique_key_dynamic('{this.DataSourceConfiguration.Schema}', '{tableName}', '{keyColumnName}');";
					}
					else
					{
						throw new InvalidOperationException($"Unhandled data source type: '{this.DataSourceConfiguration.DataSourceEnum}' ({this.GetType().Name})");
					}

					break;
				default:
					throw new InvalidOperationException(
						$"The key column datatype ('{keyDataType}') was not handled: Method='{nameof(this.BuildGetNextId)}' " + 
						$"({this.GetType().Name})");
			}

			return querySql;
		}

		/// <inheritdoc/>
		public string BuildInsert(
			string tableName,
			List<string> columnNames,
			List<string> keyColumnNames,
			List<string> entityColumns,
			bool keyIsAutoIncrement = true,
			bool createKey = true)
		{
			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentException(
					$"The {nameof(tableName)} argument cannot be null or empty ({this.GetType().Name})");
			}

			if (columnNames == null ||
				columnNames.Count == 0)
			{
				throw new ArgumentException(
					$"The {nameof(columnNames)} argument cannot be null or empty ({this.GetType().Name})");
			}

			if (keyColumnNames == null ||
				keyColumnNames.Count == 0)
			{
				throw new ArgumentException(
					$"The {nameof(keyColumnNames)} argument cannot be null or empty ({this.GetType().Name})");
			}

			var delimitedSchemaName = DataSourceUtilities.DelimitName(this.DataSourceConfiguration.Schema, this.DataSourceConfiguration.DataSourceEnum);
			var delimitedTableName = DataSourceUtilities.DelimitName(tableName, this.DataSourceConfiguration.DataSourceEnum);
			var minimizedKeyColumnNames = DataSourceUtilities.MinimizeNames(keyColumnNames);

			var querySql = new StringBuilder("INSERT INTO ");
			querySql.Append(delimitedSchemaName);
			querySql.Append(".");
			querySql.Append(delimitedTableName);
			querySql.Append(" (");

			var columnsString = new StringBuilder();
			foreach (var columnName in columnNames)
			{
				var minimizedColumnName = DataSourceUtilities.MinimizeName(columnName);
				var isAutoKeyFilter = 
					createKey && 
					keyIsAutoIncrement &&
					minimizedKeyColumnNames.Contains(minimizedColumnName);
				if (isAutoKeyFilter && this.IsSqlServer)
				{
					// don't include key columns in update set, when autoincrement is active (SqlServer)
					continue;
				}

				if (columnsString.Length > 0)
				{
					columnsString.Append(",");
				}

				var delimitedColumnName = DataSourceUtilities.DelimitName(columnName, this.DataSourceConfiguration.DataSourceEnum);
				columnsString.Append(delimitedColumnName);
			}

			querySql.Append(columnsString);
			querySql.Append(") VALUES (");

			columnsString.Clear();
			for (var columnIndex = 0; columnIndex < columnNames.Count; columnIndex++)
			{
				var minimizedColumnName = DataSourceUtilities.MinimizeName(columnNames[columnIndex]);
				var isAutoKeyFilter = 
					createKey &&
					keyIsAutoIncrement && 
					minimizedKeyColumnNames.Contains(minimizedColumnName);
				if (isAutoKeyFilter && this.IsSqlServer)
				{
					// don't include key columns in values, when autoincrement is active (SqlServer)
					continue;
				}

				if (columnsString.Length > 0)
				{
					columnsString.Append(",");
				}

				var minimizedEntityName = DataSourceUtilities.MinimizeName(entityColumns[columnIndex]);
				columnsString.Append($"@{minimizedEntityName}");
			}

			querySql.Append(columnsString);
			querySql.Append(") ");

			if (createKey && 
				keyIsAutoIncrement && 
				keyColumnNames.Count == 1)
			{
				if (this.IsSqlServer)
				{
					// Use SCOPE_IDENTITY() in SQL Server to retrieve the latest generated identity value.
					// This is used to get the auto-incremented identity value after an INSERT operation.
					querySql.Append("; SELECT CAST(SCOPE_IDENTITY() as int);");
				}
				else if (this.IsPostgreSQL)
				{
					// Use RETURNING in PostgreSQL to retrieve the latest generated identity value.
					// This is used to get the auto-incremented identity value after an INSERT operation.
					var idColumn = keyColumnNames.First();
					querySql.Append(" RETURNING ");
					querySql.Append(idColumn);
					querySql.Append(";");
				}
				else
				{
					throw new InvalidOperationException($"Unhandled data source type: '{this.DataSourceConfiguration.DataSourceEnum}' ({this.GetType().Name})");
				}
			}
			else
			{
				// composite key - cannot return newly created composite ids: so, return a zero.
				// If you need the details about the new row, execute this:
				//	string sql = "INSERT INTO [dbo].[Order Details] ([Discount],[OrderID],[ProductID],[Quantity],[UnitPrice]) " +
				//			 "OUTPUT INSERTED.* " +
				//			 "VALUES (@Discount,@OrderID,@ProductID,@Quantity,@UnitPrice);";

				//	// Returns the full populated OrderDetailsEntity
				//	var result = await connection.QueryFirstOrDefaultAsync<OrderDetailsEntity>(sql, orderDetailsEntity);
				querySql.Append("; SELECT 1 AS id;");
			}

			return querySql.ToString();
		}

		/// <inheritdoc/>
		public string BuildSelect(
			string tableName,
			List<string> columnNames,
			List<string>? keyColumnNames = null)
		{
			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentException(
					$"The {nameof(tableName)} argument cannot be null or empty ({this.GetType().Name})");
			}

			if (columnNames == null ||
				columnNames.Count == 0)
			{
				throw new ArgumentException(
					$"The {nameof(columnNames)} argument cannot be null or empty ({this.GetType().Name})");
			}

			var querySql = new StringBuilder("SELECT ");
			foreach (var columnName in columnNames)
			{
				if (querySql.Length > 7)
				{
					querySql.Append(",");
				}

				var delimitedColumnName = DataSourceUtilities.DelimitName(columnName, this.DataSourceConfiguration.DataSourceEnum);
				querySql.Append(delimitedColumnName);
			}

			var delimitedSchemaName = DataSourceUtilities.DelimitName(this.DataSourceConfiguration.Schema, this.DataSourceConfiguration.DataSourceEnum);
			var delimitedTableName = DataSourceUtilities.DelimitName(tableName, this.DataSourceConfiguration.DataSourceEnum);

			querySql.Append(" FROM ");
			querySql.Append(delimitedSchemaName);
			querySql.Append(".");
			querySql.Append(delimitedTableName);

			if (keyColumnNames != null &&
				keyColumnNames.Count > 0)
			{
				querySql.Append(" ");

				var whereClause = this.BuildWhereClause(keyColumnNames);
				querySql.Append(whereClause);
			}

			return querySql.ToString();
		}

		/// <inheritdoc/>
		public string BuildUpdate(
			string tableName,
			List<string> columnNames,
			List<string> keyColumnNames,
			List<string> entityColumns,
			bool keyIsAutoIncrement = true)
		{
			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentException(
					$"The {nameof(tableName)} argument cannot be null or empty ({this.GetType().Name})");
			}

			if (columnNames == null ||
				columnNames.Count == 0)
			{
				throw new ArgumentException(
					$"The {nameof(columnNames)} argument cannot be null or empty ({this.GetType().Name})");
			}

			if (keyColumnNames == null ||
				keyColumnNames.Count == 0)
			{
				throw new ArgumentException(
					$"The {nameof(keyColumnNames)} argument cannot be null or empty ({this.GetType().Name})");
			}

			var delimitedSchemaName = DataSourceUtilities.DelimitName(this.DataSourceConfiguration.Schema, this.DataSourceConfiguration.DataSourceEnum);
			var delimitedTableName = DataSourceUtilities.DelimitName(tableName, this.DataSourceConfiguration.DataSourceEnum);
			var minimizedKeyColumnNames = DataSourceUtilities.MinimizeNames(keyColumnNames);

			var querySql = new StringBuilder("UPDATE ");
			querySql.Append(delimitedSchemaName);
			querySql.Append(".");
			querySql.Append(delimitedTableName);
			querySql.Append(" SET ");

			var setString = new StringBuilder();
			for (var columnIndex = 0; columnIndex < columnNames.Count; columnIndex++)
			{
				var minimizedColumnName = DataSourceUtilities.MinimizeName(columnNames[columnIndex]);
				if (keyIsAutoIncrement && minimizedKeyColumnNames.Contains(minimizedColumnName))
				{
					// don't include key columns in update set, when autoincrement is active
					continue;
				}

				if (setString.Length > 0)
				{
					setString.Append(",");
				}

				var minimizedEntityName = DataSourceUtilities.MinimizeName(entityColumns[columnIndex]);
				var delimitedColumnName = DataSourceUtilities.DelimitName(columnNames[columnIndex], this.DataSourceConfiguration.DataSourceEnum);
				setString.Append($"{delimitedColumnName} = @{minimizedEntityName}");
			}

			querySql.Append(setString);
			querySql.Append(" ");

			var whereClause = this.BuildWhereClause(keyColumnNames);
			querySql.Append(whereClause);

			return querySql.ToString();
		}

		/// <inheritdoc/>
		public string BuildWhereClause(List<string> keyColumnNames)
		{
			if (keyColumnNames == null ||
				keyColumnNames.Count == 0)
			{
				throw new ArgumentException(
					$"The {nameof(keyColumnNames)} argument cannot be null or empty ({this.GetType().Name})");
			}

			var querySql = new StringBuilder("WHERE ");
			foreach (var keyColumn in keyColumnNames)
			{
				if (querySql.Length > 6)
				{
					querySql.Append(" AND ");
				}

				var delimitedColumnName = DataSourceUtilities.DelimitName(keyColumn, this.DataSourceConfiguration.DataSourceEnum);
				var minimizedColumnName = DataSourceUtilities.MinimizeName(keyColumn);
				querySql.Append($"{delimitedColumnName} = @{minimizedColumnName}");
			}

			return querySql.ToString();
		}

		/// <summary>
		/// Initialize the current object with values from the constructor.
		/// </summary>
		/// <param name="applicationConfiguration">
		/// An application configuration object.
		/// </param>
		protected virtual void Initialize(IApplicationConfiguration applicationConfiguration)
		{
			if (applicationConfiguration == null)
			{
				throw new ConfigurationException(
					$"The {nameof(applicationConfiguration)} argument cannot be null ({this.GetType().Name})");
			}

			this.DataConnectionConfiguration = applicationConfiguration.DataConnections
				.First(f => f.Active);

			if (this.DataConnectionConfiguration == null)
			{
				throw new ConfigurationException(
					$"The {nameof(IApplicationConfiguration)}.{nameof(applicationConfiguration.DataConnections)} " +
					$"section should have at least one active setting ({this.GetType().Name})");
			}

			this.DataSourceConfiguration = applicationConfiguration.GetDataSource(this.DataConnectionConfiguration.DataSourceName);

			if (this.DataSourceConfiguration.DataSourceEnum == DataSourceTypes.Unrecognized &&
				!string.IsNullOrWhiteSpace(this.DataSourceConfiguration.DataSourceType))
			{
				this.DataSourceConfiguration.DataSourceEnum = DataSourceUtilities.ResolveDataSources(this.DataSourceConfiguration.DataSourceType);
			}

			this.IsPostgreSQL = this.DataSourceConfiguration.DataSourceEnum == DataSourceTypes.PostgreSQL;
			this.IsSqlServer = this.DataSourceConfiguration.DataSourceEnum == DataSourceTypes.SqlServer;
		}
	}
}
