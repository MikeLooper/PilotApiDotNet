using PilotApi.Domain.Contracts.Entities.Base;

namespace PilotApi.Domain.Contracts.Entities
{
	/// <summary>
	/// An interface that defines the properties of a Shippers entity.
	/// </summary>
	public interface IShippersEntity : IEntityBase
	{
		/// <summary>
		/// Gets or sets the name of the shipping company.
		/// </summary>
		string? CompanyName { get; set; }

		/// <summary>
		/// Gets or sets the phone number of the shipping company.
		/// </summary>
		string? Phone { get; set; }

		/// <summary>
		/// Gets or sets the ID of the shipper.
		/// </summary>
		int? ShipperID { get; set; }
	}
}
