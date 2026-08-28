using PilotApi.Domain.Contracts.Entities;
using PilotApi.Repositories.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PilotApi.Repositories.Models.Entities
{
	/// <summary>
	/// An entity class that represents a product in the database.
	/// </summary>
	[Table("Products", Schema = "dbo")]
	public class ProductsEntity : EntityBase, IProductsEntity
	{
		/// <summary>
		/// Gets or sets the ID of the category that the product belongs to.
		/// This property is nullable, as a product may not belong to any category.
		/// </summary>
		public int? CategoryID { get; set; }

		/// <summary>
		/// Gets or sets the discontinued status of the product.
		/// </summary>
		public bool Discontinued { get; set; }

		/// <summary>
		/// Gets or sets the ID of the product.
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int? ProductID { get; set; }

		/// <summary>
		/// Gets or sets the name of the product.
		/// </summary>
		public string? ProductName { get; set; }

		/// <summary>
		/// Gets or sets the quantity of the product per unit.
		/// </summary>
		public string? QuantityPerUnit { get; set; }

		/// <summary>
		/// Gets or sets the reorder level of the product.
		/// </summary>
		public short ReorderLevel { get; set; }

		/// <summary>
		/// Gets or sets the ID of the supplier that provides the product.
		/// </summary>
		public int? SupplierID { get; set; }

		/// <summary>
		/// Gets or sets the unit price of the product.
		/// </summary>
		public decimal? UnitPrice { get; set; }

		/// <summary>
		/// Gets or sets the number of units of the product that are currently in stock.
		/// </summary>
		public short UnitsInStock { get; set; }

		/// <summary>
		/// Gets or sets the number of units of the product that are currently on order.
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
