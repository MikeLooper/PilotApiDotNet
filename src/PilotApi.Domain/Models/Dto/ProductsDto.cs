using PilotApi.Domain.Contracts.Base;
using System.ComponentModel.DataAnnotations;

namespace PilotApi.Domain.Models.Dto
{
	/// <summary>
	/// A DTO for the Products table in the Northwind database.
	/// </summary>
	public class ProductsDto : IDtoBase
	{
		/// <summary>
		/// Gets or sets the category ID of the product.
		/// This property is nullable, indicating that a product may not belong to any category.
		/// </summary>
		public int? CategoryID { get; set; }

		/// <summary>
		/// Gets or sets the discontinued status of the product.
		/// </summary>
		public bool Discontinued { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier for the product.
		/// </summary>
		public int? ProductID { get; set; }

		/// <summary>
		/// Gets or sets the name of the product.
		/// </summary>
		[Required]
		public string? ProductName { get; set; }

		/// <summary>
		/// Gets or sets the quantity of the product per unit.
		/// </summary>
		public string? QuantityPerUnit { get; set; }

		/// <summary>
		/// Gets or sets the reorder level for the product.
		/// </summary>
		public short ReorderLevel { get; set; }

		/// <summary>
		/// Gets or sets the supplier ID of the product.
		/// </summary>
		public int? SupplierID { get; set; }

		/// <summary>
		/// Gets or sets the unit price of the product.
		/// </summary>
		public decimal? UnitPrice { get; set; }

		/// <summary>
		/// Gets or sets the number of units in stock for the product.
		/// </summary>
		public short UnitsInStock { get; set; }

		/// <summary>
		/// Gets or sets the number of units on order for the product.
		/// </summary>
		public short UnitsOnOrder { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.CategoryID)}={this.CategoryID}, " +
				$"{nameof(this.Discontinued)}={this.Discontinued}, " +
				$"{nameof(this.ProductID)}={this.ProductID}, " +
				$"{nameof(this.ProductName)}={this.ProductName}, " +
				$"{nameof(this.QuantityPerUnit)}={this.QuantityPerUnit}, " +
				$"{nameof(this.ReorderLevel)}={this.ReorderLevel}, " +
				$"{nameof(this.SupplierID)}={this.SupplierID}, " +
				$"{nameof(this.UnitPrice)}={this.UnitPrice}, " +
				$"{nameof(this.UnitsInStock)}={this.UnitsInStock}, " +
				$"{nameof(this.UnitsOnOrder)}={this.UnitsOnOrder}";
		}
	}
}
