using Microsoft.Extensions.Logging;
using PilotApi.Domain.Contracts.DataSource;
using PilotApi.Repositories.Handlers;
using PilotApi.Repositories.Repositories;
using PilotApi.Shared.Handlers;
using System.Collections.Generic;
using System.Diagnostics;

namespace PilotApi.Repositories.Tests.Testing.Doubles
{
	public class CategoriesRepositorySpy : CategoriesRepository
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="CategoriesRepositorySpy"/> class.
		/// </summary>
		/// <param name="loggerFactory">
		/// A logger factory used to create loggers for logging information, warnings, and errors.
		/// </param>
		/// <param name="dataStoreContext">
		/// A data store context that provides access to the underlying data store for performing CRUD operations.
		/// </param>
		/// <param name="sqlBuilder">
		/// A SQL builder object.
		/// </param>
		/// <param name="entityUpdateHandler">
		/// An entity update handler object.
		/// </param>
		public CategoriesRepositorySpy(
			ILoggerFactory loggerFactory,
			IDataSourceContext dataStoreContext,
			ISqlBuilder sqlBuilder,
			IEntityUpdateHandler entityUpdateHandler)
			: base(loggerFactory, dataStoreContext, sqlBuilder, entityUpdateHandler)
		{
		}

		/// <summary>
		/// A spy property to access the protected ColumnNames property from the base class.
		/// </summary>
		public List<string> ColumnNamesSpy { get { return base.ColumnNames; } }

		/// <summary>
		/// A spy property to access the protected EntityColumns property from the base class.
		/// </summary>
		public List<string> EntityColumnsSpy { get { return base.EntityColumns; } }

		/// <summary>
		/// A spy property to access the protected KeyColumnNames property from the base class.
		/// </summary>
		public List<string> KeyColumnNamesSpy { get { return base.KeyColumnNames; } }

		/// <summary>
		/// A spy property to access the protected TableName property from the base class.
		/// </summary>
		public string TableNameSpy { get { return base.TableName; } }

		public Stopwatch LogDatabaseCallStartSpy(string methodName) => base.LogDatabaseCallStart(methodName);

		public void LogDatabaseCallEndSpy(string methodName, Stopwatch stopwatch) => base.LogDatabaseCallEnd(methodName, stopwatch);
	}
}
