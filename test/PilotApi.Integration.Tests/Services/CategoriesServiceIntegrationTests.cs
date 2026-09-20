using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using PilotApi.Integration.Tests.Testing.Doubles;
using PilotApi.Integration.Tests.Testing.Utilities;
using PilotApi.Repositories.Models.Entities;
using PilotApi.Services.Handlers;
using PilotApi.Services.Services;
using System;
using System.Data;
using System.Threading.Tasks;

namespace PilotApi.Integration.Tests.Services
{
	/// <summary>
	/// Integration tests that exercise the real <see cref="CategoriesService"/>, real <see cref="DataMapperHandler"/>,
	/// and the real <see cref="PilotApi.Repositories.Repositories.CategoriesRepository"/> (built via
	/// <see cref="IntegrationTestDoublesUtilities.GetCategoriesRepository"/>) all wired together. Only the raw ADO.NET
	/// connection (<see cref="FakeDbConnection"/>) is faked, since no live database server is available in this environment.
	/// </summary>
	/// <remarks>
	/// Each test builds its own local, disposable set of dependencies (via <see cref="IntegrationTestDoublesUtilities"/>)
	/// rather than sharing state through class-level fields, to avoid any risk of cross-test data corruption.
	/// </remarks>
	[TestFixture]
	public class CategoriesServiceIntegrationTests
	{
		[Test]
		public async Task CategoriesService_GetAllAsync_ReturnsMappedDtos_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCategoriesRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add(nameof(CategoriesEntity.CategoryID), typeof(int));
			table.Columns.Add(nameof(CategoriesEntity.CategoryName), typeof(string));
			table.Rows.Add(1, "Beverages");
			fakeConnection.NextQueryResult = table;

			var testObject = new CategoriesService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.GetAllAsync();

			// assert
			Assert.IsNotNull(result);
			Assert.IsFalse(result!.IsError);
			Assert.That(result.Result!.Count, Is.EqualTo(1));
			Assert.That(result.Result![0].CategoryID, Is.EqualTo(1));
			Assert.That(result.Result![0].CategoryName, Is.EqualTo("Beverages"));
		}

		[Test]
		public async Task CategoriesService_GetByIdAsync_ReturnsMappedDto_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCategoriesRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add(nameof(CategoriesEntity.CategoryID), typeof(int));
			table.Columns.Add(nameof(CategoriesEntity.CategoryName), typeof(string));
			table.Rows.Add(2, "Condiments");
			fakeConnection.NextQueryResult = table;

			var testObject = new CategoriesService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.GetByIdAsync(new[] { 2 });

			// assert
			Assert.IsNotNull(result);
			Assert.That(result!.Result!.CategoryID, Is.EqualTo(2));
			Assert.That(result.Result!.CategoryName, Is.EqualTo("Condiments"));
		}

		[Test]
		public void CategoriesService_GetByIdAsync_WithEmptyIds_ThrowsArgumentException_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCategoriesRepository(dataSourceContext, sqlBuilder);

			var testObject = new CategoriesService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act & assert
			Assert.ThrowsAsync<ArgumentException>(async () => await testObject.GetByIdAsync(Array.Empty<int>()));
		}

		[Test]
		public async Task CategoriesService_InsertAsync_MapsDtoToEntity_AndReturnsResult_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCategoriesRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add("Id", typeof(int));
			table.Rows.Add(5);
			fakeConnection.NextQueryResult = table;

			var testObject = new CategoriesService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			var dto = new CategoriesDto { CategoryName = "Produce", Description = "Fresh vegetables" };

			// act
			var result = await testObject.InsertAsync<int>(dto);

			// assert
			Assert.IsNotNull(result);
			Assert.That(result!.Result, Is.EqualTo(5));

			var executedCommand = fakeConnection.ExecutedCommands[0];
			Assert.That(executedCommand.CommandText, Does.Contain("INSERT"));
			Assert.That(executedCommand.Parameters["CategoryName"], Is.EqualTo("Produce"));
			Assert.That(executedCommand.Parameters["Description"], Is.EqualTo("Fresh vegetables"));
		}

		[Test]
		public void CategoriesService_InsertAsync_WithNullModel_ThrowsArgumentException_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCategoriesRepository(dataSourceContext, sqlBuilder);

			var testObject = new CategoriesService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act & assert
			Assert.ThrowsAsync<ArgumentException>(async () => await testObject.InsertAsync<int>(null!));
		}

		[Test]
		public async Task CategoriesService_UpdateAsync_MapsDtoToEntity_AndReturnsSuccess_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCategoriesRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextNonQueryResult = 1;

			var testObject = new CategoriesService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			var dto = new CategoriesDto { CategoryID = 3, CategoryName = "Grains" };

			// act
			var result = await testObject.UpdateAsync(dto);

			// assert
			Assert.IsTrue(result.Result);

			var executedCommand = fakeConnection.ExecutedCommands[0];
			Assert.That(executedCommand.CommandText, Does.Contain("UPDATE"));
			Assert.That(executedCommand.Parameters["CategoryID"], Is.EqualTo(3));
		}

		[Test]
		public async Task CategoriesService_UpdateAsync_WhenRepositoryReturnsError_PropagatesErrorMessage_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCategoriesRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextNonQueryResult = 0;

			var testObject = new CategoriesService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			var dto = new CategoriesDto { CategoryID = 99, CategoryName = "Unknown" };

			// act
			var result = await testObject.UpdateAsync(dto);

			// assert
			Assert.IsFalse(result.Result);
			Assert.IsTrue(result.IsError);
			Assert.That(result.ErrorMessage, Does.Contain("Zero rows were updated"));
		}

		[Test]
		public async Task CategoriesService_DeleteAsync_ReturnsSuccess_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCategoriesRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextNonQueryResult = 1;

			var testObject = new CategoriesService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.DeleteAsync(new[] { 1 });

			// assert
			Assert.IsTrue(result.Result);
		}

		[Test]
		public void CategoriesService_DeleteAsync_WithEmptyIds_ThrowsArgumentException_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetCategoriesRepository(dataSourceContext, sqlBuilder);

			var testObject = new CategoriesService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act & assert
			Assert.ThrowsAsync<ArgumentException>(async () => await testObject.DeleteAsync(Array.Empty<int>()));
		}
	}
}
