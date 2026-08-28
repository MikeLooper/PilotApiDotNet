using PilotApi.Domain.Contracts.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace PilotApi.Domain.Models.Dto
{
	/// <summary>
	/// A DTO for the Order Details table in the Northwind database.
	/// </summary>
	public class OrderDetailsDto : IDtoBase
	{
		/// <summary>
		/// Gets or sets the discount applied to the order detail.
		/// </summary>
		[Required]
		public Single Discount { get; set; }

		/// <summary>
		/// Gets or sets the ID of the order associated with the order detail.
		/// </summary>
		[Required]
		public int OrderID { get; set; }

		/// <summary>
		/// Gets or sets the ID of the product associated with the order detail.
		/// </summary>
		[Required]
		public int ProductID { get; set; }

		/// <summary>
		/// Gets or sets the quantity of the product ordered in the order detail.
		/// </summary>
		[Required]
		public short Quantity { get; set; }

		/// <summary>
		/// Gets or sets the unit price of the product in the order detail.
		/// </summary>
		[Required]
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
