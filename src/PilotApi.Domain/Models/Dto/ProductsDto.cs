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
		[Range(1, int.MaxValue)]
		public int? CategoryId { get; set; }

		/// <summary>
		/// Gets or sets the discontinued status of the product.
		/// </summary>
		public bool Discontinued { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier for the product.
		/// </summary>
		[Range(1, int.MaxValue)]
		public int? ProductId { get; set; }

		/// <summary>
		/// Gets or sets the name of the product.
		/// </summary>
		[Required]
		[StringLength(40, MinimumLength = 1)]
		public string ProductName { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the quantity of the product per unit.
		/// </summary>
		[MaxLength(20)]
		public string? QuantityPerUnit { get; set; }

		/// <summary>
		/// Gets or sets the reorder level for the product.
		/// </summary>
		[Range(0, short.MaxValue)]
		public short? ReorderLevel { get; set; }

		/// <summary>
		/// Gets or sets the supplier ID of the product.
		/// </summary>
		[Range(1, int.MaxValue)]
		public int? SupplierId { get; set; }

		/// <summary>
		/// Gets or sets the unit price of the product.
		/// </summary>
		[Range(typeof(decimal), "0", "922337203685477.5807", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
		public decimal? UnitPrice { get; set; }

		/// <summary>
		/// Gets or sets the number of units in stock for the product.
		/// </summary>
		[Range(0, short.MaxValue)]
		public short? UnitsInStock { get; set; }

		/// <summary>
		/// Gets or sets the number of units on order for the product.
		/// </summary>
		[Range(0, short.MaxValue)]
		public short? UnitsOnOrder { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.CategoryId)}={this.CategoryId}, " +
				$"{nameof(this.Discontinued)}={this.Discontinued}, " +
				$"{nameof(this.ProductId)}={this.ProductId}, " +
				$"{nameof(this.ProductName)}={this.ProductName}, " +
				$"{nameof(this.QuantityPerUnit)}={this.QuantityPerUnit}, " +
				$"{nameof(this.ReorderLevel)}={this.ReorderLevel}, " +
				$"{nameof(this.SupplierId)}={this.SupplierId}, " +
				$"{nameof(this.UnitPrice)}={this.UnitPrice}, " +
				$"{nameof(this.UnitsInStock)}={this.UnitsInStock}, " +
				$"{nameof(this.UnitsOnOrder)}={this.UnitsOnOrder}";
		}
	}
}
