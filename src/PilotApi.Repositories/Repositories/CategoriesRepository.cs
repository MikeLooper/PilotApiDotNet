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
	/// A repository for accessing and manipulating category data in the data store.
	/// </summary>
	public class CategoriesRepository : RepositoryBase<CategoriesEntity>, ICategoriesRepository
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="CategoriesRepository"/> class.
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
		public CategoriesRepository(
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
							"CategoryID",
							"CategoryName",
							"Description",
							"Picture"
						};

						break;
					case DataSourceTypes.PostgreSQL:
						namesList = new List<string>
						{
							"categoryid",
							"categoryname",
							"description",
							"picture"
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
					nameof(CategoriesEntity.CategoryID),
					nameof(CategoriesEntity.CategoryName),
					nameof(CategoriesEntity.Description),
					nameof(CategoriesEntity.Picture)
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
							"CategoryID"
						};
						
						break;
					case DataSourceTypes.PostgreSQL:
						namesList = new List<string>
						{
							"categoryid"
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

				switch(this.DataSourceContext.DataSourceConfiguration.DataSourceEnum)
				{
					case DataSourceTypes.SqlServer:
						tablename = "Categories";
						
						break;
					case DataSourceTypes.PostgreSQL:
						tablename = "categories";
						
						break;
					default:
						throw new InvalidOperationException($"Unhandled data source type: '{this.DataSourceContext.DataSourceConfiguration.DataSourceEnum}' ({this.GetType().Name})");
				}

				return tablename;
			}
		}
	}
}
