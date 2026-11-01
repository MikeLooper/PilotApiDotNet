using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using PilotApi.Domain.Models.Dto;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PilotApi.Integration.Tests.OpenApi
{
	/// <summary>
	/// Integration tests verifying that numeric JSON values cannot be sent as strings, as the shared
	/// OpenAPI specification declares numeric properties as numbers only.
	/// </summary>
	[TestFixture]
	public class JsonNumberHandlingIntegrationTests
	{
		[Test]
		public void JsonOptions_NumberHandling_IsStrictForMvcAndOpenApi_Test()
		{
			// Arrange
			using var factory = new WebApplicationFactory<Program>();

			// Act
			var mvcOptions = factory.Services.GetRequiredService<IOptions<Microsoft.AspNetCore.Mvc.JsonOptions>>().Value;
			var httpOptions = factory.Services.GetRequiredService<IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions>>().Value;

			// Assert
			Assert.That(mvcOptions.JsonSerializerOptions.NumberHandling, Is.EqualTo(JsonNumberHandling.Strict));
			Assert.That(httpOptions.SerializerOptions.NumberHandling, Is.EqualTo(JsonNumberHandling.Strict));
		}

		[Test]
		public void JsonOptions_Deserialize_RejectsNumberSentAsString_Test()
		{
			// Arrange
			using var factory = new WebApplicationFactory<Program>();
			var options = factory.Services.GetRequiredService<IOptions<Microsoft.AspNetCore.Mvc.JsonOptions>>().Value.JsonSerializerOptions;

			// Act & Assert
			Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<CategoriesDto>("{\"categoryId\":\"1\"}", options));
			Assert.That(JsonSerializer.Deserialize<CategoriesDto>("{\"categoryId\":1}", options)!.CategoryId, Is.EqualTo(1));
		}
	}
}
