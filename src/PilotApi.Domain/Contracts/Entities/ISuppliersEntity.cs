using PilotApi.Domain.Contracts.Entities.Base;

namespace PilotApi.Domain.Contracts.Entities
{
	/// <summary>
	/// An interface that defines the properties of a suppliers entity.
	/// </summary>
	public interface ISuppliersEntity : IEntityBase
	{
		/// <summary>
		/// Gets or sets the address of the supplier.
		/// </summary>
		string? Address { get; set; }

		/// <summary>
		/// Gets or sets the city of the supplier.
		/// </summary>
		string? City { get; set; }

		/// <summary>
		/// Gets or sets the company name of the supplier.
		/// </summary>
		string? CompanyName { get; set; }

		/// <summary>
		/// Gets or sets the contact name of the supplier.
		/// </summary>
		string? ContactName { get; set; }

		/// <summary>
		/// Gets or sets the contact title of the supplier.
		/// </summary>
		string? ContactTitle { get; set; }

		/// <summary>
		/// Gets or sets the country of the supplier.
		/// </summary>
		string? Country { get; set; }

		/// <summary>
		/// Gets or sets the fax number of the supplier.
		/// </summary>
		string? Fax { get; set; }

		/// <summary>
		/// Gets or sets the homepage URL of the supplier.
		/// </summary>
		string? HomePage { get; set; }

		/// <summary>
		/// Gets or sets the phone number of the supplier.
		/// </summary>
		string? Phone { get; set; }

		/// <summary>
		/// Gets or sets the postal code of the supplier.
		/// </summary>
		string? PostalCode { get; set; }

		/// <summary>
		/// Gets or sets the region of the supplier.
		/// </summary>
		string? Region { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier of the supplier.
		/// </summary>
		int? SupplierID { get; set; }
	}
}
