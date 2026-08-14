using PilotApi.Domain.Contracts.Entities.Base;

namespace PilotApi.Domain.Contracts.Entities
{
	/// <summary>
	/// An interface that defines the properties of a product entity.
	/// </summary>
	public interface IProductsEntity : IEntityBase
	{
		/// <summary>
		/// Gets or sets the ID of the category that the product belongs to. 
		/// This property is nullable, meaning that a product may not belong to any category.
		/// </summary>
		int? CategoryID { get; set; }

		/// <summary>
		/// Gets or sets the ID of the product. 
		/// This property is required and must be unique for each product.
		/// </summary>
		bool Discontinued { get; set; }

		/// <summary>
		/// Gets or sets the ID of the product. 
		/// This property is required and must be unique for each product.
		/// </summary>
		int? ProductID { get; set; }

		/// <summary>
		/// Gets or sets the name of the product. 
		/// This property is required and cannot be null or empty.
		/// </summary>
		string? ProductName { get; set; }

		/// <summary>
		/// Gets or sets the quantity of the product per unit. 
		/// This property is optional and can be null if the quantity is not specified.
		/// </summary>
		string? QuantityPerUnit { get; set; }

		/// <summary>
		/// Gets or sets the reorder level of the product. 
		/// This property is required and indicates the minimum quantity of the product that should be kept in stock before reordering.
		/// </summary>
		short ReorderLevel { get; set; }

		/// <summary>
		/// Gets or sets the ID of the supplier that provides the product.
		/// </summary>
		int? SupplierID { get; set; }

		/// <summary>
		/// Gets or sets the unit price of the product.
		/// </summary>
		decimal? UnitPrice { get; set; }

		/// <summary>
		/// Gets or sets the number of units of the product that are currently in stock.
		/// </summary>
		short UnitsInStock { get; set; }

		/// <summary>
		/// Gets or sets the number of units of the product that are currently on order.
		/// </summary>
		short UnitsOnOrder { get; set; }
	}
}
