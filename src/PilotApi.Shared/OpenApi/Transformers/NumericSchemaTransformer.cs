using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Shared.OpenApi.Transformers
{
	/// <summary>
	/// Applies numeric constraints that cannot be expressed with validation attributes, matching the shared
	/// OpenAPI specification.
	/// </summary>
	/// <remarks>
	/// <list type="bullet">
	/// <item>
	/// Range-constrained int16/int32 schemas that also accept their string form (<c>[integer, string]</c>) are given
	/// a <c>minLength</c>/<c>maxLength</c> sized to the digits of their range.
	/// </item>
	/// <item>
	/// Decimal schemas map to SQL Server MONEY columns, so their string pattern is limited to 4 decimal places.
	/// </item>
	/// </list>
	/// </remarks>
	public class NumericSchemaTransformer : IOpenApiSchemaTransformer
	{
		/// <summary>
		/// The pattern for a decimal string with at most 4 decimal places (SQL Server MONEY scale).
		/// </summary>
		public const string MoneyPattern = @"^-?(?:0|[1-9]\d*)(?:\.\d{1,4})?$";

		/// <inheritdoc/>
		public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
		{
			if (IsRangedIntegerString(schema))
			{
				schema.MinLength = 1;
				schema.MaxLength = Math.Max(schema.Minimum!.Length, schema.Maximum!.Length);
			}

			var type = Nullable.GetUnderlyingType(context.JsonTypeInfo.Type) ?? context.JsonTypeInfo.Type;
			if (type == typeof(decimal) && !string.IsNullOrEmpty(schema.Pattern))
			{
				schema.Pattern = MoneyPattern;
			}

			return Task.CompletedTask;
		}

		private static bool IsRangedIntegerString(OpenApiSchema schema)
		{
			return schema.Type.HasValue
				&& schema.Type.Value.HasFlag(JsonSchemaType.Integer)
				&& schema.Type.Value.HasFlag(JsonSchemaType.String)
				&& (schema.Format == "int16" || schema.Format == "int32")
				&& !string.IsNullOrEmpty(schema.Minimum)
				&& !string.IsNullOrEmpty(schema.Maximum);
		}
	}
}
