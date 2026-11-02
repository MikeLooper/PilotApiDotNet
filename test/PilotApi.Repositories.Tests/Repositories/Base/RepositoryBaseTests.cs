using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PilotApi.Repositories.Repositories.Base;
using PilotApi.Repositories.Models.Entities;
using PilotApi.Repositories.Tests.Testing.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PilotApi.Repositories.Tests.Repositories.Base
{
	[TestFixture]
	public class RepositoryBaseTests : TestBase
	{
		[Test]
		public void RepositoryBase_IsAbstractClass_Test()
		{
			// Arrange
			var repositoryBaseType = typeof(RepositoryBase<>);

			// Assert
			Assert.That(repositoryBaseType.IsAbstract, Is.True);
		}

		[Test]
		public void RepositoryBase_GenericTypeHasConstraints_Test()
		{
			// Arrange
			var repositoryBaseType = typeof(RepositoryBase<>);
			var genericArgs = repositoryBaseType.GetGenericArguments();

			// Assert
			Assert.That(genericArgs.Length, Is.EqualTo(1));
			Assert.That(genericArgs[0].Name, Is.EqualTo("TEntity"));
		}

		[Test]
		public void RepositoryBase_CanBeUsedWithEntityBaseTypes_Test()
		{
			// Verify that concrete repository implementations exist
			var categoriesRepoType = typeof(PilotApi.Repositories.Repositories.CategoriesRepository);
			Assert.That(categoriesRepoType, Is.Not.Null);

			var customersRepoType = typeof(PilotApi.Repositories.Repositories.CustomersRepository);
			Assert.That(customersRepoType, Is.Not.Null);

			var employeesRepoType = typeof(PilotApi.Repositories.Repositories.EmployeesRepository);
			Assert.That(employeesRepoType, Is.Not.Null);

			var ordersRepoType = typeof(PilotApi.Repositories.Repositories.OrdersRepository);
			Assert.That(ordersRepoType, Is.Not.Null);

			var orderDetailsRepoType = typeof(PilotApi.Repositories.Repositories.OrderDetailsRepository);
			Assert.That(orderDetailsRepoType, Is.Not.Null);

			var productsRepoType = typeof(PilotApi.Repositories.Repositories.ProductsRepository);
			Assert.That(productsRepoType, Is.Not.Null);

			var shippersRepoType = typeof(PilotApi.Repositories.Repositories.ShippersRepository);
			Assert.That(shippersRepoType, Is.Not.Null);

			var suppliersRepoType = typeof(PilotApi.Repositories.Repositories.SuppliersRepository);
			Assert.That(suppliersRepoType, Is.Not.Null);
		}

		[Test]
		public void RepositoryBase_HasAbstractProperties_Test()
		{
			// Arrange
			var repositoryBaseType = typeof(RepositoryBase<>);
			var allProperties = repositoryBaseType.GetProperties(
				System.Reflection.BindingFlags.Public |
				System.Reflection.BindingFlags.NonPublic |
				System.Reflection.BindingFlags.Instance);

			// Assert
			Assert.That(allProperties.Length, Is.GreaterThan(0));
		}

		[Test]
		public void RepositoryBase_LogDatabaseCallStart_LogsStartWithRepositoryMethodAndTable_Test()
		{
			// Arrange
			var logger = new Mock<ILogger>();
			var testObject = RepositoryDoublesUtilities.GetCategoriesRepositorySpy(logger.Object);

			// Act
			var stopwatch = testObject.LogDatabaseCallStartSpy("GetAllAsync");

			// Assert
			Assert.That(stopwatch.IsRunning, Is.True);
			var state = GetSingleLoggedState(logger, LogLevel.Information);
			Assert.That(state.ToString(), Does.StartWith("Database call start:"));
			Assert.That(GetValue(state, "Repository"), Is.EqualTo("CategoriesRepositorySpy"));
			Assert.That(GetValue(state, "DatabaseMethod"), Is.EqualTo("GetAllAsync"));
			Assert.That(GetValue(state, "TableName"), Is.EqualTo(testObject.TableNameSpy));
		}

		[Test]
		public void RepositoryBase_LogDatabaseCallEnd_StopsStopwatchAndLogsEndWithElapsedTime_Test()
		{
			// Arrange
			var logger = new Mock<ILogger>();
			var testObject = RepositoryDoublesUtilities.GetCategoriesRepositorySpy(logger.Object);
			var stopwatch = Stopwatch.StartNew();

			// Act
			testObject.LogDatabaseCallEndSpy("GetAllAsync", stopwatch);

			// Assert
			Assert.That(stopwatch.IsRunning, Is.False);
			var state = GetSingleLoggedState(logger, LogLevel.Information);
			Assert.That(state.ToString(), Does.StartWith("Database call end:"));
			Assert.That(GetValue(state, "DatabaseMethod"), Is.EqualTo("GetAllAsync"));
			Assert.That(GetValue(state, "ElapsedMilliseconds"), Is.EqualTo(stopwatch.ElapsedMilliseconds));
		}

		private static IReadOnlyList<KeyValuePair<string, object?>> GetSingleLoggedState(Mock<ILogger> logger, LogLevel logLevel)
		{
			var invocation = logger.Invocations.Single(i => i.Method.Name == nameof(ILogger.Log));
			Assert.That(invocation.Arguments[0], Is.EqualTo(logLevel));

			return (IReadOnlyList<KeyValuePair<string, object?>>)invocation.Arguments[2];
		}

		private static object? GetValue(IReadOnlyList<KeyValuePair<string, object?>> state, string key)
		{
			return state.Single(kvp => kvp.Key == key).Value;
		}
	}
}
