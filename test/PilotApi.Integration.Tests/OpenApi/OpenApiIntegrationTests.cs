using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;
using NUnit.Framework;
using PilotApi.Integration.Tests.Testing.Utilities;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PilotApi.Integration.Tests.OpenApi
{
	/// <summary>
	/// Integration tests that compare the OpenAPI specification maintained in the shared
	/// "PilotSharedSource" repository (<c>PilotApi_v1.yaml</c>) against the OpenAPI document generated
	/// live by the running <see cref="Web"/> application (served at <c>/openapi/v1.json</c>).
	/// </summary>
	/// <remarks>
	/// Severity of reported mismatches:
	/// <list type="bullet">
	/// <item>
	/// Metadata (title/description/summary/contact/license), paths/operations, parameters, responses,
	/// and security requirement mismatches are treated as Errors and will fail the test.
	/// </item>
	/// <item>
	/// Component schema (DTO) property mismatches are treated as Warnings and are written to the test
	/// output without failing the test.
	/// </item>
	/// </list>
	/// </remarks>
	[TestFixture]
	public class OpenApiIntegrationTests
	{
		/// <summary>
		/// The relative path, from the repository root, to the shared reference OpenAPI specification.
		/// </summary>
		private const string SharedOpenApiSpecificationRelativePath = "shared/PilotSharedSource/OpenAPI/PilotApi_v1.yaml";

		[Test]
		public async Task OpenApi_V1_GeneratedDocument_MatchesSharedSpecification_Test()
		{
			// Arrange
			using var factory = new WebApplicationFactory<Program>();
			using var client = factory.CreateClient();

			var actualDocument = await GetActualDocumentAsync(client);
			var expectedDocument = GetExpectedDocument();

			// Act
			var comparisonResult = OpenApiComparisonUtilities.Compare(expectedDocument, actualDocument);

			foreach (var warning in comparisonResult.Warnings)
			{
				TestContext.Out.WriteLine($"WARN: {warning}");
			}

			// Assert
			Assert.That(
				comparisonResult.Errors,
				Is.Empty,
				$"Found {comparisonResult.Errors.Count} OpenAPI specification mismatch(es):{Environment.NewLine}" +
					string.Join(Environment.NewLine, comparisonResult.Errors));
		}

		private static string FindRepositoryRoot(string startingDirectory)
		{
			var currentDirectory = new DirectoryInfo(startingDirectory);
			while (currentDirectory != null)
			{
				var candidatePath = Path.Combine(currentDirectory.FullName, SharedOpenApiSpecificationRelativePath.Replace('/', Path.DirectorySeparatorChar));
				if (File.Exists(candidatePath))
				{
					return currentDirectory.FullName;
				}

				currentDirectory = currentDirectory.Parent;
			}

			throw new FileNotFoundException(
				$"Could not locate the shared OpenAPI specification '{SharedOpenApiSpecificationRelativePath}' " +
				$"by searching upward from '{startingDirectory}'.");
		}

		private static async Task<OpenApiDocument> GetActualDocumentAsync(HttpClient client)
		{
			using var responseStream = await client.GetStreamAsync("/openapi/v1.json");

			var settings = new OpenApiReaderSettings();
			var readResult = await OpenApiModelFactory.LoadAsync(responseStream, "json", settings);

			return readResult.Document;
		}

		private static OpenApiDocument GetExpectedDocument()
		{
			var repositoryRoot = FindRepositoryRoot(AppContext.BaseDirectory);
			var specificationPath = Path.Combine(repositoryRoot, SharedOpenApiSpecificationRelativePath.Replace('/', Path.DirectorySeparatorChar));

			var settings = new OpenApiReaderSettings();
			settings.AddYamlReader();

			using var fileStream = new MemoryStream(File.ReadAllBytes(specificationPath));
			var readResult = OpenApiModelFactory.Load(fileStream, "yaml", settings);

			return readResult.Document;
		}
	}
}
