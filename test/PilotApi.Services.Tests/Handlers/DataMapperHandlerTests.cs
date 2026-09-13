using NUnit.Framework;
using PilotApi.Domain.Contracts.Base;
using PilotApi.Domain.Contracts.Entities.Base;
using PilotApi.Domain.Models.Dto;
using PilotApi.Repositories.Models.Base;
using PilotApi.Repositories.Models.Entities;
using PilotApi.Services.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PilotApi.Services.Tests.Handlers
{
	[TestFixture]
	public class DataMapperHandlerTests
	{
		[Test]
		public void DataMapperHandler_DataMapperHandler_CanInstantiate_Test()
		{
			// Arrange
			Type type = typeof(DataMapperHandler);

			// Act
			object? instance = Activator.CreateInstance(type);

			// Assert
			Assert.That(type, Is.Not.Null);
			Assert.That(instance, Is.Not.Null);
		}

		[Test]
		public async Task DataMapperHandler_MapDtoToEntity_NullInputReturnsNull_Test()
		{
			// Arrange
			var handler = new DataMapperHandler();
			CategoriesDto? dto = null;

			// Act
			CategoriesEntity? result = await handler.MapDtoToEntityAsync<CategoriesDto, CategoriesEntity>(dto);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task DataMapperHandler_MapEntityToDto_NullInputReturnsNull_Test()
		{
			// Arrange
			var handler = new DataMapperHandler();
			CategoriesEntity? entity = null;

			// Act
			CategoriesDto? result = await handler.MapEntityToDtoAsync<CategoriesDto, CategoriesEntity>(entity);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task DataMapperHandler_MapEntityToDtoList_NullInputReturnsNull_Test()
		{
			// Arrange
			var handler = new DataMapperHandler();
			IEnumerable<CategoriesEntity>? entities = null;

			// Act
			IEnumerable<CategoriesDto>? result = await handler.MapEntityToDtoListAsync<CategoriesDto, CategoriesEntity>(entities);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task DataMapperHandler_MapDtoToEntity_CategoriesDtoMapsToCategoriesEntity_Test()
		{
			// Arrange
			var handler = new DataMapperHandler();
			var dto = new CategoriesDto
			{
				CategoryID = 1,
				CategoryName = "Beverages",
				Description = "Soft drinks",
				Picture = new byte[] { 1, 2, 3 }
			};

			// Act
			CategoriesEntity? result = await handler.MapDtoToEntityAsync<CategoriesDto, CategoriesEntity>(dto);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.CategoryID, Is.EqualTo(dto.CategoryID));
			Assert.That(result.CategoryName, Is.EqualTo(dto.CategoryName));
			Assert.That(result.Description, Is.EqualTo(dto.Description));
			Assert.That(result.Picture, Is.EqualTo(dto.Picture));
		}

		[Test]
		public async Task DataMapperHandler_MapEntityToDto_CustomersEntityMapsToCustomersDto_Test()
		{
			// Arrange
			var handler = new DataMapperHandler();
			var entity = new CustomersEntity
			{
				CustomerID = "ALFKI",
				CompanyName = "Alfreds",
				ContactName = "Maria",
				ContactTitle = "Owner",
				Address = "Obere Str. 57",
				City = "Berlin",
				Region = "BE",
				PostalCode = "12209",
				Country = "Germany",
				Phone = "030-0074321",
				Fax = "030-0076545"
			};

			// Act
			CustomersDto? result = await handler.MapEntityToDtoAsync<CustomersDto, CustomersEntity>(entity);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.CustomerID, Is.EqualTo(entity.CustomerID));
			Assert.That(result.CompanyName, Is.EqualTo(entity.CompanyName));
			Assert.That(result.ContactName, Is.EqualTo(entity.ContactName));
			Assert.That(result.ContactTitle, Is.EqualTo(entity.ContactTitle));
			Assert.That(result.Address, Is.EqualTo(entity.Address));
			Assert.That(result.City, Is.EqualTo(entity.City));
			Assert.That(result.Region, Is.EqualTo(entity.Region));
			Assert.That(result.PostalCode, Is.EqualTo(entity.PostalCode));
			Assert.That(result.Country, Is.EqualTo(entity.Country));
			Assert.That(result.Phone, Is.EqualTo(entity.Phone));
			Assert.That(result.Fax, Is.EqualTo(entity.Fax));
		}

		[Test]
		public async Task DataMapperHandler_MapDtoToEntity_EmployeesDtoMapsToEmployeesEntity_Test()
		{
			// Arrange
			var handler = new DataMapperHandler();
			var dto = new EmployeesDto
			{
				EmployeeID = 7,
				FirstName = "Nancy",
				LastName = "Davolio",
				Title = "Sales Representative",
				TitleOfCourtesy = "Ms.",
				BirthDate = new DateTime(1968, 12, 8),
				HireDate = new DateTime(1992, 5, 1),
				Address = "507 - 20th Ave.",
				City = "Seattle",
				Region = "WA",
				PostalCode = "98122",
				Country = "USA",
				HomePhone = "(206) 555-9857",
				Extension = "5467",
				Photo = new byte[] { 9, 8, 7 },
				Notes = "Top performer",
				ReportsTo = 2,
				PhotoPath = "http://example/photo"
			};

			// Act
			EmployeesEntity? result = await handler.MapDtoToEntityAsync<EmployeesDto, EmployeesEntity>(dto);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.EmployeeID, Is.EqualTo(dto.EmployeeID));
			Assert.That(result.FirstName, Is.EqualTo(dto.FirstName));
			Assert.That(result.LastName, Is.EqualTo(dto.LastName));
			Assert.That(result.Title, Is.EqualTo(dto.Title));
			Assert.That(result.TitleOfCourtesy, Is.EqualTo(dto.TitleOfCourtesy));
			Assert.That(result.BirthDate, Is.EqualTo(dto.BirthDate));
			Assert.That(result.HireDate, Is.EqualTo(dto.HireDate));
			Assert.That(result.Address, Is.EqualTo(dto.Address));
			Assert.That(result.City, Is.EqualTo(dto.City));
			Assert.That(result.Region, Is.EqualTo(dto.Region));
			Assert.That(result.PostalCode, Is.EqualTo(dto.PostalCode));
			Assert.That(result.Country, Is.EqualTo(dto.Country));
			Assert.That(result.HomePhone, Is.EqualTo(dto.HomePhone));
			Assert.That(result.Extension, Is.EqualTo(dto.Extension));
			Assert.That(result.Photo, Is.EqualTo(dto.Photo));
			Assert.That(result.Notes, Is.EqualTo(dto.Notes));
			Assert.That(result.ReportsTo, Is.EqualTo(dto.ReportsTo));
			Assert.That(result.PhotoPath, Is.EqualTo(dto.PhotoPath));
		}

		[Test]
		public async Task DataMapperHandler_MapEntityToDto_OrderDetailsEntityMapsToOrderDetailsDto_Test()
		{
			// Arrange
			var handler = new DataMapperHandler();
			var entity = new OrderDetailsEntity
			{
				OrderID = 11,
				ProductID = 42,
				UnitPrice = 12.50m,
				Quantity = 3,
				Discount = 0.25f
			};

			// Act
			OrderDetailsDto? result = await handler.MapEntityToDtoAsync<OrderDetailsDto, OrderDetailsEntity>(entity);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.OrderID, Is.EqualTo(entity.OrderID));
			Assert.That(result.ProductID, Is.EqualTo(entity.ProductID));
			Assert.That(result.UnitPrice, Is.EqualTo(entity.UnitPrice));
			Assert.That(result.Quantity, Is.EqualTo(entity.Quantity));
			Assert.That(result.Discount, Is.EqualTo(entity.Discount));
		}

		[Test]
		public async Task DataMapperHandler_MapDtoToEntity_OrdersDtoMapsToOrdersEntity_Test()
		{
			// Arrange
			var handler = new DataMapperHandler();
			var dto = new OrdersDto
			{
				OrderID = 10248,
				CustomerID = "VINET",
				EmployeeID = 5,
				OrderDate = new DateTime(2024, 1, 2),
				RequiredDate = new DateTime(2024, 1, 9),
				ShippedDate = new DateTime(2024, 1, 5),
				ShipVia = 3,
				Freight = 32.38m,
				ShipName = "Vins et alcools",
				ShipAddress = "59 rue de l'Abbaye",
				ShipCity = "Reims",
				ShipRegion = "RM",
				ShipPostalCode = "51100",
				ShipCountry = "France"
			};

			// Act
			OrdersEntity? result = await handler.MapDtoToEntityAsync<OrdersDto, OrdersEntity>(dto);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.OrderID, Is.EqualTo(dto.OrderID));
			Assert.That(result.CustomerID, Is.EqualTo(dto.CustomerID));
			Assert.That(result.EmployeeID, Is.EqualTo(dto.EmployeeID));
			Assert.That(result.OrderDate, Is.EqualTo(dto.OrderDate));
			Assert.That(result.RequiredDate, Is.EqualTo(dto.RequiredDate));
			Assert.That(result.ShippedDate, Is.EqualTo(dto.ShippedDate));
			Assert.That(result.ShipVia, Is.EqualTo(dto.ShipVia));
			Assert.That(result.Freight, Is.EqualTo(dto.Freight));
			Assert.That(result.ShipName, Is.EqualTo(dto.ShipName));
			Assert.That(result.ShipAddress, Is.EqualTo(dto.ShipAddress));
			Assert.That(result.ShipCity, Is.EqualTo(dto.ShipCity));
			Assert.That(result.ShipRegion, Is.EqualTo(dto.ShipRegion));
			Assert.That(result.ShipPostalCode, Is.EqualTo(dto.ShipPostalCode));
			Assert.That(result.ShipCountry, Is.EqualTo(dto.ShipCountry));
		}

		[Test]
		public async Task DataMapperHandler_MapEntityToDto_ProductsEntityMapsToProductsDto_Test()
		{
			// Arrange
			var handler = new DataMapperHandler();
			var entity = new ProductsEntity
			{
				ProductID = 1,
				ProductName = "Chai",
				SupplierID = 1,
				CategoryID = 1,
				QuantityPerUnit = "10 boxes x 20 bags",
				UnitPrice = 18.00m,
				UnitsInStock = 39,
				UnitsOnOrder = 0,
				ReorderLevel = 10,
				Discontinued = false
			};

			// Act
			ProductsDto? result = await handler.MapEntityToDtoAsync<ProductsDto, ProductsEntity>(entity);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.ProductID, Is.EqualTo(entity.ProductID));
			Assert.That(result.ProductName, Is.EqualTo(entity.ProductName));
			Assert.That(result.SupplierID, Is.EqualTo(entity.SupplierID));
			Assert.That(result.CategoryID, Is.EqualTo(entity.CategoryID));
			Assert.That(result.QuantityPerUnit, Is.EqualTo(entity.QuantityPerUnit));
			Assert.That(result.UnitPrice, Is.EqualTo(entity.UnitPrice));
			Assert.That(result.UnitsInStock, Is.EqualTo(entity.UnitsInStock));
			Assert.That(result.UnitsOnOrder, Is.EqualTo(entity.UnitsOnOrder));
			Assert.That(result.ReorderLevel, Is.EqualTo(entity.ReorderLevel));
			Assert.That(result.Discontinued, Is.EqualTo(entity.Discontinued));
		}

		[Test]
		public async Task DataMapperHandler_MapDtoToEntity_ShippersDtoMapsToShippersEntity_Test()
		{
			// Arrange
			var handler = new DataMapperHandler();
			var dto = new ShippersDto
			{
				ShipperID = 2,
				CompanyName = "United Package",
				Phone = "(503) 555-3199"
			};

			// Act
			ShippersEntity? result = await handler.MapDtoToEntityAsync<ShippersDto, ShippersEntity>(dto);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.ShipperID, Is.EqualTo(dto.ShipperID));
			Assert.That(result.CompanyName, Is.EqualTo(dto.CompanyName));
			Assert.That(result.Phone, Is.EqualTo(dto.Phone));
		}

		[Test]
		public async Task DataMapperHandler_MapEntityToDto_SuppliersEntityMapsToSuppliersDto_Test()
		{
			// Arrange
			var handler = new DataMapperHandler();
			var entity = new SuppliersEntity
			{
				SupplierID = 8,
				CompanyName = "Specialty Biscuits",
				ContactName = "Peter Wilson",
				ContactTitle = "Sales Representative",
				Address = "29 King's Way",
				City = "Manchester",
				Region = "NW",
				PostalCode = "M14 GSD",
				Country = "UK",
				Phone = "(161) 555-4448",
				Fax = "(161) 555-4449",
				HomePage = "https://example.com"
			};

			// Act
			SuppliersDto? result = await handler.MapEntityToDtoAsync<SuppliersDto, SuppliersEntity>(entity);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.SupplierID, Is.EqualTo(entity.SupplierID));
			Assert.That(result.CompanyName, Is.EqualTo(entity.CompanyName));
			Assert.That(result.ContactName, Is.EqualTo(entity.ContactName));
			Assert.That(result.ContactTitle, Is.EqualTo(entity.ContactTitle));
			Assert.That(result.Address, Is.EqualTo(entity.Address));
			Assert.That(result.City, Is.EqualTo(entity.City));
			Assert.That(result.Region, Is.EqualTo(entity.Region));
			Assert.That(result.PostalCode, Is.EqualTo(entity.PostalCode));
			Assert.That(result.Country, Is.EqualTo(entity.Country));
			Assert.That(result.Phone, Is.EqualTo(entity.Phone));
			Assert.That(result.Fax, Is.EqualTo(entity.Fax));
			Assert.That(result.HomePage, Is.EqualTo(entity.HomePage));
		}

		[Test]
		public async Task DataMapperHandler_MapEntityToDtoList_CategoriesEntitiesMapsToCategoriesDtos_Test()
		{
			// Arrange
			var handler = new DataMapperHandler();
			var entities = new List<CategoriesEntity>
			{
				new CategoriesEntity { CategoryID = 1, CategoryName = "Beverages", Description = "Drinks" },
				new CategoriesEntity { CategoryID = 2, CategoryName = "Condiments", Description = "Sauces" }
			};

			// Act
			IEnumerable<CategoriesDto>? result = await handler.MapEntityToDtoListAsync<CategoriesDto, CategoriesEntity>(entities);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.Count(), Is.EqualTo(2));
			Assert.That(result.ElementAt(0).CategoryID, Is.EqualTo(1));
			Assert.That(result.ElementAt(1).CategoryID, Is.EqualTo(2));
		}

		[Test]
		public async Task DataMapperHandler_MapDtoToEntity_UnsupportedDtoThrowsInvalidOperationException_Test()
		{
			// Arrange
			var handler = new DataMapperHandler();
			var dto = new UnsupportedDto();

			// Act
			AsyncTestDelegate action = async () => await handler.MapDtoToEntityAsync<UnsupportedDto, CategoriesEntity>(dto);

			// Assert
			Assert.That(action, Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DataMapperHandler_MapEntityToDto_UnsupportedEntityThrowsInvalidOperationException_Test()
		{
			// Arrange
			var handler = new DataMapperHandler();
			var entity = new UnsupportedEntity();

			// Act
			AsyncTestDelegate action = async () => await handler.MapEntityToDtoAsync<CategoriesDto, UnsupportedEntity>(entity);

			// Assert
			Assert.That(action, Throws.TypeOf<InvalidOperationException>());
		}
	}

	internal class UnsupportedDto : IDtoBase
	{
	}

	internal class UnsupportedEntity : IEntityBase
	{
	}
}

