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
		public string? CustomerID { get; set; }

		/// <summary>
		/// Gets or sets the employee ID associated with the order.
		/// </summary>
		public int? EmployeeID { get; set; }

		/// <summary>
		/// Gets or sets the freight cost for the order.
		/// </summary>
		public decimal? Freight { get; set; }

		/// <summary>
		/// Gets or sets the date when the order was placed.
		/// </summary>
		public DateTime? OrderDate { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier for the order.
		/// </summary>
		public int? OrderID { get; set; }

		/// <summary>
		/// Gets or sets the date when the order is required to be delivered.
		/// </summary>
		public DateTime? RequiredDate { get; set; }

		/// <summary>
		/// Gets or sets the address where the order should be shipped.
		/// </summary>
		public string? ShipAddress { get; set; }

		/// <summary>
		/// Gets or sets the city where the order should be shipped.
		/// </summary>
		public string? ShipCity { get; set; }

		/// <summary>
		/// Gets or sets the country where the order should be shipped.
		/// </summary>
		public string? ShipCountry { get; set; }

		/// <summary>
		/// Gets or sets the name of the shipper for the order.
		/// </summary>
		public string? ShipName { get; set; }

		/// <summary>
		/// Gets or sets the date when the order was shipped.
		/// </summary>
		public DateTime? ShippedDate { get; set; }

		/// <summary>
		/// Gets or sets the postal code for the shipping address of the order.
		/// </summary>
		public string? ShipPostalCode { get; set; }

		/// <summary>
		/// Gets or sets the region where the order should be shipped.
		/// </summary>
		public string? ShipRegion { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the shipper used for the order.
		/// </summary>
		public int? ShipVia { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.CustomerID)}={this.CustomerID}, " +
				$"{nameof(this.EmployeeID)}={this.EmployeeID}, " +
				$"{nameof(this.Freight)}={this.Freight}, " +
				$"{nameof(this.OrderDate)}={this.OrderDate}, " +
				$"{nameof(this.OrderID)}={this.OrderID}, " +
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
