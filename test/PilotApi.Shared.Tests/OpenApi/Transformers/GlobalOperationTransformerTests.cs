using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using NUnit.Framework;
using PilotApi.Shared.Constants;
using PilotApi.Shared.OpenApi.Transformers;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PilotApi.Shared.Tests.OpenApi.Transformers
{
	[TestFixture]
	public class GlobalOperationTransformerTests : TestBase
	{
		[Test]
		public void GlobalOperationTransformer_Constructor_ShouldInitialize_Test()
		{
			// Arrange & Act
			var transformer = new GlobalOperationTransformer();

			// Assert
			Assert.NotNull(transformer);
		}

		[Test]
		public void GlobalOperationTransformer_Transformer_IsIOpenApiOperationTransformer_Test()
		{
			// Arrange & Act
			var transformer = new GlobalOperationTransformer();
			var isTransformer = transformer is Microsoft.AspNetCore.OpenApi.IOpenApiOperationTransformer;

			// Assert
			Assert.That(isTransformer, Is.True);
		}

		[Test]
		public void GlobalOperationTransformer_Transformer_CanBeCreatedMultipleTimes_Test()
		{
			// Arrange & Act
			var transformer = new GlobalOperationTransformer();
			var transformer1 = new GlobalOperationTransformer();
			var transformer2 = new GlobalOperationTransformer();

			// Assert
			Assert.NotNull(transformer1);
			Assert.NotNull(transformer2);
			Assert.AreNotSame(transformer1, transformer2);
		}

		[Test]
		public async Task GlobalOperationTransformer_TransformAsync_ReplacesApiVersionHeaderParameter_Test()
		{
			// Arrange
			var transformer = new GlobalOperationTransformer();
			var operation = new OpenApiOperation
			{
				Parameters =
				[
					new OpenApiParameter
					{
						Name = "X-Api-Version",
						In = ParameterLocation.Header
					}
				]
			};
			var actionDescriptor = new ControllerActionDescriptor
			{
				ControllerTypeInfo = typeof(GlobalOperationTransformerTests).GetTypeInfo(),
				MethodInfo = typeof(GlobalOperationTransformerTests).GetMethod(nameof(TestAction))!
			};
			var context = (OpenApiOperationTransformerContext)Activator.CreateInstance(typeof(OpenApiOperationTransformerContext))!;
			typeof(OpenApiOperationTransformerContext).GetProperty(nameof(OpenApiOperationTransformerContext.Description))!
				.SetValue(context, new ApiDescription { ActionDescriptor = actionDescriptor });

			// Act
			await transformer.TransformAsync(operation, context, default);

			// Assert
			Assert.That(operation.Parameters, Has.Count.EqualTo(2));
			Assert.That(operation.Parameters[0].Name, Is.EqualTo("X-Api-Version"));
			Assert.That(operation.Parameters, Has.None.Matches<OpenApiParameter>(parameter => parameter.Name == "api-version"));
		}

		[Test]
		public async Task GlobalOperationTransformer_TransformAsync_FormatsExistingCorrelationIdHeaderAsUuid_Test()
		{
			// Arrange
			var transformer = new GlobalOperationTransformer();
			var operation = new OpenApiOperation
			{
				Parameters =
				[
					new OpenApiParameter
					{
						Name = HeaderConstants.CorrelationIdHeaderName,
						In = ParameterLocation.Header,
						Schema = new OpenApiSchema
						{
							Type = JsonSchemaType.String,
							Format = "other"
						}
					}
				]
			};
			var actionDescriptor = new ControllerActionDescriptor
			{
				ControllerTypeInfo = typeof(GlobalOperationTransformerTests).GetTypeInfo(),
				MethodInfo = typeof(GlobalOperationTransformerTests).GetMethod(nameof(TestAction))!
			};
			var context = (OpenApiOperationTransformerContext)Activator.CreateInstance(typeof(OpenApiOperationTransformerContext))!;
			typeof(OpenApiOperationTransformerContext).GetProperty(nameof(OpenApiOperationTransformerContext.Description))!
				.SetValue(context, new ApiDescription { ActionDescriptor = actionDescriptor });

			// Act
			await transformer.TransformAsync(operation, context, default);

			// Assert
			var correlationIdParameter = operation.Parameters.Single(parameter =>
				parameter.Name == HeaderConstants.CorrelationIdHeaderName);
			Assert.That(correlationIdParameter.Schema!.Format, Is.EqualTo("uuid"));
		}

		[Test]
		public async Task GlobalOperationTransformer_TransformAsync_AddsCorrelationIdHeaderWithUuidFormat_Test()
		{
			// Arrange
			var transformer = new GlobalOperationTransformer();
			var operation = new OpenApiOperation { Parameters = [] };
			var actionDescriptor = new ControllerActionDescriptor
			{
				ControllerTypeInfo = typeof(GlobalOperationTransformerTests).GetTypeInfo(),
				MethodInfo = typeof(GlobalOperationTransformerTests).GetMethod(nameof(TestAction))!
			};
			var context = (OpenApiOperationTransformerContext)Activator.CreateInstance(typeof(OpenApiOperationTransformerContext))!;
			typeof(OpenApiOperationTransformerContext).GetProperty(nameof(OpenApiOperationTransformerContext.Description))!
				.SetValue(context, new ApiDescription { ActionDescriptor = actionDescriptor });

			// Act
			await transformer.TransformAsync(operation, context, default);

			// Assert
			var correlationIdParameter = operation.Parameters.Single(parameter =>
				parameter.Name == HeaderConstants.CorrelationIdHeaderName);
			Assert.That(correlationIdParameter.In, Is.EqualTo(ParameterLocation.Header));
			Assert.That(correlationIdParameter.Schema!.Format, Is.EqualTo("uuid"));
		}

		[AllowAnonymous]
		public void TestAction()
		{
		}

		[FromHeader(Name = "X-Api-Version")]
		public string? ApiVersion { get; set; }

		[FromHeader(Name = HeaderConstants.CorrelationIdHeaderName)]
		public string? CorrelationId { get; set; }
	}
}
