using System;

namespace PilotApi.Shared.Models
{
	/// <summary>
	/// Represents the attributes of a controller in an ASP.NET Core application.
	/// </summary>
	internal class ControllerAttributes
	{
		/// <summary>
		/// Gets or sets the name of the controller.
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// Gets or sets the full name of the controller, including its namespace.
		/// </summary>
		public string? FullName { get; set; }

		/// <summary>
		/// Gets or sets the name of the assembly in which the controller is defined.
		/// </summary>
		public string? AssemblyName { get; set; }

		/// <summary>
		///	Gets or sets the <see cref="Type"/> of the controller.
		/// </summary>
		public Type? Type { get; set; }
	}
}