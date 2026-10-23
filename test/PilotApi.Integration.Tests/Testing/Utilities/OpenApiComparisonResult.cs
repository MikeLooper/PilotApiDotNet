using System.Collections.Generic;

namespace PilotApi.Integration.Tests.Testing.Utilities
{
	/// <summary>
	/// The result of comparing two OpenAPI documents.
	/// </summary>
	public class OpenApiComparisonResult
	{
		/// <summary>
		/// Gets the list of differences considered severe enough to fail a test (Error level).
		/// </summary>
		public List<string> Errors { get; } = new List<string>();

		/// <summary>
		/// Gets the list of differences considered informational/non-blocking (Warn level).
		/// </summary>
		public List<string> Warnings { get; } = new List<string>();

		/// <summary>
		/// Gets the list of differences considered purely informational and non-blocking (Info level).
		/// </summary>
		public List<string> Infos { get; } = new List<string>();
	}
}
