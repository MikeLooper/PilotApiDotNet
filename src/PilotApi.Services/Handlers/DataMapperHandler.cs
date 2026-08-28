using PilotApi.Domain.Contracts.Base;
using PilotApi.Domain.Contracts.Entities.Base;
using PilotApi.Domain.Models.Dto;
using PilotApi.Repositories.Models.Base;
using PilotApi.Repositories.Models.Entities;
using PilotApi.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PilotApi.Services.Handlers
{
	/// <summary>
	/// A data mapping handler.
	/// </summary>
	public class DataMapperHandler : IDataMapperHandler
	{
		/// <inheritdoc/>
		public async Task<TEntity?> MapDtoToEntity<TDto, TEntity>(TDto? dto)
			where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			if (dto == null)
			{
				return default;
			}

			TEntity? value = default;
			if (dto is CategoriesDto)
			{
				value = this.CategoriesDtoConverter<TDto, TEntity>(dto);
			}
			else if (dto is CustomersDto)
			{
				value = this.CustomersDtoConverter<TDto, TEntity>(dto);
			}
			else if (dto is EmployeesDto)
			{
				value = this.EmployeesDtoConverter<TDto, TEntity>(dto);
			}
			else if (dto is OrderDetailsDto)
			{
				value = this.OrderDetailsDtoConverter<TDto, TEntity>(dto);
			}
			else if (dto is OrdersDto)
			{
				value = this.OrdersDtoConverter<TDto, TEntity>(dto);
			}
			else if (dto is ProductsDto)
			{
				value = this.ProductsDtoConverter<TDto, TEntity>(dto);
			}
			else if (dto is ShippersDto)
			{
				value = this.ShippersDtoConverter<TDto, TEntity>(dto);
			}
			else if (dto is SuppliersDto)
			{
				value = this.SuppliersDtoConverter<TDto, TEntity>(dto);
			}
			else
			{
				throw new InvalidOperationException($"No mapper was found for the {dto.GetType().Name} DTO object ({this.GetType().Name})");
			}

			return value;
		}

		/// <inheritdoc/>
		public async Task<TDto?> MapEntityToDto<TDto, TEntity>(TEntity? entity)
			where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			if (entity == null)
			{
				return default;
			}

			TDto? value = default;
			if (entity is CategoriesEntity)
			{
				value = CategoriesEntityConverter<TDto, TEntity>(entity);
			}
			else if (entity is CustomersEntity)
			{
				value = this.CustomersEntityConverter<TDto, TEntity>(entity);
			}
			else if (entity is EmployeesEntity)
			{
				value = this.EmployeesEntityConverter<TDto, TEntity>(entity);
			}
			else if (entity is OrderDetailsEntity)
			{
				value = this.OrderDetailsEntityConverter<TDto, TEntity>(entity);
			}
			else if (entity is OrdersEntity)
			{
				value = this.OrdersEntityConverter<TDto, TEntity>(entity);
			}
			else if (entity is ProductsEntity)
			{
				value = this.ProductsEntityConverter<TDto, TEntity>(entity);
			}
			else if (entity is ShippersEntity)
			{
				value = this.ShippersEntityConverter<TDto, TEntity>(entity);
			}
			else if (entity is SuppliersEntity)
			{
				value = this.SuppliersEntityConverter<TDto, TEntity>(entity);
			}
			else
			{
				throw new InvalidOperationException($"No mapper was found for the {entity.GetType().Name} Entity object ({this.GetType().Name})");
			}

			return value;
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<TDto>?> MapEntityToDtoList<TDto, TEntity>(IEnumerable<TEntity>? entities)
			where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			if (entities == null)
			{
				return default;
			}

			var dtoList = new List<TDto>();
			foreach (var entity in entities)
			{
				var dto = await MapEntityToDto<TDto, TEntity>(entity);
				if (dto != null)
				{
					dtoList.Add(dto);
				}
			}

			return dtoList;
		}

		/// <summary>
		/// Converts a Dto to an Entity.
		/// </summary>
		/// <param name="sourceDto">
		/// A source DTO object.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TEntity? CategoriesDtoConverter<TDto, TEntity>(TDto sourceDto)
			where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			var asCategories = sourceDto as CategoriesDto;
			if (asCategories != null)
			{
				var newEntity = new CategoriesEntity
				{
					CategoryID = asCategories.CategoryID,
					CategoryName = asCategories.CategoryName,
					Description = asCategories.Description,
					Picture = asCategories.Picture
				};

				return (TEntity)(dynamic)newEntity;
			}

			return default;
		}

		/// <summary>
		/// Converts an Entity to a Dto.
		/// </summary>
		/// <typeparam name="TDto">
		/// A DTO type.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceEntity">
		/// A source Entity object.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TDto? CategoriesEntityConverter<TDto, TEntity>(TEntity sourceEntity)
			where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			var asCategories = sourceEntity as CategoriesEntity;
			if (asCategories != null)
			{
				var newDto = new CategoriesDto
				{
					CategoryID = asCategories.CategoryID,
					CategoryName = asCategories.CategoryName,
					Description = asCategories.Description,
					Picture = asCategories.Picture
				};

				return (TDto)(dynamic)newDto;
			}

			return default;
		}

		/// <summary>
		/// Converts a Dto to an Entity.
		/// </summary>
		/// <typeparam name="TDto">
		/// A DTO type.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceDto">
		/// A source DTO object.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TEntity? CustomersDtoConverter<TDto, TEntity>(TDto sourceDto)
					where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			var asCustomers = sourceDto as CustomersDto;
			if (asCustomers != null)
			{
				var newEntity = new CustomersEntity
				{
					CustomerID = asCustomers.CustomerID,
					CompanyName = asCustomers.CompanyName,
					ContactName = asCustomers.ContactName,
					ContactTitle = asCustomers.ContactTitle,
					Address = asCustomers.Address,
					City = asCustomers.City,
					Region = asCustomers.Region,
					PostalCode = asCustomers.PostalCode,
					Country = asCustomers.Country,
					Phone = asCustomers.Phone,
					Fax = asCustomers.Fax
				};

				return (TEntity)(dynamic)newEntity;
			}

			return default;
		}

		/// <summary>
		/// Converts an Entity to a Dto.
		/// </summary>
		/// <typeparam name="TDto">
		/// A DTO type.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceEntity">
		/// A source Entity object.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TDto? CustomersEntityConverter<TDto, TEntity>(TEntity sourceEntity)
			where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			var asCustomers = sourceEntity as CustomersEntity;
			if (asCustomers != null)
			{
				var newDto = new CustomersDto
				{
					CustomerID = asCustomers.CustomerID,
					CompanyName = asCustomers.CompanyName,
					ContactName = asCustomers.ContactName,
					ContactTitle = asCustomers.ContactTitle,
					Address = asCustomers.Address,
					City = asCustomers.City,
					Region = asCustomers.Region,
					PostalCode = asCustomers.PostalCode,
					Country = asCustomers.Country,
					Phone = asCustomers.Phone,
					Fax = asCustomers.Fax
				};

				return (TDto)(dynamic)newDto;
			}

			return default;
		}

		/// <summary>
		/// Converts a Dto to an Entity.
		/// </summary>
		/// <typeparam name="TDto">
		/// A DTO type.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceDto">
		/// A source DTO object.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TEntity? EmployeesDtoConverter<TDto, TEntity>(TDto sourceDto)
					where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			var asEmployees = sourceDto as EmployeesDto;
			if (asEmployees != null)
			{
				var newEntity = new EmployeesEntity
				{
					Address = asEmployees.Address,
					BirthDate = asEmployees.BirthDate,
					City = asEmployees.City,
					Country = asEmployees.Country,
					EmployeeID = asEmployees.EmployeeID,
					Extension = asEmployees.Extension,
					FirstName = asEmployees.FirstName,
					HireDate = asEmployees.HireDate,
					HomePhone = asEmployees.HomePhone,
					LastName = asEmployees.LastName,
					Notes = asEmployees.Notes,
					Photo = asEmployees.Photo,
					PhotoPath = asEmployees.PhotoPath,
					PostalCode = asEmployees.PostalCode,
					Region = asEmployees.Region,
					ReportsTo = asEmployees.ReportsTo,
					Title = asEmployees.Title,
					TitleOfCourtesy = asEmployees.TitleOfCourtesy
				};

				return (TEntity)(dynamic)newEntity;
			}

			return default;
		}

		/// <summary>
		/// Converts an Entity to a Dto.
		/// </summary>
		/// <typeparam name="TDto">
		/// A DTO type.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceEntity">
		/// A source Entity object.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TDto? EmployeesEntityConverter<TDto, TEntity>(TEntity sourceEntity)
			where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			var asEmployees = sourceEntity as EmployeesEntity;
			if (asEmployees != null)
			{
				var newDto = new EmployeesDto
				{
					Address = asEmployees.Address,
					BirthDate = asEmployees.BirthDate,
					City = asEmployees.City,
					Country = asEmployees.Country,
					EmployeeID = asEmployees.EmployeeID,
					Extension = asEmployees.Extension,
					FirstName = asEmployees.FirstName,
					HireDate = asEmployees.HireDate,
					HomePhone = asEmployees.HomePhone,
					LastName = asEmployees.LastName,
					Notes = asEmployees.Notes,
					Photo = asEmployees.Photo,
					PhotoPath = asEmployees.PhotoPath,
					PostalCode = asEmployees.PostalCode,
					Region = asEmployees.Region,
					ReportsTo = asEmployees.ReportsTo,
					Title = asEmployees.Title,
					TitleOfCourtesy = asEmployees.TitleOfCourtesy
				};

				return (TDto)(dynamic)newDto;
			}

			return default;
		}

		/// <summary>
		/// Converts a Dto to an Entity.
		/// </summary>
		/// <typeparam name="TDto">
		/// A DTO type.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceDto">
		/// A source DTO object.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TEntity? OrderDetailsDtoConverter<TDto, TEntity>(TDto sourceDto)
					where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			var asOrderDetails = sourceDto as OrderDetailsDto;
			if (asOrderDetails != null)
			{
				var newEntity = new OrderDetailsEntity
				{
					OrderID = asOrderDetails.OrderID,
					ProductID = asOrderDetails.ProductID,
					UnitPrice = asOrderDetails.UnitPrice,
					Quantity = asOrderDetails.Quantity,
					Discount = asOrderDetails.Discount
				};

				return (TEntity)(dynamic)newEntity;
			}

			return default;
		}

		/// <summary>
		/// Converts an Entity to a Dto.
		/// </summary>
		/// <typeparam name="TDto">
		/// A DTO type.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceEntity">
		/// A source Entity object.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TDto? OrderDetailsEntityConverter<TDto, TEntity>(TEntity sourceEntity)
			where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			var asOrderDetails = sourceEntity as OrderDetailsEntity;
			if (asOrderDetails != null)
			{
				var newDto = new OrderDetailsDto
				{
					OrderID = asOrderDetails.OrderID,
					ProductID = asOrderDetails.ProductID,
					UnitPrice = asOrderDetails.UnitPrice,
					Quantity = asOrderDetails.Quantity,
					Discount = asOrderDetails.Discount
				};

				return (TDto)(dynamic)newDto;
			}

			return default;
		}

		/// <summary>
		/// Converts a Dto to an Entity.
		/// </summary>
		/// <typeparam name="TDto">
		/// A DTO type.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceDto">
		/// A source DTO object.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TEntity? OrdersDtoConverter<TDto, TEntity>(TDto sourceDto)
					where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			var asOrders = sourceDto as OrdersDto;
			if (asOrders != null)
			{
				var newEntity = new OrdersEntity
				{
					OrderID = asOrders.OrderID,
					CustomerID = asOrders.CustomerID,
					EmployeeID = asOrders.EmployeeID,
					OrderDate = asOrders.OrderDate,
					RequiredDate = asOrders.RequiredDate,
					ShippedDate = asOrders.ShippedDate,
					ShipVia = asOrders.ShipVia,
					Freight = asOrders.Freight,
					ShipName = asOrders.ShipName,
					ShipAddress = asOrders.ShipAddress,
					ShipCity = asOrders.ShipCity,
					ShipRegion = asOrders.ShipRegion,
					ShipPostalCode = asOrders.ShipPostalCode,
					ShipCountry = asOrders.ShipCountry
				};

				return (TEntity)(dynamic)newEntity;
			}

			return default;
		}

		/// <summary>
		/// Converts an Entity to a Dto.
		/// </summary>
		/// <typeparam name="TDto">
		/// A DTO type.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceEntity">
		/// A source Entity object.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TDto? OrdersEntityConverter<TDto, TEntity>(TEntity sourceEntity)
			where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			var asOrders = sourceEntity as OrdersEntity;
			if (asOrders != null)
			{
				var newDto = new OrdersDto
				{
					OrderID = asOrders.OrderID,
					CustomerID = asOrders.CustomerID,
					EmployeeID = asOrders.EmployeeID,
					OrderDate = asOrders.OrderDate,
					RequiredDate = asOrders.RequiredDate,
					ShippedDate = asOrders.ShippedDate,
					ShipVia = asOrders.ShipVia,
					Freight = asOrders.Freight,
					ShipName = asOrders.ShipName,
					ShipAddress = asOrders.ShipAddress,
					ShipCity = asOrders.ShipCity,
					ShipRegion = asOrders.ShipRegion,
					ShipPostalCode = asOrders.ShipPostalCode,
					ShipCountry = asOrders.ShipCountry
				};

				return (TDto)(dynamic)newDto;
			}

			return default;
		}

		/// <summary>
		/// Converts a Dto to an Entity.
		/// </summary>
		/// <typeparam name="TDto">
		/// A DTO type.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceDto">
		/// A source DTO object.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TEntity? ProductsDtoConverter<TDto, TEntity>(TDto sourceDto)
					where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			var asProducts = sourceDto as ProductsDto;
			if (asProducts != null)
			{
				var newEntity = new ProductsEntity
				{
					ProductID = asProducts.ProductID,
					ProductName = asProducts.ProductName,
					SupplierID = asProducts.SupplierID,
					CategoryID = asProducts.CategoryID,
					QuantityPerUnit = asProducts.QuantityPerUnit,
					UnitPrice = asProducts.UnitPrice,
					UnitsInStock = asProducts.UnitsInStock,
					UnitsOnOrder = asProducts.UnitsOnOrder,
					ReorderLevel = asProducts.ReorderLevel,
					Discontinued = asProducts.Discontinued
				};

				return (TEntity)(dynamic)newEntity;
			}

			return default;
		}

		/// <summary>
		/// Converts an Entity to a Dto.
		/// </summary>
		/// <typeparam name="TDto">
		/// A DTO type.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceEntity">
		/// A source Entity object.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TDto? ProductsEntityConverter<TDto, TEntity>(TEntity sourceEntity)
			where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			var asProducts = sourceEntity as ProductsEntity;
			if (asProducts != null)
			{
				var newDto = new ProductsDto
				{
					ProductID = asProducts.ProductID,
					ProductName = asProducts.ProductName,
					SupplierID = asProducts.SupplierID,
					CategoryID = asProducts.CategoryID,
					QuantityPerUnit = asProducts.QuantityPerUnit,
					UnitPrice = asProducts.UnitPrice,
					UnitsInStock = asProducts.UnitsInStock,
					UnitsOnOrder = asProducts.UnitsOnOrder,
					ReorderLevel = asProducts.ReorderLevel,
					Discontinued = asProducts.Discontinued
				};

				return (TDto)(dynamic)newDto;
			}

			return default;
		}

		/// <summary>
		/// Converts a Dto to an Entity.
		/// </summary>
		/// <typeparam name="TDto">
		/// A DTO type.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceDto">
		/// A source DTO object.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TEntity? ShippersDtoConverter<TDto, TEntity>(TDto sourceDto)
					where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			var asShippers = sourceDto as ShippersDto;
			if (asShippers != null)
			{
				var newEntity = new ShippersEntity
				{
					ShipperID = asShippers.ShipperID,
					CompanyName = asShippers.CompanyName,
					Phone = asShippers.Phone
				};

				return (TEntity)(dynamic)newEntity;
			}

			return default;
		}

		/// <summary>
		/// Converts an Entity to a Dto.
		/// </summary>
		/// <typeparam name="TDto">
		/// A DTO type.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceEntity">
		/// A source Entity object.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TDto? ShippersEntityConverter<TDto, TEntity>(TEntity sourceEntity)
			where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			var asShippers = sourceEntity as ShippersEntity;
			if (asShippers != null)
			{
				var newDto = new ShippersDto
				{
					ShipperID = asShippers.ShipperID,
					CompanyName = asShippers.CompanyName,
					Phone = asShippers.Phone
				};

				return (TDto)(dynamic)newDto;
			}

			return default;
		}

		/// <summary>
		/// Converts a Dto to an Entity.
		/// </summary>
		/// <typeparam name="TDto">
		/// A DTO type.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceDto">
		/// A source DTO object.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TEntity? SuppliersDtoConverter<TDto, TEntity>(TDto sourceDto)
																							where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			var asSuppliers = sourceDto as SuppliersDto;
			if (asSuppliers != null)
			{
				var newEntity = new SuppliersEntity
				{
					SupplierID = asSuppliers.SupplierID,
					CompanyName = asSuppliers.CompanyName,
					ContactName = asSuppliers.ContactName,
					ContactTitle = asSuppliers.ContactTitle,
					Address = asSuppliers.Address,
					City = asSuppliers.City,
					Region = asSuppliers.Region,
					PostalCode = asSuppliers.PostalCode,
					Country = asSuppliers.Country,
					Phone = asSuppliers.Phone,
					Fax = asSuppliers.Fax,
					HomePage = asSuppliers.HomePage
				};

				return (TEntity)(dynamic)newEntity;
			}

			return default;
		}

		/// <summary>
		/// Converts a Dto to an Entity. 
		/// </summary>
		/// <typeparam name="TDto">
		/// A DTO type.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceEntity">
		/// A source Entity object.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TDto? SuppliersEntityConverter<TDto, TEntity>(TEntity sourceEntity)
																					where TDto : IDtoBase
			where TEntity : IEntityBase
		{
			var asSuppliers = sourceEntity as SuppliersEntity;
			if (asSuppliers != null)
			{
				var newDto = new SuppliersDto
				{
					SupplierID = asSuppliers.SupplierID,
					CompanyName = asSuppliers.CompanyName,
					ContactName = asSuppliers.ContactName,
					ContactTitle = asSuppliers.ContactTitle,
					Address = asSuppliers.Address,
					City = asSuppliers.City,
					Region = asSuppliers.Region,
					PostalCode = asSuppliers.PostalCode,
					Country = asSuppliers.Country,
					Phone = asSuppliers.Phone,
					Fax = asSuppliers.Fax,
					HomePage = asSuppliers.HomePage
				};

				return (TDto)(dynamic)newDto;
			}

			return default;
		}
	}
}
