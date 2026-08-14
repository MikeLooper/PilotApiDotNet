using PilotApi.Domain.Contracts.Base;
using PilotApi.Shared.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace PilotApi.Domain.Models.Dto
{
	/// <summary>
	/// A DTO for the Employees table in the Northwind database.
	/// </summary>
	public class EmployeesDto : IDtoBase
	{
		/// <summary>
		/// Gets or sets the address of the employee.
		/// </summary>
		public string? Address { get; set; }

		/// <summary>
		/// Gets or sets the birth date of the employee.
		/// </summary>
		public DateTime? BirthDate { get; set; }

		/// <summary>
		/// Gets or sets the city of the employee.
		/// </summary>
		public string? City { get; set; }

		/// <summary>
		/// Gets or sets the region of the employee.
		/// </summary>
		public string? Country { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier for the employee.
		/// </summary>
		public int? EmployeeID { get; set; }

		/// <summary>
		/// Gets or sets the extension number of the employee's phone.
		/// </summary>
		public string? Extension { get; set; }

		/// <summary>
		/// Gets or sets the first name of the employee.
		/// </summary>
		[Required]
		public string? FirstName { get; set; }

		/// <summary>
		/// Gets or sets the hire date of the employee.
		/// </summary>
		public DateTime? HireDate { get; set; }

		/// <summary>
		/// Gets or sets the home phone number of the employee.
		/// </summary>
		public string? HomePhone { get; set; }

		/// <summary>
		/// Gets or sets the last name of the employee.
		/// </summary>
		[Required]
		public string? LastName { get; set; }

		/// <summary>
		/// Gets or sets the notes about the employee.
		/// </summary>
		public string? Notes { get; set; }

		/// <summary>
		/// Gets or sets the photo of the employee as a byte array.
		/// </summary>
		public byte[]? Photo { get; set; }

		/// <summary>
		/// Gets or sets the path to the photo of the employee.
		/// </summary>
		public string? PhotoPath { get; set; }

		/// <summary>
		/// Gets or sets the postal code of the employee's address.
		/// </summary>
		public string? PostalCode { get; set; }

		/// <summary>
		/// Gets or sets the region of the employee's address.
		/// </summary>
		public string? Region { get; set; }

		/// <summary>
		/// Gets or sets the ID of the employee's manager (if any).
		/// </summary>
		public int? ReportsTo { get; set; }

		/// <summary>
		/// Gets or sets the title of the employee.
		/// </summary>
		public string? Title { get; set; }

		/// <summary>
		/// Gets or sets the title of courtesy for the employee (e.g., Mr., Mrs., Dr.).
		/// </summary>
		public string? TitleOfCourtesy { get; set; }

		/// <inheritdoc/>>
		public override string ToString()
		{
			return $"{nameof(this.Address)}={this.Address}, " +
				$"{nameof(this.BirthDate)}={this.BirthDate}, " +
				$"{nameof(this.City)}={this.City}, " +
				$"{nameof(this.Country)}={this.Country}, " +
				$"{nameof(this.EmployeeID)}={this.EmployeeID}, " +
				$"{nameof(this.Extension)}={this.Extension}, " +
				$"{nameof(this.FirstName)}={this.FirstName}, " +
				$"{nameof(this.HireDate)}={this.HireDate}, " +
				$"{nameof(this.HomePhone)}={this.HomePhone}, " +
				$"{nameof(this.LastName)}={this.LastName}, " +
				$"{nameof(this.Notes)}={this.Notes}, " +
				$"{nameof(this.Photo)}={(this.Photo == null ? StringConstants.LogNull : StringConstants.HasContents)}, " +
				$"{nameof(this.PhotoPath)}={this.PhotoPath}, " +
				$"{nameof(this.PostalCode)}={this.PostalCode}, " +
				$"{nameof(this.Region)}={this.Region}, " +
				$"{nameof(this.ReportsTo)}={this.ReportsTo}, " +
				$"{nameof(this.Title)}={this.Title}, " +
				$"{nameof(this.TitleOfCourtesy)}={this.TitleOfCourtesy}";
		}
	}
}
