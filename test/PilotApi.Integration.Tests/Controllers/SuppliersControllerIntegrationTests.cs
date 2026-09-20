using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using PilotApi.Domain.Models.Responses;
using PilotApi.Integration.Tests.Testing.Doubles;
using PilotApi.Integration.Tests.Testing.Utilities;
using PilotApi.Repositories.Models.Entities;
using PilotApi.Services.Handlers;
using PilotApi.Services.Services;
using PilotApi.Web.Controllers.V1;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Integration.Tests.Controllers
{
	/// <summary>
	/// Integration tests that exercise the real <see cref="SuppliersController"/>, real <see cref="SuppliersService"/>,
	/// real <see cref="DataMapperHandler"/>, and the real <see cref="PilotApi.Repositories.Repositories.SuppliersRepository"/>
	/// (built via <see cref="IntegrationTestDoublesUtilities.GetSuppliersService"/>) all wired together. Only the raw
	/// ADO.NET connection (<see cref="FakeDbConnection"/>) is faked, since no live database server is available in
	/// this environment.
	/// </summary>
	/// <remarks>
	/// Each test builds its own local, disposable set of dependencies (via <see cref="IntegrationTestDoublesUtilities"/>)
	/// rather than sharing state through class-level fields, to avoid any risk of cross-test data corruption.
	/// </remarks>
	[TestFixture]
	public class SuppliersControllerIntegrationTests
	{
		[Test]
		public async Task Add_WhenRepositoryInsertSucceeds_ReturnsCreatedAtActionWithNewId_Test()
		{
			// Arrange
			var fakeConnection = new FakeDbConnection();
			var service = IntegrationTestDoublesUtilities.GetSuppliersService(fakeConnection);
			var controller = new SuppliersController(service);

			var table = new DataTable();
			table.Columns.Add("Id", typeof(int));
			table.Rows.Add(42);
			fakeConnection.NextQueryResult = table;

			var model = new SuppliersDto { CompanyName = "Exotic Liquids" };

			// Act
			var actionResult = await controller.Add(model, CancellationToken.None);

			// Assert
			var createdResult = actionResult as CreatedAtActionResult;
			Assert.That(createdResult, Is.Not.Null);
			Assert.That(createdResult!.StatusCode, Is.EqualTo(201));
			var addResponse = createdResult.Value as AddResponseInt;
			Assert.That(addResponse, Is.Not.Null);
			Assert.That(addResponse!.Id, Is.EqualTo(42));
		}

		[Test]
		public async Task Delete_WhenRepositoryReturnsError_ReturnsBadRequestWithWarningHeader_Test()
		{
			// Arrange
			var fakeConnection = new FakeDbConnection();
			var service = IntegrationTestDoublesUtilities.GetSuppliersService(fakeConnection);
			var controller = new SuppliersController(service);

			fakeConnection.NextNonQueryResult = 0;

			var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = httpContext
			};

			// Act
			var actionResult = await controller.Delete(5, CancellationToken.None);

			// Assert
			Assert.That(actionResult, Is.TypeOf<BadRequestResult>());
			Assert.That(httpContext.Response.Headers["Warning"].ToString(), Does.Contain("Zero rows were deleted"));
		}

		[Test]
		public async Task GetAll_WhenRepositoryReturnsEntities_ReturnsOkWithMappedDtos_Test()
		{
			// Arrange
			var fakeConnection = new FakeDbConnection();
			var service = IntegrationTestDoublesUtilities.GetSuppliersService(fakeConnection);
			var controller = new SuppliersController(service);

			var table = new DataTable();
			table.Columns.Add(nameof(SuppliersEntity.SupplierID), typeof(int));
			table.Columns.Add(nameof(SuppliersEntity.CompanyName), typeof(string));
			table.Rows.Add(1, "Exotic Liquids");
			table.Rows.Add(2, "New Orleans Cajun Delights");
			fakeConnection.NextQueryResult = table;

			// Act
			var actionResult = await controller.GetAll(cancellationToken: CancellationToken.None);

			// Assert
			var okResult = actionResult as OkObjectResult;
			Assert.That(okResult, Is.Not.Null);
			var dtos = okResult!.Value as IReadOnlyList<SuppliersDto>;
			Assert.That(dtos, Is.Not.Null);
			Assert.That(dtos!, Has.Count.EqualTo(2));
			Assert.That(dtos![0].CompanyName, Is.EqualTo("Exotic Liquids"));
		}

		[Test]
		public async Task GetById_WhenRepositoryReturnsEntity_ReturnsOkWithMappedDto_Test()
		{
			// Arrange
			var fakeConnection = new FakeDbConnection();
			var service = IntegrationTestDoublesUtilities.GetSuppliersService(fakeConnection);
			var controller = new SuppliersController(service);

			var table = new DataTable();
			table.Columns.Add(nameof(SuppliersEntity.SupplierID), typeof(int));
			table.Columns.Add(nameof(SuppliersEntity.CompanyName), typeof(string));
			table.Rows.Add(5, "Cooperativa de Quesos 'Las Cabras'");
			fakeConnection.NextQueryResult = table;

			// Act
			var actionResult = await controller.GetById(5, CancellationToken.None);

			// Assert
			var okResult = actionResult as OkObjectResult;
			Assert.That(okResult, Is.Not.Null);
			var dto = okResult!.Value as SuppliersDto;
			Assert.That(dto, Is.Not.Null);
			Assert.That(dto!.CompanyName, Is.EqualTo("Cooperativa de Quesos 'Las Cabras'"));
		}

		[Test]
		public async Task GetById_WhenRepositoryReturnsNull_ReturnsNotFound_Test()
		{
			// Arrange
			var fakeConnection = new FakeDbConnection();
			var service = IntegrationTestDoublesUtilities.GetSuppliersService(fakeConnection);
			var controller = new SuppliersController(service);

			fakeConnection.NextQueryResult = new DataTable();

			// Act
			var actionResult = await controller.GetById(99, CancellationToken.None);

			// Assert
			Assert.That(actionResult, Is.TypeOf<NotFoundResult>());
		}
	}
}
