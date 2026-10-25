using PilotApi.Domain.Contracts.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace PilotApi.Domain.Models.Dto
{
	/// <summary>
	/// A DTO for the Orders table in the Northwind database.
	/// </summary>
	public class OrdersDto : IDtoBase
	{
		/// <summary>
		/// Gets or sets the customer ID associated with the order.
		/// </summary>
		[StringLength(5, MinimumLength = 5)]
		public string? CustomerId { get; set; }

		/// <summary>
		/// Gets or sets the employee ID associated with the order.
		/// </summary>
		[Range(1, int.MaxValue)]
		public int? EmployeeId { get; set; }

		/// <summary>
		/// Gets or sets the freight cost for the order.
		/// </summary>
		[Range(typeof(decimal), "-922337203685477.5808", "922337203685477.5807", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
		public decimal? Freight { get; set; }

		/// <summary>
		/// Gets or sets the date when the order was placed.
		/// </summary>
		public DateTime? OrderDate { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier for the order.
		/// </summary>
		[Range(1, int.MaxValue)]
		public int? OrderId { get; set; }

		/// <summary>
		/// Gets or sets the date when the order is required to be delivered.
		/// </summary>
		public DateTime? RequiredDate { get; set; }

		/// <summary>
		/// Gets or sets the address where the order should be shipped.
		/// </summary>
		[MaxLength(60)]
		public string? ShipAddress { get; set; }

		/// <summary>
		/// Gets or sets the city where the order should be shipped.
		/// </summary>
		[MaxLength(15)]
		public string? ShipCity { get; set; }

		/// <summary>
		/// Gets or sets the country where the order should be shipped.
		/// </summary>
		[MaxLength(15)]
		public string? ShipCountry { get; set; }

		/// <summary>
		/// Gets or sets the name of the shipper for the order.
		/// </summary>
		[MaxLength(40)]
		public string? ShipName { get; set; }

		/// <summary>
		/// Gets or sets the date when the order was shipped.
		/// </summary>
		public DateTime? ShippedDate { get; set; }

		/// <summary>
		/// Gets or sets the postal code for the shipping address of the order.
		/// </summary>
		[MaxLength(10)]
		public string? ShipPostalCode { get; set; }

		/// <summary>
		/// Gets or sets the region where the order should be shipped.
		/// </summary>
		[MaxLength(15)]
		public string? ShipRegion { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the shipper used for the order.
		/// </summary>
		[Range(1, int.MaxValue)]
		public int? ShipVia { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.CustomerId)}={this.CustomerId}, " +
				$"{nameof(this.EmployeeId)}={this.EmployeeId}, " +
				$"{nameof(this.Freight)}={this.Freight}, " +
				$"{nameof(this.OrderDate)}={this.OrderDate}, " +
				$"{nameof(this.OrderId)}={this.OrderId}, " +
				$"{nameof(this.RequiredDate)}={this.RequiredDate}, " +
				$"{nameof(this.ShipAddress)}={this.ShipAddress}, " +
				$"{nameof(this.ShipCity)}={this.ShipCity}, " +
				$"{nameof(this.ShipCountry)}={this.ShipCountry}, " +
				$"{nameof(this.ShipName)}={this.ShipName}, " +
				$"{nameof(this.ShippedDate)}={this.ShippedDate}, " +
				$"{nameof(this.ShipPostalCode)}={this.ShipPostalCode}, " +
				$"{nameof(this.ShipRegion)}={this.ShipRegion}, " +
				$"{nameof(this.ShipVia)}={this.ShipVia}";
		}
	}
}
