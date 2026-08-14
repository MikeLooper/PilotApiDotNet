using PilotApi.Domain.Contracts.Entities.Base;
using System;

namespace PilotApi.Domain.Contracts.Entities
{
	/// <summary>
	/// An interface that defines the properties of an employee entity.
	/// </summary>
	public interface IEmployeesEntity : IEntityBase
	{
		/// <summary>
		/// Gets or sets the address of the employee.
		/// </summary>
		string? Address { get; set; }

		/// <summary>
		/// Gets or sets the birth date of the employee.
		/// </summary>
		DateTime? BirthDate { get; set; }

		/// <summary>
		/// Gets or sets the city of the employee.
		/// </summary>
		string? City { get; set; }

		/// <summary>
		/// Gets or sets the country of the employee.
		/// </summary>
		string? Country { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier of the employee.
		/// </summary>
		int? EmployeeID { get; set; }

		/// <summary>
		/// Gets or sets the extension number of the employee.
		/// </summary>
		string? Extension { get; set; }

		/// <summary>
		/// Gets or sets the first name of the employee.
		/// </summary>
		string? FirstName { get; set; }

		/// <summary>
		/// Gets or sets the 
		/// 
		/// </summary>
		DateTime? HireDate { get; set; }

		/// <summary>
		/// Gets or sets the home phone number of the employee.
		/// </summary>
		string? HomePhone { get; set; }

		/// <summary>
		/// Gets or sets the last name of the employee.
		/// </summary>
		string? LastName { get; set; }

		/// <summary>
		/// Gets or sets the notes about the employee.
		/// </summary>
		string? Notes { get; set; }

		/// <summary>
		/// Gets or sets the photo of the employee as a byte array.
		/// </summary>
		byte[]? Photo { get; set; }

		/// <summary>
		/// Gets or sets the path to the photo of the employee.
		/// </summary>
		string? PhotoPath { get; set; }

		/// <summary>
		/// Gets or sets the postal code of the employee's address.
		/// </summary>
		string? PostalCode { get; set; }

		/// <summary>
		/// Gets or sets the region of the employee's address.
		/// </summary>
		string? Region { get; set; }

		/// <summary>
		/// Gets or sets the ID of the employee's manager or supervisor.
		/// </summary>
		int? ReportsTo { get; set; }

		/// <summary>
		/// Gets or sets the title of the employee's job position.
		/// </summary>
		string? Title { get; set; }

		/// <summary>
		/// Gets or sets the title of courtesy for the employee (e.g., Mr., Mrs., Dr.).
		/// </summary>
		string? TitleOfCourtesy { get; set; }
	}
}
