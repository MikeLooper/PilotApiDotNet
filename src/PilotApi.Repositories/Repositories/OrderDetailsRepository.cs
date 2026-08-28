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
	/// A repository for accessing and manipulating order details data in the data store.
	/// </summary>
	public class OrderDetailsRepository : RepositoryBase<OrderDetailsEntity>, IOrderDetailsRepository
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="OrderDetailsRepository"/> class.
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
		/// <remarks>
		/// The source table contains a two-column key: ProductID, OrderID.
		/// The KeyColumnNames property lists the two columns in that order.
		/// So, the GetById and Delete methods must handle IDs in that order.
		/// </remarks>
		public OrderDetailsRepository(
			ILoggerFactory loggerFactory,
			IDataSourceContext dataStoreContext,
			ISqlBuilder sqlBuilder,
			IEntityUpdateHandler entityUpdateHandler)
			: base(loggerFactory, dataStoreContext, sqlBuilder, entityUpdateHandler)
		{
			this.KeyIsAutoIncrement = false;
			this.KeyColumnDataTypes = new List<string>
			{
				KeyColumnDataTypeConstants.Int,
				KeyColumnDataTypeConstants.Int
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
							"Discount",
							"OrderID",
							"ProductID",
							"Quantity",
							"UnitPrice"
						};

						break;
					case DataSourceTypes.PostgreSQL:
						namesList = new List<string>
						{
							"discount",
							"orderid",
							"productid",
							"quantity",
							"unitprice"
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
					nameof(OrderDetailsEntity.Discount),
					nameof(OrderDetailsEntity.OrderID),
					nameof(OrderDetailsEntity.ProductID),
					nameof(OrderDetailsEntity.Quantity),
					nameof(OrderDetailsEntity.UnitPrice)
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
							"ProductID",
							"OrderID"
						};

						break;
					case DataSourceTypes.PostgreSQL:
						namesList = new List<string>
						{
							"productid",
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
						tablename = "Order Details";

						break;
					case DataSourceTypes.PostgreSQL:
						tablename = "orderdetails";

						break;
					default:
						throw new InvalidOperationException($"Unhandled data source type: '{this.DataSourceContext.DataSourceConfiguration.DataSourceEnum}' ({this.GetType().Name})");
				}

				return tablename;
			}
		}
	}
}
