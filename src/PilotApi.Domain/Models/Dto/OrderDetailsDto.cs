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
		[Range(0d, 1d)]
		public Single Discount { get; set; }

		/// <summary>
		/// Gets or sets the ID of the order associated with the order detail.
		/// </summary>
		[Required]
		[Range(1, int.MaxValue)]
		public int OrderId { get; set; }

		/// <summary>
		/// Gets or sets the ID of the product associated with the order detail.
		/// </summary>
		[Required]
		[Range(1, int.MaxValue)]
		public int ProductId { get; set; }

		/// <summary>
		/// Gets or sets the quantity of the product ordered in the order detail.
		/// </summary>
		[Required]
		[Range(1, short.MaxValue)]
		public short Quantity { get; set; }

		/// <summary>
		/// Gets or sets the unit price of the product in the order detail.
		/// </summary>
		[Required]
		[Range(typeof(decimal), "0", "922337203685477.5807", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
		public decimal UnitPrice { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.Discount)}={this.Discount}, " +
				$"{nameof(this.OrderId)}={this.OrderId}, " +
				$"{nameof(this.ProductId)}={this.ProductId}, " +
				$"{nameof(this.Quantity)}={this.Quantity}, " +
				$"{nameof(this.UnitPrice)}={this.UnitPrice}";
		}
	}
}
