using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PilotApi.Domain.Tests.Testing.Utilities
{
	/// <summary>
	/// Helpers for running DataAnnotations validation against DTOs, as ASP.NET Core model validation does.
	/// </summary>
	public static class ValidationTestUtilities
	{
		/// <summary>
		/// Validates all properties of the given instance and returns the names of the members that failed.
		/// </summary>
		/// <param name="instance">
		/// The object to validate.
		/// </param>
		/// <returns>
		/// The distinct member names reported by the failed validation results.
		/// </returns>
		public static IList<string> ValidateMemberNames(object instance)
		{
			var results = new List<ValidationResult>();
			Validator.TryValidateObject(instance, new ValidationContext(instance), results, validateAllProperties: true);

			return results
				.SelectMany(result => result.MemberNames)
				.Distinct()
				.ToList();
		}
	}
}
