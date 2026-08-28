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
	/// A repository for accessing and manipulating order data in the data store.
	/// </summary>
	public class OrdersRepository : RepositoryBase<OrdersEntity>, IOrdersRepository
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="OrdersRepository"/> class.
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
		public OrdersRepository(
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
							"CustomerID",
							"EmployeeID",
							"Freight",
							"OrderDate",
							"OrderID",
							"RequiredDate",
							"ShipAddress",
							"ShipCity",
							"ShipCountry",
							"ShipName",
							"ShippedDate",
							"ShipPostalCode",
							"ShipRegion",
							"ShipVia"
						};

						break;
					case DataSourceTypes.PostgreSQL:
						namesList = new List<string>
						{
							"customerid",
							"employeeid",
							"freight",
							"orderdate",
							"orderid",
							"requireddate",
							"shipaddress",
							"shipcity",
							"shipcountry",
							"shipname",
							"shippeddate",
							"shippostalcode",
							"shipregion",
							"shipvia"
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
					nameof(OrdersEntity.CustomerID),
					nameof(OrdersEntity.EmployeeID),
					nameof(OrdersEntity.Freight),
					nameof(OrdersEntity.OrderDate),
					nameof(OrdersEntity.OrderID),
					nameof(OrdersEntity.RequiredDate),
					nameof(OrdersEntity.ShipAddress),
					nameof(OrdersEntity.ShipCity),
					nameof(OrdersEntity.ShipCountry),
					nameof(OrdersEntity.ShipName),
					nameof(OrdersEntity.ShippedDate),
					nameof(OrdersEntity.ShipPostalCode),
					nameof(OrdersEntity.ShipRegion),
					nameof(OrdersEntity.ShipVia) 
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
							"OrderID"
						};

						break;
					case DataSourceTypes.PostgreSQL:
						namesList = new List<string>
						{
							"orderid"
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
						tablename = "Orders";

						break;
					case DataSourceTypes.PostgreSQL:
						tablename = "orders";

						break;
					default:
						throw new InvalidOperationException($"Unhandled data source type: '{this.DataSourceContext.DataSourceConfiguration.DataSourceEnum}' ({this.GetType().Name})");
				}

				return tablename;
			}
		}
	}
}
