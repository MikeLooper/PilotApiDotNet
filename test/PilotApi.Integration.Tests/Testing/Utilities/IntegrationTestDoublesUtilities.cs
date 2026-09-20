using Microsoft.Extensions.Logging;
using PilotApi.Integration.Tests.Testing.Doubles;
using PilotApi.Repositories.Handlers;
using PilotApi.Repositories.Repositories;
using PilotApi.Services.Handlers;
using PilotApi.Services.Services;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Handlers;
using PilotApi.TestingShared.Doubles;
using PilotApi.TestingShared.Utilities;

namespace PilotApi.Integration.Tests.Testing.Utilities
{
	/// <summary>
	/// Utility methods for building shared dependencies used across integration test classes.
	/// Focused on constructing real, fully-wired production objects (repositories, services, controllers) backed 
	/// by fake ADO.NET connection infrastructure instead of mocked repository/service interfaces.
	/// </summary>
	public static class IntegrationTestDoublesUtilities
	{
		/// <summary>
		/// Creates a real <see cref="CategoriesRepository"/> instance, wired to the supplied
		/// <see cref="TestableDataSourceContext"/> and <see cref="SqlBuilder"/>.
		/// </summary>
		/// <param name="dataSourceContext">
		/// The data source context to use, typically obtained via <see cref="GetDataSourceContext"/>.
		/// </param>
		/// <param name="sqlBuilder">
		/// The SQL builder to use, typically obtained via <see cref="GetSqlBuilder"/>.
		/// </param>
		/// <returns>
		/// A <see cref="CategoriesRepository"/> instance.
		/// </returns>
		public static CategoriesRepository GetCategoriesRepository(
			TestableDataSourceContext dataSourceContext,
			SqlBuilder sqlBuilder)
		{
			var loggerFactory = GetLoggerFactory();

			return new CategoriesRepository(loggerFactory, dataSourceContext, sqlBuilder, new EntityUpdateHandler());
		}

		/// <summary>
		/// Creates a real <see cref="CategoriesService"/> instance.
		/// </summary>
		/// <param name="fakeConnection">
		/// The fake ADO.NET connection to use, typically obtained via <see cref="FakeDbConnection"/>.
		/// Optional; if not supplied, a new <see cref="FakeDbConnection"/> will be created.
		/// </param>
		/// <returns>
		/// A <see cref="CategoriesService"/> instance.
		/// </returns>
		public static CategoriesService GetCategoriesService(FakeDbConnection? fakeConnection = null)
		{
			var loggerFactory = GetLoggerFactory();
			var dataMapperHandler = new DataMapperHandler();
			var sqlBuilder = GetSqlBuilder();

			if (fakeConnection == null)
			{
				fakeConnection = new FakeDbConnection();
			}

			var dataSourceContext = GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = GetCategoriesRepository(dataSourceContext, sqlBuilder);

			return new CategoriesService(loggerFactory, repository, dataMapperHandler);
		}

		/// <summary>
		/// Creates a real <see cref="TestableDataSourceContext"/> instance, backed by the supplied
		/// <see cref="FakeDbConnection"/> so that no live database server is required.
		/// </summary>
		/// <param name="fakeDbConnection">
		/// The fake ADO.NET connection that will back the data source context.
		/// </param>
		/// <param name="sqlBuilder">
		/// The SQL builder to use, typically obtained via <see cref="GetSqlBuilder"/>.
		/// </param>
		/// <param name="dataSourceType">
		/// The <see cref="DataSourceTypes"/> value to use for the mock application configuration.
		/// Default = <see cref="DataSourceTypes.SqlServer"/>.
		/// </param>
		/// <returns>
		/// A <see cref="TestableDataSourceContext"/> instance.
		/// </returns>
		public static TestableDataSourceContext GetDataSourceContext(
			FakeDbConnection fakeDbConnection,
			SqlBuilder sqlBuilder,
			DataSourceTypes dataSourceType = DataSourceTypes.SqlServer)
		{
			var applicationConfiguration = TestingSharedDoublesUtilities.GetApplicationConfiguration(dataSourceType);
			var loggerFactory = GetLoggerFactory();

			return new TestableDataSourceContext(loggerFactory, applicationConfiguration, sqlBuilder, fakeDbConnection);
		}

		/// <summary>
		/// Creates a logger factory instance suitable for use with real service/handler classes under test.
		/// </summary>
		/// <returns>
		/// An <see cref="ILoggerFactory"/> instance.
		/// </returns>
		public static ILoggerFactory GetLoggerFactory()
		{
			return new MockLoggerFactory();
		}

		/// <summary>
		/// Creates a real <see cref="SqlBuilder"/> instance for the supplied data source type.
		/// Shared by all repository-builder methods so a single SQL builder can be re-used across
		/// a repository and its owning <see cref="TestableDataSourceContext"/>.
		/// </summary>
		/// <param name="dataSourceType">
		/// The <see cref="DataSourceTypes"/> value to use for the mock application configuration.
		/// Default = <see cref="DataSourceTypes.SqlServer"/>.
		/// </param>
		/// <returns>
		/// A <see cref="SqlBuilder"/> instance.
		/// </returns>
		public static SqlBuilder GetSqlBuilder(DataSourceTypes dataSourceType = DataSourceTypes.SqlServer)
		{
			var applicationConfiguration = TestingSharedDoublesUtilities.GetApplicationConfiguration(dataSourceType);
			var loggerFactory = GetLoggerFactory();

			return new SqlBuilder(loggerFactory, applicationConfiguration);
		}

		/// <summary>
		/// Creates a real <see cref="CustomersRepository"/> instance, wired to the supplied
		/// <see cref="TestableDataSourceContext"/> and <see cref="SqlBuilder"/>.
		/// </summary>
		public static CustomersRepository GetCustomersRepository(
			TestableDataSourceContext dataSourceContext,
			SqlBuilder sqlBuilder)
		{
			var loggerFactory = GetLoggerFactory();

			return new CustomersRepository(loggerFactory, dataSourceContext, sqlBuilder, new EntityUpdateHandler());
		}

		/// <summary>
		/// Creates a real <see cref="CustomersService"/> instance.
		/// </summary>
		public static CustomersService GetCustomersService(FakeDbConnection? fakeConnection = null)
		{
			var loggerFactory = GetLoggerFactory();
			var dataMapperHandler = new DataMapperHandler();
			var sqlBuilder = GetSqlBuilder();

			if (fakeConnection == null)
			{
				fakeConnection = new FakeDbConnection();
			}

			var dataSourceContext = GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = GetCustomersRepository(dataSourceContext, sqlBuilder);

			return new CustomersService(loggerFactory, repository, dataMapperHandler);
		}

		/// <summary>
		/// Creates a real <see cref="EmployeesRepository"/> instance, wired to the supplied
		/// <see cref="TestableDataSourceContext"/> and <see cref="SqlBuilder"/>.
		/// </summary>
		public static EmployeesRepository GetEmployeesRepository(
			TestableDataSourceContext dataSourceContext,
			SqlBuilder sqlBuilder)
		{
			var loggerFactory = GetLoggerFactory();

			return new EmployeesRepository(loggerFactory, dataSourceContext, sqlBuilder, new EntityUpdateHandler());
		}

		/// <summary>
		/// Creates a real <see cref="EmployeesService"/> instance.
		/// </summary>
		public static EmployeesService GetEmployeesService(FakeDbConnection? fakeConnection = null)
		{
			var loggerFactory = GetLoggerFactory();
			var dataMapperHandler = new DataMapperHandler();
			var sqlBuilder = GetSqlBuilder();

			if (fakeConnection == null)
			{
				fakeConnection = new FakeDbConnection();
			}

			var dataSourceContext = GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = GetEmployeesRepository(dataSourceContext, sqlBuilder);

			return new EmployeesService(loggerFactory, repository, dataMapperHandler);
		}

		/// <summary>
		/// Creates a real <see cref="OrderDetailsRepository"/> instance, wired to the supplied
		/// <see cref="TestableDataSourceContext"/> and <see cref="SqlBuilder"/>.
		/// </summary>
		public static OrderDetailsRepository GetOrderDetailsRepository(
			TestableDataSourceContext dataSourceContext,
			SqlBuilder sqlBuilder)
		{
			var loggerFactory = GetLoggerFactory();

			return new OrderDetailsRepository(loggerFactory, dataSourceContext, sqlBuilder, new EntityUpdateHandler());
		}

		/// <summary>
		/// Creates a real <see cref="OrderDetailsService"/> instance.
		/// </summary>
		public static OrderDetailsService GetOrderDetailsService(FakeDbConnection? fakeConnection = null)
		{
			var loggerFactory = GetLoggerFactory();
			var dataMapperHandler = new DataMapperHandler();
			var sqlBuilder = GetSqlBuilder();

			if (fakeConnection == null)
			{
				fakeConnection = new FakeDbConnection();
			}

			var dataSourceContext = GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = GetOrderDetailsRepository(dataSourceContext, sqlBuilder);

			return new OrderDetailsService(loggerFactory, repository, dataMapperHandler);
		}

		/// <summary>
		/// Creates a real <see cref="OrdersRepository"/> instance, wired to the supplied
		/// <see cref="TestableDataSourceContext"/> and <see cref="SqlBuilder"/>.
		/// </summary>
		public static OrdersRepository GetOrdersRepository(
			TestableDataSourceContext dataSourceContext,
			SqlBuilder sqlBuilder)
		{
			var loggerFactory = GetLoggerFactory();

			return new OrdersRepository(loggerFactory, dataSourceContext, sqlBuilder, new EntityUpdateHandler());
		}

		/// <summary>
		/// Creates a real <see cref="OrdersService"/> instance.
		/// </summary>
		public static OrdersService GetOrdersService(FakeDbConnection? fakeConnection = null)
		{
			var loggerFactory = GetLoggerFactory();
			var dataMapperHandler = new DataMapperHandler();
			var sqlBuilder = GetSqlBuilder();

			if (fakeConnection == null)
			{
				fakeConnection = new FakeDbConnection();
			}

			var dataSourceContext = GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = GetOrdersRepository(dataSourceContext, sqlBuilder);

			return new OrdersService(loggerFactory, repository, dataMapperHandler);
		}

		/// <summary>
		/// Creates a real <see cref="ProductsRepository"/> instance, wired to the supplied
		/// <see cref="TestableDataSourceContext"/> and <see cref="SqlBuilder"/>.
		/// </summary>
		public static ProductsRepository GetProductsRepository(
			TestableDataSourceContext dataSourceContext,
			SqlBuilder sqlBuilder)
		{
			var loggerFactory = GetLoggerFactory();

			return new ProductsRepository(loggerFactory, dataSourceContext, sqlBuilder, new EntityUpdateHandler());
		}

		/// <summary>
		/// Creates a real <see cref="ProductsService"/> instance.
		/// </summary>
		public static ProductsService GetProductsService(FakeDbConnection? fakeConnection = null)
		{
			var loggerFactory = GetLoggerFactory();
			var dataMapperHandler = new DataMapperHandler();
			var sqlBuilder = GetSqlBuilder();

			if (fakeConnection == null)
			{
				fakeConnection = new FakeDbConnection();
			}

			var dataSourceContext = GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = GetProductsRepository(dataSourceContext, sqlBuilder);

			return new ProductsService(loggerFactory, repository, dataMapperHandler);
		}

		/// <summary>
		/// Creates a real <see cref="ShippersRepository"/> instance, wired to the supplied
		/// <see cref="TestableDataSourceContext"/> and <see cref="SqlBuilder"/>.
		/// </summary>
		public static ShippersRepository GetShippersRepository(
			TestableDataSourceContext dataSourceContext,
			SqlBuilder sqlBuilder)
		{
			var loggerFactory = GetLoggerFactory();

			return new ShippersRepository(loggerFactory, dataSourceContext, sqlBuilder, new EntityUpdateHandler());
		}

		/// <summary>
		/// Creates a real <see cref="ShippersService"/> instance.
		/// </summary>
		public static ShippersService GetShippersService(FakeDbConnection? fakeConnection = null)
		{
			var loggerFactory = GetLoggerFactory();
			var dataMapperHandler = new DataMapperHandler();
			var sqlBuilder = GetSqlBuilder();

			if (fakeConnection == null)
			{
				fakeConnection = new FakeDbConnection();
			}

			var dataSourceContext = GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = GetShippersRepository(dataSourceContext, sqlBuilder);

			return new ShippersService(loggerFactory, repository, dataMapperHandler);
		}

		/// <summary>
		/// Creates a real <see cref="SuppliersRepository"/> instance, wired to the supplied
		/// <see cref="TestableDataSourceContext"/> and <see cref="SqlBuilder"/>.
		/// </summary>
		public static SuppliersRepository GetSuppliersRepository(
			TestableDataSourceContext dataSourceContext,
			SqlBuilder sqlBuilder)
		{
			var loggerFactory = GetLoggerFactory();

			return new SuppliersRepository(loggerFactory, dataSourceContext, sqlBuilder, new EntityUpdateHandler());
		}

		/// <summary>
		/// Creates a real <see cref="SuppliersService"/> instance.
		/// </summary>
		public static SuppliersService GetSuppliersService(FakeDbConnection? fakeConnection = null)
		{
			var loggerFactory = GetLoggerFactory();
			var dataMapperHandler = new DataMapperHandler();
			var sqlBuilder = GetSqlBuilder();

			if (fakeConnection == null)
			{
				fakeConnection = new FakeDbConnection();
			}

			var dataSourceContext = GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = GetSuppliersRepository(dataSourceContext, sqlBuilder);

			return new SuppliersService(loggerFactory, repository, dataMapperHandler);
		}
	}
}
