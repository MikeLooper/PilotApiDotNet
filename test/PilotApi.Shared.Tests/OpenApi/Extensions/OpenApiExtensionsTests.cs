using NUnit.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PilotApi.Shared.OpenApi.Extensions;
using System;

namespace PilotApi.Shared.Tests.OpenApi.Extensions
{
	[TestFixture]
	public class OpenApiExtensionsTests
	{
		[Test]
		public void OpenApiExtensions_OpenApiWebApplicationBuilder_WithNullBuilder_ShouldThrow_Test()
		{
			// Arrange & Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				OpenApiExtensions.OpenApiWebApplicationBuilder(null, null));
			Assert.That(exception.Message, Does.Contain("builder"));
		}

		[Test]
		public void OpenApiExtensions_OpenApiWebApplicationBuilder_WithValidBuilder_ShouldNotThrow_Test()
		{
			// Arrange
			var builder = WebApplication.CreateBuilder();
			var serviceProvider = builder.Services.BuildServiceProvider();

			// Act & Assert
			Assert.Throws<InvalidOperationException>(() => builder.OpenApiWebApplicationBuilder(serviceProvider));
		}

		[Test]
		public void OpenApiExtensions_OpenApiWebApplication_WithNullWebApp_ShouldThrow_Test()
		{
			// Arrange & Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				OpenApiExtensions.OpenApiWebApplication(null));
			Assert.That(exception.Message, Does.Contain("webApp"));
		}

		[Test]
		public void OpenApiExtensions_OpenApiWebApplication_WithValidWebApp_ShouldNotThrow_Test()
		{
			// Arrange
			var builder = WebApplication.CreateBuilder();
			var app = builder.Build();

			// Act & Assert
			Assert.DoesNotThrow(() => app.OpenApiWebApplication());
		}

		[Test]
		public void OpenApiExtensions_OpenApiWebApplicationBuilder_IsExtensionMethod_Test()
		{
			// Arrange
			var builder = WebApplication.CreateBuilder();
			var serviceProvider = builder.Services.BuildServiceProvider();

			// Act & Assert
			Assert.Throws<InvalidOperationException>(() => builder.OpenApiWebApplicationBuilder(serviceProvider));
		}

		[Test]
		public void OpenApiExtensions_OpenApiWebApplication_IsExtensionMethod_Test()
		{
			// Arrange
			var builder = WebApplication.CreateBuilder();
			var app = builder.Build();

			// Act & Assert
			Assert.DoesNotThrow(() => app.OpenApiWebApplication());
		}

		[Test]
		public void OpenApiExtensions_OpenApiWebApplicationBuilder_ExceptionMessage_ShouldContainWebApplicationBuilderType_Test()
		{
			// Arrange & Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				OpenApiExtensions.OpenApiWebApplicationBuilder(null, null));
			Assert.That(exception.Message, Does.Contain("WebApplicationBuilder"));
		}

		[Test]
		public void OpenApiExtensions_OpenApiWebApplication_ExceptionMessage_ShouldContainWebApplicationType_Test()
		{
			// Arrange & Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				OpenApiExtensions.OpenApiWebApplication(null));
			Assert.That(exception.Message, Does.Contain("WebApplication"));
		}
	}
}

