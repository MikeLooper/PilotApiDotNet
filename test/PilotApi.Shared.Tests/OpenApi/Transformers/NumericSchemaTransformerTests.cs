using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using NUnit.Framework;
using PilotApi.Shared.OpenApi.Transformers;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace PilotApi.Shared.Tests.OpenApi.Transformers
{
	[TestFixture]
	public class NumericSchemaTransformerTests : TestBase
	{
		private const string DefaultIntegerPattern = @"^-?(?:0|[1-9]\d*)$";
		private const string DefaultDecimalPattern = @"^-?(?:0|[1-9]\d*)(?:\.\d+)?$";

		[TestCase("int32", "1", "2147483647", 10)]
		[TestCase("int16", "0", "32767", 5)]
		[TestCase("int16", "-32768", "32767", 6)]
		public async Task NumericSchemaTransformer_TransformAsync_AddsStringLengthsToRangedIntegerSchema_Test(
			string format,
			string minimum,
			string maximum,
			int expectedMaxLength)
		{
			// Arrange
			var transformer = new NumericSchemaTransformer();
			var schema = new OpenApiSchema
			{
				Type = JsonSchemaType.Integer | JsonSchemaType.String,
				Format = format,
				Minimum = minimum,
				Maximum = maximum,
				Pattern = DefaultIntegerPattern
			};
			var context = CreateContext(typeof(int));

			// Act
			await transformer.TransformAsync(schema, context, default);

			// Assert
			Assert.That(schema.MinLength, Is.EqualTo(1));
			Assert.That(schema.MaxLength, Is.EqualTo(expectedMaxLength));
			Assert.That(schema.Pattern, Is.EqualTo(DefaultIntegerPattern));
		}

		[Test]
		public async Task NumericSchemaTransformer_TransformAsync_LeavesUnrangedIntegerSchemaUnchanged_Test()
		{
			// Arrange
			var transformer = new NumericSchemaTransformer();
			var schema = new OpenApiSchema
			{
				Type = JsonSchemaType.Integer | JsonSchemaType.String,
				Format = "int32",
				Pattern = DefaultIntegerPattern
			};
			var context = CreateContext(typeof(int));

			// Act
			await transformer.TransformAsync(schema, context, default);

			// Assert
			Assert.That(schema.MinLength, Is.Null);
			Assert.That(schema.MaxLength, Is.Null);
		}

		[Test]
		public async Task NumericSchemaTransformer_TransformAsync_LeavesInt64SchemaUnchanged_Test()
		{
			// Arrange
			var transformer = new NumericSchemaTransformer();
			var schema = new OpenApiSchema
			{
				Type = JsonSchemaType.Integer | JsonSchemaType.String,
				Format = "int64",
				Minimum = "1",
				Maximum = "2147483647"
			};
			var context = CreateContext(typeof(long));

			// Act
			await transformer.TransformAsync(schema, context, default);

			// Assert
			Assert.That(schema.MinLength, Is.Null);
			Assert.That(schema.MaxLength, Is.Null);
		}

		[Test]
		public async Task NumericSchemaTransformer_TransformAsync_LeavesIntegerOnlySchemaUnchanged_Test()
		{
			// Arrange
			var transformer = new NumericSchemaTransformer();
			var schema = new OpenApiSchema
			{
				Type = JsonSchemaType.Integer,
				Format = "int32",
				Minimum = "1",
				Maximum = "2147483647"
			};
			var context = CreateContext(typeof(int));

			// Act
			await transformer.TransformAsync(schema, context, default);

			// Assert
			Assert.That(schema.MinLength, Is.Null);
			Assert.That(schema.MaxLength, Is.Null);
		}

		[TestCase(typeof(decimal))]
		[TestCase(typeof(decimal?))]
		public async Task NumericSchemaTransformer_TransformAsync_LimitsDecimalPatternToMoneyScale_Test(Type type)
		{
			// Arrange
			var transformer = new NumericSchemaTransformer();
			var schema = new OpenApiSchema
			{
				Type = JsonSchemaType.Number | JsonSchemaType.String,
				Format = "double",
				Pattern = DefaultDecimalPattern
			};
			var context = CreateContext(type);

			// Act
			await transformer.TransformAsync(schema, context, default);

			// Assert
			Assert.That(schema.Pattern, Is.EqualTo(NumericSchemaTransformer.MoneyPattern));
		}

		[Test]
		public async Task NumericSchemaTransformer_TransformAsync_LeavesFloatPatternUnchanged_Test()
		{
			// Arrange
			var transformer = new NumericSchemaTransformer();
			var floatPattern = @"^-?(?:0|[1-9]\d*)(?:\.\d+)?(?:[eE][+-]?\d+)?$";
			var schema = new OpenApiSchema
			{
				Type = JsonSchemaType.Number | JsonSchemaType.String,
				Format = "float",
				Pattern = floatPattern
			};
			var context = CreateContext(typeof(float));

			// Act
			await transformer.TransformAsync(schema, context, default);

			// Assert
			Assert.That(schema.Pattern, Is.EqualTo(floatPattern));
		}

		[Test]
		public async Task NumericSchemaTransformer_TransformAsync_DoesNotAddPatternToStrictDecimal_Test()
		{
			// Arrange
			var transformer = new NumericSchemaTransformer();
			var schema = new OpenApiSchema
			{
				Type = JsonSchemaType.Number,
				Format = "double"
			};
			var context = CreateContext(typeof(decimal));

			// Act
			await transformer.TransformAsync(schema, context, default);

			// Assert
			Assert.That(schema.Pattern, Is.Null);
		}

		[TestCase("1", true)]
		[TestCase("23.45", true)]
		[TestCase("99.0000", true)]
		[TestCase("-922337203685477.5808", true)]
		[TestCase("1.23456", false)]
		[TestCase("1.", false)]
		[TestCase("01.5", false)]
		public void NumericSchemaTransformer_MoneyPattern_MatchesUpToFourDecimalPlaces_Test(string value, bool expected)
		{
			// Act
			var isMatch = System.Text.RegularExpressions.Regex.IsMatch(value, NumericSchemaTransformer.MoneyPattern);

			// Assert
			Assert.That(isMatch, Is.EqualTo(expected));
		}

		private static OpenApiSchemaTransformerContext CreateContext(Type type)
		{
			var context = (OpenApiSchemaTransformerContext)Activator.CreateInstance(typeof(OpenApiSchemaTransformerContext))!;
			var jsonTypeInfo = JsonSerializerOptions.Default.GetTypeInfo(type);
			typeof(OpenApiSchemaTransformerContext).GetProperty(nameof(OpenApiSchemaTransformerContext.JsonTypeInfo))!
				.SetValue(context, jsonTypeInfo);

			return context;
		}
	}
}
