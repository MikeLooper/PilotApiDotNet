using Microsoft.AspNetCore.Routing;

namespace PilotApi.Shared.Api.Transformers
{
	/// <summary>
	/// A transformer that converts outbound parameter values to lowercase.
	/// </summary>
	public sealed class LowercaseParameterTransformer : IOutboundParameterTransformer
	{
		/// <summary>
		/// Transforms the given value to lowercase for outbound routing.
		/// </summary>
		/// <param name="value">
		/// The value to transform.
		/// </param>
		/// <returns>
		/// The transformed lowercase string, or null if the input is null.
		/// </returns>
		public string? TransformOutbound(object? value) =>
			value?.ToString()?.ToLowerInvariant();
	}
}
