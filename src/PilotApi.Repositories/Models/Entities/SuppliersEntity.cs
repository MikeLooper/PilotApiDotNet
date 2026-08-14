using PilotApi.Domain.Contracts.Entities;
using PilotApi.Repositories.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PilotApi.Repositories.Models.Entities
{
	/// <summary>
	/// A class representing the Suppliers entity in the database.
	/// </summary>
	[Table("Suppliers", Schema = "dbo")]
	public class SuppliersEntity : EntityBase, ISuppliersEntity
	{
		/// <summary>
		/// Gets or sets the address of the supplier.
		/// </summary>
		public string? Address { get; set; }

		/// <summary>
		/// Gets or sets the city of the supplier.
		/// </summary>
		public string? City { get; set; }

		/// <summary>
		/// Gets or sets the name of the company associated with the supplier.
		/// </summary>
		public string? CompanyName { get; set; }

		/// <summary>
		/// Gets or sets the name of the contact person for the supplier.
		/// </summary>
		public string? ContactName { get; set; }

		/// <summary>
		/// Gets or sets the title of the contact person for the supplier.
		/// </summary>
		public string? ContactTitle { get; set; }

		/// <summary>
		/// Gets or sets the country of the supplier.
		/// </summary>
		public string? Country { get; set; }

		/// <summary>
		/// Gets or sets the fax number of the supplier.
		/// </summary>
		public string? Fax { get; set; }

		/// <summary>
		/// Gets or sets the homepage URL of the supplier.
		/// </summary>
		public string? HomePage { get; set; }

		/// <summary>
		/// Gets or sets the phone number of the supplier.
		/// </summary>
		public string? Phone { get; set; }

		/// <summary>
		/// Gets or sets the postal code of the supplier.
		/// </summary>
		public string? PostalCode { get; set; }

		/// <summary>
		/// Gets or sets the region of the supplier.
		/// </summary>
		public string? Region { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier for the supplier.
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int? SupplierID { get; set; }

		/// <inheritdoc/>>
		public override string ToString()
		{
			return $"{nameof(this.Address)}={this.Address}, " +
				$"{nameof(this.City)}={this.City}, " +
				$"{nameof(this.CompanyName)}={this.CompanyName}, " +
				$"{nameof(this.ContactName)}={this.ContactName}, " +
				$"{nameof(this.ContactTitle)}={this.ContactTitle}, " +
				$"{nameof(this.Country)}={this.Country}, " +
				$"{nameof(this.Fax)}={this.Fax}, " +
				$"{nameof(this.HomePage)}={this.HomePage}, " +
				$"{nameof(this.Phone)}={this.Phone}, " +
				$"{nameof(this.PostalCode)}={this.PostalCode}, " +
				$"{nameof(this.Region)}={this.Region}, " +
				$"{nameof(this.SupplierID)}={this.SupplierID}";
		}
	}
}
