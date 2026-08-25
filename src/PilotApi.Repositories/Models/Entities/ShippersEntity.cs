using PilotApi.Domain.Contracts.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PilotApi.Repositories.Models.Base
{
	/// <summary>
	/// A class representing the Shippers entity in the database.
	/// </summary>
	[Table("Shippers", Schema = "dbo")]
	public class ShippersEntity : EntityBase, IShippersEntity
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
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
