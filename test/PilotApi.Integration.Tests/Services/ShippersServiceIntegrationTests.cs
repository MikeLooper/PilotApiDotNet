using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using PilotApi.Integration.Tests.Testing.Doubles;
using PilotApi.Integration.Tests.Testing.Utilities;
using PilotApi.Repositories.Models.Base;
using PilotApi.Services.Handlers;
using PilotApi.Services.Services;
using System;
using System.Data;
using System.Threading.Tasks;

namespace PilotApi.Integration.Tests.Services
{
	/// <summary>
	/// Integration tests that exercise the real <see cref="ShippersService"/>, real <see cref="DataMapperHandler"/>,
	/// and the real <see cref="PilotApi.Repositories.Repositories.ShippersRepository"/> (built via
	/// <see cref="IntegrationTestDoublesUtilities.GetShippersRepository"/>) all wired together. Only the raw ADO.NET
	/// connection (<see cref="FakeDbConnection"/>) is faked, since no live database server is available in this environment.
	/// </summary>
	/// <remarks>
	/// Each test builds its own local, disposable set of dependencies (via <see cref="IntegrationTestDoublesUtilities"/>)
	/// rather than sharing state through class-level fields, to avoid any risk of cross-test data corruption.
	/// </remarks>
	[TestFixture]
	public class ShippersServiceIntegrationTests
	{
		[Test]
		public async Task ShippersService_GetAllAsync_ReturnsMappedDtos_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetShippersRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add(nameof(ShippersEntity.ShipperID), typeof(int));
			table.Columns.Add(nameof(ShippersEntity.CompanyName), typeof(string));
			table.Rows.Add(1, "Speedy Express");
			fakeConnection.NextQueryResult = table;

			var testObject = new ShippersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.GetAllAsync();

			// assert
			Assert.That(result!.Result!.Count, Is.EqualTo(1));
			Assert.That(result.Result![0].CompanyName, Is.EqualTo("Speedy Express"));
		}

		[Test]
		public async Task ShippersService_GetByIdAsync_ReturnsMappedDto_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetShippersRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add(nameof(ShippersEntity.ShipperID), typeof(int));
			table.Columns.Add(nameof(ShippersEntity.CompanyName), typeof(string));
			table.Rows.Add(2, "United Package");
			fakeConnection.NextQueryResult = table;

			var testObject = new ShippersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.GetByIdAsync(new[] { 2 });

			// assert
			Assert.That(result!.Result!.ShipperID, Is.EqualTo(2));
			Assert.That(result.Result!.CompanyName, Is.EqualTo("United Package"));
		}

		[Test]
		public void ShippersService_GetByIdAsync_WithEmptyIds_ThrowsArgumentException_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetShippersRepository(dataSourceContext, sqlBuilder);

			var testObject = new ShippersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act & assert
			Assert.ThrowsAsync<ArgumentException>(async () => await testObject.GetByIdAsync(Array.Empty<int>()));
		}

		[Test]
		public async Task ShippersService_InsertAsync_MapsDtoToEntity_AndReturnsResult_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetShippersRepository(dataSourceContext, sqlBuilder);

			var table = new DataTable();
			table.Columns.Add("Id", typeof(int));
			table.Rows.Add(4);
			fakeConnection.NextQueryResult = table;

			var testObject = new ShippersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			var dto = new ShippersDto { CompanyName = "Federal Shipping" };

			// act
			var result = await testObject.InsertAsync<int>(dto);

			// assert
			Assert.That(result!.Result, Is.EqualTo(4));

			var executedCommand = fakeConnection.ExecutedCommands[0];
			Assert.That(executedCommand.CommandText, Does.Contain("INSERT"));
			Assert.That(executedCommand.Parameters["CompanyName"], Is.EqualTo("Federal Shipping"));
		}

		[Test]
		public void ShippersService_InsertAsync_WithNullModel_ThrowsArgumentException_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetShippersRepository(dataSourceContext, sqlBuilder);

			var testObject = new ShippersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act & assert
			Assert.ThrowsAsync<ArgumentException>(async () => await testObject.InsertAsync<int>(null!));
		}

		[Test]
		public async Task ShippersService_UpdateAsync_MapsDtoToEntity_AndReturnsSuccess_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetShippersRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextNonQueryResult = 1;

			var testObject = new ShippersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			var dto = new ShippersDto { ShipperID = 1, CompanyName = "Updated Shipping" };

			// act
			var result = await testObject.UpdateAsync(dto);

			// assert
			Assert.IsTrue(result.Result);

			var executedCommand = fakeConnection.ExecutedCommands[0];
			Assert.That(executedCommand.Parameters["CompanyName"], Is.EqualTo("Updated Shipping"));
		}

		[Test]
		public async Task ShippersService_DeleteAsync_ReturnsSuccess_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetShippersRepository(dataSourceContext, sqlBuilder);

			fakeConnection.NextNonQueryResult = 1;

			var testObject = new ShippersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act
			var result = await testObject.DeleteAsync(new[] { 1 });

			// assert
			Assert.IsTrue(result.Result);
		}

		[Test]
		public void ShippersService_DeleteAsync_WithEmptyIds_ThrowsArgumentException_Test()
		{
			// arrange
			using var fakeConnection = new FakeDbConnection();
			var sqlBuilder = IntegrationTestDoublesUtilities.GetSqlBuilder();
			using var dataSourceContext = IntegrationTestDoublesUtilities.GetDataSourceContext(fakeConnection, sqlBuilder);
			var repository = IntegrationTestDoublesUtilities.GetShippersRepository(dataSourceContext, sqlBuilder);

			var testObject = new ShippersService(
				IntegrationTestDoublesUtilities.GetLoggerFactory(),
				repository,
				new DataMapperHandler());

			// act & assert
			Assert.ThrowsAsync<ArgumentException>(async () => await testObject.DeleteAsync(Array.Empty<int>()));
		}
	}
}
