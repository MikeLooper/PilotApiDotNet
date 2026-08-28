using PilotApi.Domain.Contracts.Base;
using System.ComponentModel.DataAnnotations;

namespace PilotApi.Domain.Models.Dto
{
	/// <summary>
	/// A DTO for the Customers table in the Northwind database.
	/// </summary>
	public class CustomersDto : IDtoBase
	{
		/// <summary>
		/// Gets or sets the address of the customer.
		/// </summary>
		public string? Address { get; set; }

		/// <summary>
		/// Gets or sets the city of the customer.
		/// </summary>
		public string? City { get; set; }

		/// <summary>
		/// Gets or sets the company name of the customer.
		/// </summary>
		[Required]
		public string? CompanyName { get; set; }

		/// <summary>
		/// Gets or sets the contact name of the customer.
		/// </summary>
		public string? ContactName { get; set; }

		/// <summary>
		/// Gets or sets the contact title of the customer.
		/// </summary>
		public string? ContactTitle { get; set; }

		/// <summary>
		/// Gets or sets the country of the customer.
		/// </summary>
		public string? Country { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier of the customer.
		/// </summary>
		[Required]
		public string? CustomerID { get; set; }

		/// <summary>
		/// Gets or sets the fax number of the customer.
		/// </summary>
		public string? Fax { get; set; }

		/// <summary>
		/// Gets or sets the phone number of the customer.
		/// </summary>
		public string? Phone { get; set; }

		/// <summary>
		/// Gets or sets the postal code of the customer.
		/// </summary>
		public string? PostalCode { get; set; }

		/// <summary>
		/// Gets or sets the region of the customer.
		/// </summary>
		public string? Region { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.Address)}={this.Address}, " +
				$"{nameof(this.City)}={this.City}, " +
				$"{nameof(this.CompanyName)}={this.CompanyName}, " +
				$"{nameof(this.ContactName)}={this.ContactName}, " +
				$"{nameof(this.ContactTitle)}={this.ContactTitle}, " +
				$"{nameof(this.Country)}={this.Country}, " +
				$"{nameof(this.CustomerID)}={this.CustomerID}, " +
				$"{nameof(this.Fax)}={this.Fax}, " +
				$"{nameof(this.Phone)}={this.Phone}, " +
				$"{nameof(this.PostalCode)}={this.PostalCode}, " +
				$"{nameof(this.Region)}={this.Region}";
		}
	}
}
