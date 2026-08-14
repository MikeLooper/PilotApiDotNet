using PilotApi.Domain.Contracts.Entities;
using PilotApi.Repositories.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PilotApi.Repositories.Models.Entities
{
	/// <summary>
	/// An entity that represents an order in the database.
	/// </summary>
	[Table("Orders", Schema = "dbo")]
	public class OrdersEntity : EntityBase, IOrdersEntity
	{
		/// <summary>
		/// Gets or sets the ID of the customer who placed the order.
		/// </summary>
		public string? CustomerID { get; set; }

		/// <summary>
		/// Gets or sets the ID of the employee who processed the order.
		/// </summary>
		public int? EmployeeID { get; set; }

		/// <summary>
		/// Gets or sets the freight cost of the order.
		/// </summary>
		public decimal? Freight { get; set; }

		/// <summary>
		/// Gets or sets the date the order was placed.
		/// </summary>
		public DateTime? OrderDate { get; set; }

		/// <summary>
		/// Gets or sets the ID of the order.
		/// This property is the primary key of the entity.
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int? OrderID { get; set; }

		/// <summary>
		/// Gets or sets the date the order is required to be delivered.
		/// </summary>
		public DateTime? RequiredDate { get; set; }

		/// <summary>
		/// Gets or sets the address to which the order is to be shipped.
		/// </summary>
		public string? ShipAddress { get; set; }

		/// <summary>
		/// Gets or sets the city to which the order is to be shipped.
		/// </summary>
		public string? ShipCity { get; set; }

		/// <summary>
		/// Gets or sets the country to which the order is to be shipped.
		/// </summary>
		public string? ShipCountry { get; set; }

		/// <summary>
		/// Gets or sets the name of the shipper for the order.
		/// </summary>
		public string? ShipName { get; set; }

		/// <summary>
		/// Gets or sets the date the order was shipped.
		/// </summary>
		public DateTime? ShippedDate { get; set; }

		/// <summary>
		/// Gets or sets the postal code to which the order is to be shipped.
		/// </summary>
		public string? ShipPostalCode { get; set; }

		/// <summary>
		/// Gets or sets the region to which the order is to be shipped.
		/// </summary>
		public string? ShipRegion { get; set; }

		/// <summary>
		/// Gets or sets the ID of the shipper for the order.
		/// </summary>
		public int? ShipVia { get; set; }

		/// <inheritdoc/>>
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
