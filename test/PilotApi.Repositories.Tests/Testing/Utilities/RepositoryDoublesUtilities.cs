using Microsoft.Extensions.Logging;
using Moq;
using PilotApi.Domain.Contracts.DataSource;
using PilotApi.Repositories.Handlers;
using PilotApi.Repositories.Tests.Testing.Doubles;
using PilotApi.Shared.Configuration;
using PilotApi.Shared.Constants;
using PilotApi.Shared.Contracts.Configuration;
using PilotApi.Shared.Handlers;
using System.Data;
using System.Threading.Tasks;

namespace PilotApi.Repositories.Tests.Testing.Utilities
{
	/// <summary>
	/// Utility method for working with repository doubles in unit tests.
	/// Provides methods to create and configure mock repositories for testing purposes.
	/// </summary>
	public static class RepositoryDoublesUtilities
	{
		/// <summary>
		/// A spy for the SuppliersRepository class, which allows for testing and verifying interactions with the repository.
		/// </summary>
		/// <returns></returns>
		public static SuppliersRepositorySpy GetSuppliersRepositorySpy()
		{
			var loggerFactory = new Mock<ILoggerFactory>();

			var dataConnectionConfiguration = new Mock<IDataConnectionConfiguration>();
			dataConnectionConfiguration.Setup(s => s.ConnectTimeout)
				.Returns(30);
			dataConnectionConfiguration.Setup(s => s.DataSourceName)
				.Returns("TestDb");
			dataConnectionConfiguration.Setup(s => s.Host)
				.Returns("localhost");
			dataConnectionConfiguration.Setup(s => s.Port);
			dataConnectionConfiguration.Setup(s => s.UserName)
				.Returns("myUserName");
			dataConnectionConfiguration.Setup(s => s.Password)
				.Returns("myUserPass");

			var dataSourceConfiguration = new Mock<IDataSourceConfiguration>();
			dataSourceConfiguration.Setup(s => s.DataSourceName)
				.Returns("TestDb");
			dataSourceConfiguration.Setup(s => s.DataSourceType)
				.Returns("SqlServer");
			dataSourceConfiguration.Setup(s => s.DataSourceEnum)
				.Returns(DataSourceTypes.SqlServer);
			dataSourceConfiguration.Setup(s => s.Schema)
				.Returns("dbo");
			dataSourceConfiguration.Setup(s => s.DataSource)
				.Returns("TestDb");

			var dataSourceContext = new Mock<IDataSourceContext>();
			dataSourceContext.Setup(s => s.GetConnection(
				It.IsAny<bool>()))
				.Returns(Mock.Of<IDbConnection>());
			dataSourceContext.Setup(s => s.DataSourceTransaction)
				.Returns(Mock.Of<IDbTransaction>());
			dataSourceContext.Setup(s => s.CommitAsync())
				.Returns(Task.CompletedTask);
			dataSourceContext.Setup(s => s.RollbackAsync())
				.Returns(Task.CompletedTask);
			dataSourceContext.Setup(s => s.Dispose());
			dataSourceContext.Setup(s => s.ConnectionStringClean)
				.Returns("Server=localhost;Database=TestDb;User Id=testuser;Password=[Redacted];");
			dataSourceContext.Setup(s => s.DataConnectionConfiguration)
				.Returns(dataConnectionConfiguration.Object);
			dataSourceContext.Setup(s => s.DataSourceConfiguration)
				.Returns(dataSourceConfiguration.Object);

			var sqlBuilder = new Mock<ISqlBuilder>();

			var entityUpdateHandler = new Mock<IEntityUpdateHandler>();

			var testObject = new SuppliersRepositorySpy(
				loggerFactory.Object,
				dataSourceContext.Object,
				sqlBuilder.Object,
				entityUpdateHandler.Object);

			return testObject;
		}

		/// <summary>
		/// A spy for the CategoriesRepository class, which allows for testing and verifying interactions with the repository.
		/// </summary>
		/// <returns></returns>
		public static CategoriesRepositorySpy GetCategoriesRepositorySpy()
		{
			var loggerFactory = new Mock<ILoggerFactory>();

			var dataConnectionConfiguration = new Mock<IDataConnectionConfiguration>();
			dataConnectionConfiguration.Setup(s => s.ConnectTimeout)
				.Returns(30);
			dataConnectionConfiguration.Setup(s => s.DataSourceName)
				.Returns("TestDb");
			dataConnectionConfiguration.Setup(s => s.Host)
				.Returns("localhost");
			dataConnectionConfiguration.Setup(s => s.Port);
			dataConnectionConfiguration.Setup(s => s.UserName)
				.Returns("myUserName");
			dataConnectionConfiguration.Setup(s => s.Password)
				.Returns("myUserPass");

			var dataSourceConfiguration = new Mock<IDataSourceConfiguration>();
			dataSourceConfiguration.Setup(s => s.DataSourceName)
				.Returns("TestDb");
			dataSourceConfiguration.Setup(s => s.DataSourceType)
				.Returns("SqlServer");
			dataSourceConfiguration.Setup(s => s.DataSourceEnum)
				.Returns(DataSourceTypes.SqlServer);
			dataSourceConfiguration.Setup(s => s.Schema)
				.Returns("dbo");
			dataSourceConfiguration.Setup(s => s.DataSource)
				.Returns("TestDb");

			var dataSourceContext = new Mock<IDataSourceContext>();
			dataSourceContext.Setup(s => s.GetConnection(
				It.IsAny<bool>()))
				.Returns(Mock.Of<IDbConnection>());
			dataSourceContext.Setup(s => s.DataSourceTransaction)
				.Returns(Mock.Of<IDbTransaction>());
			dataSourceContext.Setup(s => s.CommitAsync())
				.Returns(Task.CompletedTask);
			dataSourceContext.Setup(s => s.RollbackAsync())
				.Returns(Task.CompletedTask);
			dataSourceContext.Setup(s => s.Dispose());
			dataSourceContext.Setup(s => s.ConnectionStringClean)
				.Returns("Server=localhost;Database=TestDb;User Id=testuser;Password=[Redacted];");
			dataSourceContext.Setup(s => s.DataConnectionConfiguration)
				.Returns(dataConnectionConfiguration.Object);
			dataSourceContext.Setup(s => s.DataSourceConfiguration)
				.Returns(dataSourceConfiguration.Object);

			var sqlBuilder = new Mock<ISqlBuilder>();

			var entityUpdateHandler = new Mock<IEntityUpdateHandler>();

			var testObject = new CategoriesRepositorySpy(
				loggerFactory.Object,
				dataSourceContext.Object,
				sqlBuilder.Object,
				entityUpdateHandler.Object);

			return testObject;
		}

		/// <summary>
		/// A spy for the CustomersRepository class, which allows for testing and verifying interactions with the repository.
		/// </summary>
		/// <returns></returns>
		public static CustomersRepositorySpy GetCustomersRepositorySpy()
		{
			var loggerFactory = new Mock<ILoggerFactory>();

			var dataConnectionConfiguration = new Mock<IDataConnectionConfiguration>();
			dataConnectionConfiguration.Setup(s => s.ConnectTimeout)
				.Returns(30);
			dataConnectionConfiguration.Setup(s => s.DataSourceName)
				.Returns("TestDb");
			dataConnectionConfiguration.Setup(s => s.Host)
				.Returns("localhost");
			dataConnectionConfiguration.Setup(s => s.Port);
			dataConnectionConfiguration.Setup(s => s.UserName)
				.Returns("myUserName");
			dataConnectionConfiguration.Setup(s => s.Password)
				.Returns("myUserPass");

			var dataSourceConfiguration = new Mock<IDataSourceConfiguration>();
			dataSourceConfiguration.Setup(s => s.DataSourceName)
				.Returns("TestDb");
			dataSourceConfiguration.Setup(s => s.DataSourceType)
				.Returns("SqlServer");
			dataSourceConfiguration.Setup(s => s.DataSourceEnum)
				.Returns(DataSourceTypes.SqlServer);
			dataSourceConfiguration.Setup(s => s.Schema)
				.Returns("dbo");
			dataSourceConfiguration.Setup(s => s.DataSource)
				.Returns("TestDb");

			var dataSourceContext = new Mock<IDataSourceContext>();
			dataSourceContext.Setup(s => s.GetConnection(
				It.IsAny<bool>()))
				.Returns(Mock.Of<IDbConnection>());
			dataSourceContext.Setup(s => s.DataSourceTransaction)
				.Returns(Mock.Of<IDbTransaction>());
			dataSourceContext.Setup(s => s.CommitAsync())
				.Returns(Task.CompletedTask);
			dataSourceContext.Setup(s => s.RollbackAsync())
				.Returns(Task.CompletedTask);
			dataSourceContext.Setup(s => s.Dispose());
			dataSourceContext.Setup(s => s.ConnectionStringClean)
				.Returns("Server=localhost;Database=TestDb;User Id=testuser;Password=[Redacted];");
			dataSourceContext.Setup(s => s.DataConnectionConfiguration)
				.Returns(dataConnectionConfiguration.Object);
			dataSourceContext.Setup(s => s.DataSourceConfiguration)
				.Returns(dataSourceConfiguration.Object);

			var sqlBuilder = new Mock<ISqlBuilder>();

			var entityUpdateHandler = new Mock<IEntityUpdateHandler>();

			var testObject = new CustomersRepositorySpy(
				loggerFactory.Object,
				dataSourceContext.Object,
				sqlBuilder.Object,
				entityUpdateHandler.Object);

			return testObject;
		}

		/// <summary>
		/// A spy for the EmployeesRepository class, which allows for testing and verifying interactions with the repository.
		/// </summary>
		/// <returns></returns>
		public static EmployeesRepositorySpy GetEmployeesRepositorySpy()
		{
			var loggerFactory = new Mock<ILoggerFactory>();

			var dataConnectionConfiguration = new Mock<IDataConnectionConfiguration>();
			dataConnectionConfiguration.Setup(s => s.ConnectTimeout)
				.Returns(30);
			dataConnectionConfiguration.Setup(s => s.DataSourceName)
				.Returns("TestDb");
			dataConnectionConfiguration.Setup(s => s.Host)
				.Returns("localhost");
			dataConnectionConfiguration.Setup(s => s.Port);
			dataConnectionConfiguration.Setup(s => s.UserName)
				.Returns("myUserName");
			dataConnectionConfiguration.Setup(s => s.Password)
				.Returns("myUserPass");

			var dataSourceConfiguration = new Mock<IDataSourceConfiguration>();
			dataSourceConfiguration.Setup(s => s.DataSourceName)
				.Returns("TestDb");
			dataSourceConfiguration.Setup(s => s.DataSourceType)
				.Returns("SqlServer");
			dataSourceConfiguration.Setup(s => s.DataSourceEnum)
				.Returns(DataSourceTypes.SqlServer);
			dataSourceConfiguration.Setup(s => s.Schema)
				.Returns("dbo");
			dataSourceConfiguration.Setup(s => s.DataSource)
				.Returns("TestDb");

			var dataSourceContext = new Mock<IDataSourceContext>();
			dataSourceContext.Setup(s => s.GetConnection(
				It.IsAny<bool>()))
				.Returns(Mock.Of<IDbConnection>());
			dataSourceContext.Setup(s => s.DataSourceTransaction)
				.Returns(Mock.Of<IDbTransaction>());
			dataSourceContext.Setup(s => s.CommitAsync())
				.Returns(Task.CompletedTask);
			dataSourceContext.Setup(s => s.RollbackAsync())
				.Returns(Task.CompletedTask);
			dataSourceContext.Setup(s => s.Dispose());
			dataSourceContext.Setup(s => s.ConnectionStringClean)
				.Returns("Server=localhost;Database=TestDb;User Id=testuser;Password=[Redacted];");
			dataSourceContext.Setup(s => s.DataConnectionConfiguration)
				.Returns(dataConnectionConfiguration.Object);
			dataSourceContext.Setup(s => s.DataSourceConfiguration)
				.Returns(dataSourceConfiguration.Object);

			var sqlBuilder = new Mock<ISqlBuilder>();

			var entityUpdateHandler = new Mock<IEntityUpdateHandler>();

			var testObject = new EmployeesRepositorySpy(
				loggerFactory.Object,
				dataSourceContext.Object,
				sqlBuilder.Object,
				entityUpdateHandler.Object);

			return testObject;
		}

		/// <summary>
		/// A spy for the OrderDetailsRepository class, which allows for testing and verifying interactions with the repository.
		/// </summary>
		/// <returns></returns>
		public static OrderDetailsRepositorySpy GetOrderDetailsRepositorySpy()
		{
			var loggerFactory = new Mock<ILoggerFactory>();

			var dataConnectionConfiguration = new Mock<IDataConnectionConfiguration>();
			dataConnectionConfiguration.Setup(s => s.ConnectTimeout)
				.Returns(30);
			dataConnectionConfiguration.Setup(s => s.DataSourceName)
				.Returns("TestDb");
			dataConnectionConfiguration.Setup(s => s.Host)
				.Returns("localhost");
			dataConnectionConfiguration.Setup(s => s.Port);
			dataConnectionConfiguration.Setup(s => s.UserName)
				.Returns("myUserName");
			dataConnectionConfiguration.Setup(s => s.Password)
				.Returns("myUserPass");

			var dataSourceConfiguration = new Mock<IDataSourceConfiguration>();
			dataSourceConfiguration.Setup(s => s.DataSourceName)
				.Returns("TestDb");
			dataSourceConfiguration.Setup(s => s.DataSourceType)
				.Returns("SqlServer");
			dataSourceConfiguration.Setup(s => s.DataSourceEnum)
				.Returns(DataSourceTypes.SqlServer);
			dataSourceConfiguration.Setup(s => s.Schema)
				.Returns("dbo");
			dataSourceConfiguration.Setup(s => s.DataSource)
				.Returns("TestDb");

			var dataSourceContext = new Mock<IDataSourceContext>();
			dataSourceContext.Setup(s => s.GetConnection(
				It.IsAny<bool>()))
				.Returns(Mock.Of<IDbConnection>());
			dataSourceContext.Setup(s => s.DataSourceTransaction)
				.Returns(Mock.Of<IDbTransaction>());
			dataSourceContext.Setup(s => s.CommitAsync())
				.Returns(Task.CompletedTask);
			dataSourceContext.Setup(s => s.RollbackAsync())
				.Returns(Task.CompletedTask);
			dataSourceContext.Setup(s => s.Dispose());
			dataSourceContext.Setup(s => s.ConnectionStringClean)
				.Returns("Server=localhost;Database=TestDb;User Id=testuser;Password=[Redacted];");
			dataSourceContext.Setup(s => s.DataConnectionConfiguration)
				.Returns(dataConnectionConfiguration.Object);
			dataSourceContext.Setup(s => s.DataSourceConfiguration)
				.Returns(dataSourceConfiguration.Object);

			var sqlBuilder = new Mock<ISqlBuilder>();

			var entityUpdateHandler = new Mock<IEntityUpdateHandler>();

			var testObject = new OrderDetailsRepositorySpy(
				loggerFactory.Object,
				dataSourceContext.Object,
				sqlBuilder.Object,
				entityUpdateHandler.Object);

			return testObject;
		}

		/// <summary>
		/// A spy for the OrdersRepository class, which allows for testing and verifying interactions with the repository.
		/// </summary>
		/// <returns></returns>
		public static OrdersRepositorySpy GetOrdersRepositorySpy()
		{
			var loggerFactory = new Mock<ILoggerFactory>();

			var dataConnectionConfiguration = new Mock<IDataConnectionConfiguration>();
			dataConnectionConfiguration.Setup(s => s.ConnectTimeout)
				.Returns(30);
			dataConnectionConfiguration.Setup(s => s.DataSourceName)
				.Returns("TestDb");
			dataConnectionConfiguration.Setup(s => s.Host)
				.Returns("localhost");
			dataConnectionConfiguration.Setup(s => s.Port);
			dataConnectionConfiguration.Setup(s => s.UserName)
				.Returns("myUserName");
			dataConnectionConfiguration.Setup(s => s.Password)
				.Returns("myUserPass");

			var dataSourceConfiguration = new Mock<IDataSourceConfiguration>();
			dataSourceConfiguration.Setup(s => s.DataSourceName)
				.Returns("TestDb");
			dataSourceConfiguration.Setup(s => s.DataSourceType)
				.Returns("SqlServer");
			dataSourceConfiguration.Setup(s => s.DataSourceEnum)
				.Returns(DataSourceTypes.SqlServer);
			dataSourceConfiguration.Setup(s => s.Schema)
				.Returns("dbo");
			dataSourceConfiguration.Setup(s => s.DataSource)
				.Returns("TestDb");

			var dataSourceContext = new Mock<IDataSourceContext>();
			dataSourceContext.Setup(s => s.GetConnection(
				It.IsAny<bool>()))
				.Returns(Mock.Of<IDbConnection>());
			dataSourceContext.Setup(s => s.DataSourceTransaction)
				.Returns(Mock.Of<IDbTransaction>());
			dataSourceContext.Setup(s => s.CommitAsync())
				.Returns(Task.CompletedTask);
			dataSourceContext.Setup(s => s.RollbackAsync())
				.Returns(Task.CompletedTask);
			dataSourceContext.Setup(s => s.Dispose());
			dataSourceContext.Setup(s => s.ConnectionStringClean)
				.Returns("Server=localhost;Database=TestDb;User Id=testuser;Password=[Redacted];");
			dataSourceContext.Setup(s => s.DataConnectionConfiguration)
				.Returns(dataConnectionConfiguration.Object);
			dataSourceContext.Setup(s => s.DataSourceConfiguration)
				.Returns(dataSourceConfiguration.Object);

			var sqlBuilder = new Mock<ISqlBuilder>();

			var entityUpdateHandler = new Mock<IEntityUpdateHandler>();

			var testObject = new OrdersRepositorySpy(
				loggerFactory.Object,
				dataSourceContext.Object,
				sqlBuilder.Object,
				entityUpdateHandler.Object);

			return testObject;
		}

		/// <summary>
		/// A spy for the ProductsRepository class, which allows for testing and verifying interactions with the repository.
		/// </summary>
		/// <returns></returns>
		public static ProductsRepositorySpy GetProductsRepositorySpy()
		{
			var loggerFactory = new Mock<ILoggerFactory>();

			var dataConnectionConfiguration = new Mock<IDataConnectionConfiguration>();
			dataConnectionConfiguration.Setup(s => s.ConnectTimeout)
				.Returns(30);
			dataConnectionConfiguration.Setup(s => s.DataSourceName)
				.Returns("TestDb");
			dataConnectionConfiguration.Setup(s => s.Host)
				.Returns("localhost");
			dataConnectionConfiguration.Setup(s => s.Port);
			dataConnectionConfiguration.Setup(s => s.UserName)
				.Returns("myUserName");
			dataConnectionConfiguration.Setup(s => s.Password)
				.Returns("myUserPass");

			var dataSourceConfiguration = new Mock<IDataSourceConfiguration>();
			dataSourceConfiguration.Setup(s => s.DataSourceName)
				.Returns("TestDb");
			dataSourceConfiguration.Setup(s => s.DataSourceType)
				.Returns("SqlServer");
			dataSourceConfiguration.Setup(s => s.DataSourceEnum)
				.Returns(DataSourceTypes.SqlServer);
			dataSourceConfiguration.Setup(s => s.Schema)
				.Returns("dbo");
			dataSourceConfiguration.Setup(s => s.DataSource)
				.Returns("TestDb");

			var dataSourceContext = new Mock<IDataSourceContext>();
			dataSourceContext.Setup(s => s.GetConnection(
				It.IsAny<bool>()))
				.Returns(Mock.Of<IDbConnection>());
			dataSourceContext.Setup(s => s.DataSourceTransaction)
				.Returns(Mock.Of<IDbTransaction>());
			dataSourceContext.Setup(s => s.CommitAsync())
				.Returns(Task.CompletedTask);
			dataSourceContext.Setup(s => s.RollbackAsync())
				.Returns(Task.CompletedTask);
			dataSourceContext.Setup(s => s.Dispose());
			dataSourceContext.Setup(s => s.ConnectionStringClean)
				.Returns("Server=localhost;Database=TestDb;User Id=testuser;Password=[Redacted];");
			dataSourceContext.Setup(s => s.DataConnectionConfiguration)
				.Returns(dataConnectionConfiguration.Object);
			dataSourceContext.Setup(s => s.DataSourceConfiguration)
				.Returns(dataSourceConfiguration.Object);

			var sqlBuilder = new Mock<ISqlBuilder>();

			var entityUpdateHandler = new Mock<IEntityUpdateHandler>();

			var testObject = new ProductsRepositorySpy(
				loggerFactory.Object,
				dataSourceContext.Object,
				sqlBuilder.Object,
				entityUpdateHandler.Object);

			return testObject;
		}

		/// <summary>
		/// A spy for the ShippersRepository class, which allows for testing and verifying interactions with the repository.
		/// </summary>
		/// <returns></returns>
		public static ShippersRepositorySpy GetShippersRepositorySpy()
		{
			var loggerFactory = new Mock<ILoggerFactory>();

			var dataConnectionConfiguration = new Mock<IDataConnectionConfiguration>();
			dataConnectionConfiguration.Setup(s => s.ConnectTimeout)
				.Returns(30);
			dataConnectionConfiguration.Setup(s => s.DataSourceName)
				.Returns("TestDb");
			dataConnectionConfiguration.Setup(s => s.Host)
				.Returns("localhost");
			dataConnectionConfiguration.Setup(s => s.Port);
			dataConnectionConfiguration.Setup(s => s.UserName)
				.Returns("myUserName");
			dataConnectionConfiguration.Setup(s => s.Password)
				.Returns("myUserPass");

			var dataSourceConfiguration = new Mock<IDataSourceConfiguration>();
			dataSourceConfiguration.Setup(s => s.DataSourceName)
				.Returns("TestDb");
			dataSourceConfiguration.Setup(s => s.DataSourceType)
				.Returns("SqlServer");
			dataSourceConfiguration.Setup(s => s.DataSourceEnum)
				.Returns(DataSourceTypes.SqlServer);
			dataSourceConfiguration.Setup(s => s.Schema)
				.Returns("dbo");
			dataSourceConfiguration.Setup(s => s.DataSource)
				.Returns("TestDb");

			var dataSourceContext = new Mock<IDataSourceContext>();
			dataSourceContext.Setup(s => s.GetConnection(
				It.IsAny<bool>()))
				.Returns(Mock.Of<IDbConnection>());
			dataSourceContext.Setup(s => s.DataSourceTransaction)
				.Returns(Mock.Of<IDbTransaction>());
			dataSourceContext.Setup(s => s.CommitAsync())
				.Returns(Task.CompletedTask);
			dataSourceContext.Setup(s => s.RollbackAsync())
				.Returns(Task.CompletedTask);
			dataSourceContext.Setup(s => s.Dispose());
			dataSourceContext.Setup(s => s.ConnectionStringClean)
				.Returns("Server=localhost;Database=TestDb;User Id=testuser;Password=[Redacted];");
			dataSourceContext.Setup(s => s.DataConnectionConfiguration)
				.Returns(dataConnectionConfiguration.Object);
			dataSourceContext.Setup(s => s.DataSourceConfiguration)
				.Returns(dataSourceConfiguration.Object);

			var sqlBuilder = new Mock<ISqlBuilder>();

			var entityUpdateHandler = new Mock<IEntityUpdateHandler>();

			var testObject = new ShippersRepositorySpy(
				loggerFactory.Object,
				dataSourceContext.Object,
				sqlBuilder.Object,
				entityUpdateHandler.Object);

			return testObject;
		}
	}
}
