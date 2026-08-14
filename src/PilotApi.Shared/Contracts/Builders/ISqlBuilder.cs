using PilotApi.Shared.Configuration;
using PilotApi.Shared.Constants;
using System.Collections.Generic;

namespace PilotApi.Shared.Handlers
{
	/// <summary>
	/// An interface for the handler for SQL building operations.
	/// </summary>
	public interface ISqlBuilder
	{
		/// <summary>
		/// Build and return a connection string.
		/// </summary>
		/// <param name="openApi">
		/// A <see cref="OpenApiConfiguration"/> object.
		/// </param>
		/// <returns>
		/// A connection string.
		/// </returns>
		string? BuildConnectionString(OpenApiConfiguration? openApi);

		/// <summary>
		/// Build and return a Count(*) string.
		/// </summary>
		/// <param name="tableName">
		/// The name of the current table.
		/// </param>
		/// <returns>
		/// A COUNT string.
		/// </returns>
		string BuildCount(string tableName);

		/// <summary>
		/// Build and return a DELETE string, based upon the supplied table name and key columns property.
		/// The key columns will be included in the WHERE clasue of the DELETE string as parameter substitutions in order of the keyColumnNames list.
		/// </summary>
		/// <param name="tableName">
		/// The name of the current table.
		/// </param>
		/// <param name="keyColumnNames">
		/// A list of the column names for the source entity key(s).
		/// </param>
		/// <returns>
		/// A DELETE string.
		/// </returns>
		string BuildDelete(string tableName, List<string> keyColumnNames);

		/// <summary>
		/// Build and return a SQL string that will determine the next id (key) value for the supplied table.
		/// </summary>
		/// <param name="tableName">
		/// The name of the target table.
		/// </param>
		/// <param name="keyColumnName">
		/// The name of the key column, for this table.
		/// </param>
		/// <param name="keyDataType">
		/// The column datatype (as a string) for the current table key.
		/// This will be one of the <see cref="KeyColumnDataTypeConstants"/> options.
		/// </param>
		/// <returns>
		/// </returns>
		string BuildGetNextId(string tableName, string keyColumnName, string keyDataType);

		/// <summary>
		/// Build and return an INSERT string.
		/// </summary>
		/// <param name="tableName">
		/// The name of the current table.
		/// </param>
		/// <param name="columnNames">
		/// A list of the column names for the source entity.
		/// </param>
		/// <param name="keyColumnNames">
		/// A list of the column names for the source entity key(s), to be used in the WHERE clause.
		/// </param>
		/// <param name="entityColumns">
		/// A list of the column names for the entity related to the current table, to be used in the SET clause.
		/// </param>
		/// <param name="keyIsAutoIncrement">
		/// A flag that indicates whether the key in the supplied table will auto-increment during insert.
		/// Default = True.
		/// </param>
		/// <returns>
		/// An INSERT string.
		/// </returns>
		string BuildInsert(
			string tableName, 
			List<string> columnNames, 
			List<string> keyColumnNames,
			List<string> entityColumns,
			bool keyIsAutoIncrement = true);

		/// <summary>
		/// Build and return a SELECT string.
		/// </summary>
		/// <param name="tableName">
		/// The name of the current table.
		/// </param>
		/// <param name="columnNames">
		/// A list of the column names for the source entity.
		/// </param>
		/// <param name="keyColumnNames">
		/// A list of the column names for the source entity key(s), to be used in the WHERE clause.
		/// If this list is null or empty, no WHERE clause will be included.
		/// Default = Null.
		/// </param>
		/// <returns>
		/// A SELECT string.
		/// </returns>
		string BuildSelect(
			string tableName, 
			List<string> columnNames, 
			List<string>? keyColumnNames = null);

		/// <summary>
		/// Build and return an UPDATE string.
		/// </summary>
		/// <param name="tableName">
		/// The name of the current table.
		/// </param>
		/// <param name="columnNames">
		/// A list of the column names for the source entity.
		/// </param>
		/// <param name="keyColumnNames">
		/// A list of the column names for the source entity key(s), to be used in the WHERE clause.
		/// </param>
		/// <param name="entityColumns">
		/// A list of the column names for the entity related to the current table, to be used in the SET clause.
		/// </param>
		/// <param name="keyIsAutoIncrement">
		/// A flag that indicates whether the key in the supplied table will auto-increment during insert.
		/// Default = True.
		/// </param>
		/// <returns>
		/// An UPDATE string.
		/// </returns>
		string BuildUpdate(
			string tableName, 
			List<string> columnNames,
			List<string> keyColumnNames,
			List<string> entityColumns,
			bool keyIsAutoIncrement = true);

		/// <summary>
		/// Build and return a WHERE clause, based upon the supplied key columns property.
		/// The key columns will be included in the where string as parameter substitutions in order of the keyColumnNames list.
		/// </summary>
		/// <param name="keyColumnNames">
		/// A list of the column names for the source entity key(s).
		/// </param>
		/// <returns>
		/// A WHERE clause string.
		/// </returns>
		string BuildWhereClause(List<string> keyColumnNames);
	}
}
