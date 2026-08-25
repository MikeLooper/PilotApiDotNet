using Microsoft.Extensions.Logging;
using PilotApi.Domain.Contracts.DataSource;
using PilotApi.Repositories.Contracts.Repository;
using PilotApi.Repositories.Handlers;
using PilotApi.Repositories.Models.Entities;
using PilotApi.Repositories.Repositories.Base;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Handlers;
using System;
using System.Collections.Generic;

namespace PilotApi.Repositories.Repositories
{
	/// <summary>
	/// A repository for accessing and manipulating employee data in the data store.
	/// </summary>
	public class EmployeesRepository : RepositoryBase<EmployeesEntity>, IEmployeesRepository
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="EmployeesRepository"/> class.
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
		public EmployeesRepository(
			ILoggerFactory loggerFactory,
			IDataSourceContext dataStoreContext,
			ISqlBuilder sqlBuilder,
			IEntityUpdateHandler entityUpdateHandler)
			: base(loggerFactory, dataStoreContext, sqlBuilder, entityUpdateHandler)
		{
		}

		/// <inheritdoc/>
		protected override List<string> ColumnNames
		{
			get
			{
				List<string>? namesList = null;

				switch (this.DataSourceContext.DataSourceConfiguration.DataSourceEnum)
				{
					case DataSourceTypes.SqlServer:
						namesList = new List<string>
						{
							"Address",
							"BirthDate",
							"City",
							"Country",
							"EmployeeID",
							"Extension",
							"FirstName",
							"HireDate",
							"HomePhone",
							"LastName",
							"Notes",
							"Photo",
							"PhotoPath",
							"PostalCode",
							"Region",
							"ReportsTo",
							"Title",
							"TitleOfCourtesy"
						};

						break;
					case DataSourceTypes.PostgreSQL:
						namesList = new List<string>
						{
							"address",
							"birthdate",
							"city",
							"country",
							"employeeid",
							"extension",
							"firstname",
							"hiredate",
							"homephone",
							"lastname",
							"notes",
							"photo",
							"photopath",
							"postalcode",
							"region",
							"reportsto",
							"title",
							"titleofcourtesy"
						};

						break;
					default:
						throw new InvalidOperationException($"Unhandled data source type: '{this.DataSourceContext.DataSourceConfiguration.DataSourceEnum}' ({this.GetType().Name})");
				}

				return namesList;
			}
		}

		/// <inheritdoc/>
		protected override List<string> EntityColumns
		{
			get
			{
				List<string>? propertiesList = new List<string>
				{
					nameof(EmployeesEntity.Address),
					nameof(EmployeesEntity.BirthDate),
					nameof(EmployeesEntity.City),
					nameof(EmployeesEntity.Country),
					nameof(EmployeesEntity.EmployeeID),
					nameof(EmployeesEntity.Extension),
					nameof(EmployeesEntity.FirstName),
					nameof(EmployeesEntity.HireDate),
					nameof(EmployeesEntity.HomePhone),
					nameof(EmployeesEntity.LastName),
					nameof(EmployeesEntity.Notes),
					nameof(EmployeesEntity.Photo),
					nameof(EmployeesEntity.PhotoPath),
					nameof(EmployeesEntity.PostalCode),
					nameof(EmployeesEntity.Region),
					nameof(EmployeesEntity.ReportsTo),
					nameof(EmployeesEntity.Title),
					nameof(EmployeesEntity.TitleOfCourtesy)
				};

				return propertiesList;
			}
		}

		/// <inheritdoc/>
		protected override List<string> KeyColumnNames
		{
			get
			{
				List<string>? namesList = null;

				switch (this.DataSourceContext.DataSourceConfiguration.DataSourceEnum)
				{
					case DataSourceTypes.SqlServer:
						namesList = new List<string>
						{
							"EmployeeID"
						};

						break;
					case DataSourceTypes.PostgreSQL:
						namesList = new List<string>
						{
							"employeeid"
						};

						break;
					default:
						throw new InvalidOperationException($"Unhandled data source type: '{this.DataSourceContext.DataSourceConfiguration.DataSourceEnum}' ({this.GetType().Name})");
				}

				return namesList;
			}
		}

		/// <inheritdoc/>
		protected override string TableName
		{
			get
			{
				string? tablename = null;

				switch (this.DataSourceContext.DataSourceConfiguration.DataSourceEnum)
				{
					case DataSourceTypes.SqlServer:
						tablename = "Employees";

						break;
					case DataSourceTypes.PostgreSQL:
						tablename = "employees";

						break;
					default:
						throw new InvalidOperationException($"Unhandled data source type: '{this.DataSourceContext.DataSourceConfiguration.DataSourceEnum}' ({this.GetType().Name})");
				}

				return tablename;
			}
		}
	}
}
