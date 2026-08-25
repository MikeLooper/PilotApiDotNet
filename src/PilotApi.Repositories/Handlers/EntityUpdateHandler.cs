using PilotApi.Domain.Contracts.Entities.Base;
using PilotApi.Repositories.Models.Base;
using PilotApi.Repositories.Models.Entities;
using System;
using System.Collections.Generic;

namespace PilotApi.Repositories.Handlers
{
	/// <inheritdoc/>
	public class EntityUpdateHandler : IEntityUpdateHandler
	{
		/// <inheritdoc/>
		public TEntity? Update<TEntity>(TEntity? entity, Dictionary<string, object> nextIds) where TEntity : EntityBase
		{
			if (entity == null)
			{
				return default;
			}

			if (entity is CategoriesEntity)
			{
				entity = this.CategoriesEntityUpdater(entity, nextIds);
			}
			else if (entity is CustomersEntity)
			{
				entity = this.CustomersEntityUpdater(entity, nextIds);
			}
			else if (entity is EmployeesEntity)
			{
				entity = this.EmployeesEntityUpdater(entity, nextIds);
			}
			else if (entity is OrderDetailsEntity)
			{
				entity = this.OrderDetailsEntityUpdater(entity, nextIds);
			}
			else if (entity is OrdersEntity)
			{
				entity = this.OrdersEntityUpdater(entity, nextIds);
			}
			else if (entity is ProductsEntity)
			{
				entity = this.ProductsEntityUpdater(entity, nextIds);
			}
			else if (entity is ShippersEntity)
			{
				entity = this.ShippersEntityUpdater(entity, nextIds);
			}
			else if (entity is SuppliersEntity)
			{
				entity = this.SuppliersEntityUpdater(entity, nextIds);
			}
			else
			{
				throw new InvalidOperationException(
					$"No mapper was found for the {entity.GetType().Name} Entity object ({this.GetType().Name})");
			}

			return entity;
		}

		/// <summary>
		/// Updates an entity.
		/// </summary>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceEntity">
		/// A source Entity object.
		/// </param>
		/// <param name="nextIds">
		/// A dictionary containing the columns and values to update the entity model.
		/// </param>
		/// <returns>
		/// The resulting updated entity.
		/// </returns>
		protected TEntity? CategoriesEntityUpdater<TEntity>(TEntity? sourceEntity, Dictionary<string, object> nextIds)
			where TEntity : IEntityBase
		{
			var asCategories = sourceEntity as CategoriesEntity;
			if (asCategories != null)
			{
				foreach (var idkpv in nextIds)
				{
					if (idkpv.Key.Equals(nameof(asCategories.CategoryID), StringComparison.OrdinalIgnoreCase))
					{
						asCategories.CategoryID = (int)idkpv.Value;
					}
				}
				
				sourceEntity = (TEntity)(dynamic)asCategories;
			}

			return sourceEntity;
		}

		/// <summary>
		/// Updates an entity.
		/// </summary>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceEntity">
		/// A source Entity object.
		/// </param>
		/// <param name="nextIds">
		/// A dictionary containing the columns and values to update the entity model.
		/// </param>
		/// <returns>
		/// The resulting updated entity.
		/// </returns>
		protected TEntity? CustomersEntityUpdater<TEntity>(TEntity? sourceEntity, Dictionary<string, object> nextIds)
			where TEntity : IEntityBase
		{
			var asCustomers = sourceEntity as CustomersEntity;
			if (asCustomers != null)
			{
				foreach (var idkpv in nextIds)
				{
					if (idkpv.Key.Equals(nameof(asCustomers.CustomerID), StringComparison.OrdinalIgnoreCase))
					{
						asCustomers.CustomerID = (string)idkpv.Value;
					}
				}

				sourceEntity = (TEntity)(dynamic)asCustomers;
			}

			return sourceEntity;
		}

		/// <summary>
		/// Updates an entity.
		/// </summary>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceEntity">
		/// A source Entity object.
		/// </param>
		/// <param name="nextIds">
		/// A dictionary containing the columns and values to update the entity model.
		/// </param>
		/// <returns>
		/// The resulting updated entity.
		/// </returns>
		protected TEntity? EmployeesEntityUpdater<TEntity>(TEntity? sourceEntity, Dictionary<string, object> nextIds)
			where TEntity : IEntityBase
		{
			var asEmployees = sourceEntity as EmployeesEntity;
			if (asEmployees != null)
			{
				foreach (var idkpv in nextIds)
				{
					if (idkpv.Key.Equals(nameof(asEmployees.EmployeeID), StringComparison.OrdinalIgnoreCase))
					{
						asEmployees.EmployeeID = (int)idkpv.Value;
					}
				}

				sourceEntity = (TEntity)(dynamic)asEmployees;
			}

			return sourceEntity;
		}

		/// <summary>
		/// Updates an entity.
		/// </summary>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceEntity">
		/// A source Entity object.
		/// </param>
		/// <param name="nextIds">
		/// A dictionary containing the columns and values to update the entity model.
		/// </param>
		/// <returns>
		/// The resulting updated entity.
		/// </returns>
		protected TEntity? OrderDetailsEntityUpdater<TEntity>(TEntity? sourceEntity, Dictionary<string, object> nextIds)
			where TEntity : IEntityBase
		{
			var asOrderDetails = sourceEntity as OrderDetailsEntity;
			if (asOrderDetails != null)
			{
				foreach (var idkpv in nextIds)
				{
					if (idkpv.Key.Equals(nameof(asOrderDetails.OrderID), StringComparison.OrdinalIgnoreCase))
					{
						asOrderDetails.OrderID = (int)idkpv.Value;
					}
					else if (idkpv.Key.Equals(nameof(asOrderDetails.ProductID), StringComparison.OrdinalIgnoreCase))
					{
						asOrderDetails.ProductID = (int)idkpv.Value;
					}
				}

				sourceEntity = (TEntity)(dynamic)asOrderDetails;
			}

			return sourceEntity;
		}

		/// <summary>
		/// Updates an entity.
		/// </summary>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceEntity">
		/// A source Entity object.
		/// </param>
		/// <param name="nextIds">
		/// A dictionary containing the columns and values to update the entity model.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TEntity? OrdersEntityUpdater<TEntity>(TEntity? sourceEntity, Dictionary<string, object> nextIds)
			where TEntity : IEntityBase
		{
			var asOrders = sourceEntity as OrdersEntity;
			if (asOrders != null)
			{
				foreach (var idkpv in nextIds)
				{
					if (idkpv.Key.Equals(nameof(asOrders.CustomerID), StringComparison.OrdinalIgnoreCase))
					{
						asOrders.CustomerID = (string)idkpv.Value;
					}
					else if (idkpv.Key.Equals(nameof(asOrders.EmployeeID), StringComparison.OrdinalIgnoreCase))
					{
						asOrders.EmployeeID = (int)idkpv.Value;
					}
					else if (idkpv.Key.Equals(nameof(asOrders.OrderID), StringComparison.OrdinalIgnoreCase))
					{
						asOrders.OrderID = (int)idkpv.Value;
					}
				}

				sourceEntity = (TEntity)(dynamic)asOrders;
			}

			return sourceEntity;
		}

		/// <summary>
		/// Updates an entity.
		/// </summary>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceEntity">
		/// A source Entity object.
		/// </param>
		/// <param name="nextIds">
		/// A dictionary containing the columns and values to update the entity model.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TEntity? ProductsEntityUpdater<TEntity>(TEntity? sourceEntity, Dictionary<string, object> nextIds)
			where TEntity : IEntityBase
		{
			var asProducts = sourceEntity as ProductsEntity;
			if (asProducts != null)
			{
				foreach (var idkpv in nextIds)
				{
					if (idkpv.Key.Equals(nameof(asProducts.CategoryID), StringComparison.OrdinalIgnoreCase))
					{
						asProducts.CategoryID = (int)idkpv.Value;
					}
					else if (idkpv.Key.Equals(nameof(asProducts.ProductID), StringComparison.OrdinalIgnoreCase))
					{
						asProducts.ProductID = (int)idkpv.Value;
					}
					else if (idkpv.Key.Equals(nameof(asProducts.SupplierID), StringComparison.OrdinalIgnoreCase))
					{
						asProducts.SupplierID = (int)idkpv.Value;
					}
				}

				sourceEntity = (TEntity)(dynamic)asProducts;
			}

			return sourceEntity;
		}

		/// <summary>
		/// Updates an entity.
		/// </summary>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceEntity">
		/// A source Entity object.
		/// </param>
		/// <param name="nextIds">
		/// A dictionary containing the columns and values to update the entity model.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TEntity? ShippersEntityUpdater<TEntity>(TEntity? sourceEntity, Dictionary<string, object> nextIds)
			where TEntity : IEntityBase
		{
			var asShippers = sourceEntity as ShippersEntity;
			if (asShippers != null)
			{
				foreach (var idkpv in nextIds)
				{
					if (idkpv.Key.Equals(nameof(asShippers.ShipperID), StringComparison.OrdinalIgnoreCase))
					{
						asShippers.ShipperID = (int)idkpv.Value;
					}
				}

				sourceEntity = (TEntity)(dynamic)asShippers;
			}

			return sourceEntity;
		}

		/// <summary>
		/// Converts a Dto to an Entity. 
		/// </summary>
		/// <typeparam name="TEntity">
		/// An Entity type.
		/// </typeparam>
		/// <param name="sourceEntity">
		/// A source Entity object.
		/// </param>
		/// <param name="nextIds">
		/// A dictionary containing the columns and values to update the entity model.
		/// </param>
		/// <returns>
		/// The resulting conversion settings.
		/// </returns>
		protected TEntity? SuppliersEntityUpdater<TEntity>(TEntity? sourceEntity, Dictionary<string, object> nextIds)
			where TEntity : IEntityBase
		{
			var asSuppliers = sourceEntity as SuppliersEntity;
			if (asSuppliers != null)
			{
				foreach (var idkpv in nextIds)
				{
					if (idkpv.Key.Equals(nameof(asSuppliers.SupplierID), StringComparison.OrdinalIgnoreCase))
					{
						asSuppliers.SupplierID = (int)idkpv.Value;
					}
				}

				sourceEntity = (TEntity)(dynamic)asSuppliers;
			}

			return sourceEntity;
		}
	}
}
