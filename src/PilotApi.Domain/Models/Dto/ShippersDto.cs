using PilotApi.Domain.Contracts.Base;
using System.ComponentModel.DataAnnotations;

namespace PilotApi.Domain.Models.Dto
{
	/// <summary>
	///  A DTO for the Shippers table in the Northwind database.
	/// </summary>
	public class ShippersDto : IDtoBase
	{
		/// <summary>
		/// Gets or sets the name of the shipping company.
		/// </summary>
		public string? CompanyName { get; set; }

		/// <summary>
		/// Gets or sets the phone number of the shipping company.
		/// </summary>
		public string? Phone { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier for the shipper.
		/// </summary>
		public int? ShipperID { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.CompanyName)}={this.CompanyName}, " +
				$"{nameof(this.Phone)}={this.Phone}, " +
				$"{nameof(this.ShipperID)}={this.ShipperID}";
		}
	}
}
