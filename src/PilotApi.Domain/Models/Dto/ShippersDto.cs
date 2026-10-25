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
		[Required]
		[StringLength(40, MinimumLength = 1)]
		public string CompanyName { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the phone number of the shipping company.
		/// </summary>
		[MaxLength(24)]
		public string? Phone { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier for the shipper.
		/// </summary>
		[Range(1, int.MaxValue)]
		public int? ShipperId { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.CompanyName)}={this.CompanyName}, " +
				$"{nameof(this.Phone)}={this.Phone}, " +
				$"{nameof(this.ShipperId)}={this.ShipperId}";
		}
	}
}
