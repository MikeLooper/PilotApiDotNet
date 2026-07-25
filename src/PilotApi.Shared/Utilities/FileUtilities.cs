using System.Linq;

namespace PilotApi.Shared.Utilities
{
	/// <summary>
	/// Utility methods for working with files.
	/// </summary>
	public static class FileUtilities
	{
		/// <summary>
		/// Extract and return the application version from the application entry assembly.
		/// </summary>
		/// <returns></returns>
		public static string GetApplicationVersion()
		{
			var assembly = System.Reflection.Assembly.GetEntryAssembly();
			var versionAttribute = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
				.FirstOrDefault() as System.Reflection.AssemblyInformationalVersionAttribute;
			return versionAttribute?.InformationalVersion ?? "Unknown";
		}
	}
}
