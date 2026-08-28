using PilotApi.Domain.Contracts.Entities;
using PilotApi.Repositories.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PilotApi.Repositories.Models.Entities
{
	/// <summary>
	/// An entity that represents the details of an order in the database.
	/// </summary>
	[Table("Order Details", Schema = "dbo")]
	public class OrderDetailsEntity : EntityBase, IOrderDetailsEntity
	{
		/// <summary>
		/// Gets or sets the discount applied to the order detail.
		/// </summary>
		public Single Discount { get; set; }

		/// <summary>
		/// Gets or sets the ID of the order associated with the order detail.
		/// </summary>
		[Key]
		public int OrderID { get; set; }

		/// <summary>
		/// Gets or sets the ID of the product associated with the order detail.
		/// </summary>
		[Key]
		public int ProductID { get; set; }

		/// <summary>
		/// Gets or sets the quantity of the product ordered in the order detail.
		/// </summary>
		public short Quantity { get; set; }

		/// <summary>
		/// Gets or sets the unit price of the product in the order detail.
		/// </summary>
		public decimal UnitPrice { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.Discount)}={this.Discount}, " +
				$"{nameof(this.OrderID)}={this.OrderID}, " +
				$"{nameof(this.ProductID)}={this.ProductID}, " +
				$"{nameof(this.Quantity)}={this.Quantity}, " +
				$"{nameof(this.UnitPrice)}={this.UnitPrice}";
		}
	}
}
