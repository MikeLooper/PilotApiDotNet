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
	/// A repository for accessing and manipulating customer data in the data store.
	/// </summary>
	public class CustomersRepository : RepositoryBase<CustomersEntity>, ICustomersRepository
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="CustomersRepository"/> class.
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
		public CustomersRepository(
			ILoggerFactory loggerFactory,
			IDataSourceContext dataStoreContext,
			ISqlBuilder sqlBuilder,
			IEntityUpdateHandler entityUpdateHandler)
			: base(loggerFactory, dataStoreContext, sqlBuilder, entityUpdateHandler)
		{
			this.KeyIsAutoIncrement = false;
			this.KeyColumnDataTypes = new List<string>
			{
				KeyColumnDataTypeConstants.String
			};
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
							"City",
							"CompanyName",
							"ContactName",
							"ContactTitle",
							"Country",
							"CustomerID",
							"Fax",
							"Phone",
							"PostalCode",
							"Region"
						};

						break;
					case DataSourceTypes.PostgreSQL:
						namesList = new List<string>
						{
							"address",
							"city",
							"companyname",
							"contactname",
							"contacttitle",
							"country",
							"customerid",
							"fax",
							"phone",
							"postalcode",
							"region"
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
					nameof(CustomersEntity.Address),
					nameof(CustomersEntity.City),
					nameof(CustomersEntity.CompanyName),
					nameof(CustomersEntity.ContactName),
					nameof(CustomersEntity.ContactTitle),
					nameof(CustomersEntity.Country),
					nameof(CustomersEntity.CustomerID),
					nameof(CustomersEntity.Fax),
					nameof(CustomersEntity.Phone),
					nameof(CustomersEntity.PostalCode),
					nameof(CustomersEntity.Region)
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
							"CustomerID"
						};

						break;
					case DataSourceTypes.PostgreSQL:
						namesList = new List<string>
						{
							"customerid"
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
						tablename = "Customers";

						break;
					case DataSourceTypes.PostgreSQL:
						tablename = "customers";

						break;
					default:
						throw new InvalidOperationException($"Unhandled data source type: '{this.DataSourceContext.DataSourceConfiguration.DataSourceEnum}' ({this.GetType().Name})");
				}

				return tablename;
			}
		}
	}
}
