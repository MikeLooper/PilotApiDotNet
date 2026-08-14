using PilotApi.Domain.Contracts.Entities.Base;
using System;

namespace PilotApi.Domain.Contracts.Entities
{
	/// <summary>
	/// An interface that defines the properties of an Orders entity.
	/// </summary>
	public interface IOrdersEntity : IEntityBase
	{
		/// <summary>
		/// Gets or sets the customer ID associated with the order.
		/// </summary>
		string? CustomerID { get; set; }

		/// <summary>
		/// Gets or sets the employee ID associated with the order.
		/// </summary>
		int? EmployeeID { get; set; }

		/// <summary>
		/// Gets or sets the freight cost associated with the order.
		/// </summary>
		decimal? Freight { get; set; }

		/// <summary>
		/// Gets or sets the date the order was placed.
		/// </summary>
		DateTime? OrderDate { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier for the order.
		/// </summary>
		int? OrderID { get; set; }

		/// <summary>
		/// Gets or sets the date the order is required to be delivered.
		/// </summary>
		 DateTime? RequiredDate { get; set; }

		/// <summary>
		/// Gets or sets the address to which the order is to be shipped.
		/// </summary>
		string? ShipAddress { get; set; }

		/// <summary>
		/// Gets or sets the city to which the order is to be shipped.
		/// </summary>
		string? ShipCity { get; set; }

		/// <summary>
		/// Gets or sets the country to which the order is to be shipped.
		/// </summary>
		string? ShipCountry { get; set; }

		/// <summary>
		/// Gets or sets the name of the shipper for the order.
		/// </summary>
		string? ShipName { get; set; }

		/// <summary>
		/// Gets or sets the date the order was shipped.
		/// </summary>
		DateTime? ShippedDate { get; set; }

		/// <summary>
		/// Gets or sets the postal code to which the order is to be shipped.
		/// </summary>
		string? ShipPostalCode { get; set; }

		/// <summary>
		/// Gets or sets the region to which the order is to be shipped.
		/// </summary>
		string? ShipRegion { get; set; }

		/// <summary>
		/// Gets or sets the ID of the shipper for the order.
		/// </summary>
		int? ShipVia { get; set; }
	}
}
