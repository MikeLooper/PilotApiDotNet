using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using PilotApi.Shared.Api.Extensions;
using PilotApi.Shared.Configuration;
using PilotApi.Shared.Constants;
using System;

namespace PilotApi.Shared.Tests.Api.Extensions
{
	[TestFixture]
	public class ApiExtensionsTests : TestBase
	{
		private static SecurityConfiguration GetSecurityConfiguration()
		{
			return new SecurityConfiguration
			{
				Active = true,
				BaseUrl = "http://localhost",
				Realm = "test-realm",
				ClientId = "test-client"
			};
		}

		[Test]
		public void ApiExtensions_AddVersioning_WithValidServiceCollection_ShouldRegisterServices_Test()
		{
			// Arrange
			var services = new ServiceCollection();

			// Act
			services.AddVersioning();
			var serviceProvider = services.BuildServiceProvider();

			// Assert
			Assert.NotNull(serviceProvider);
		}

		[Test]
		public void ApiExtensions_AddVersioning_QueryStringReader_ResolvesVersion_Test()
		{
			// Arrange
			var services = new ServiceCollection();
			services.AddVersioning();
			var serviceProvider = services.BuildServiceProvider();
			var context = new DefaultHttpContext { RequestServices = serviceProvider };
			var request = context.Request;
			request.QueryString = new QueryString("?api-version=2.0");
			var reader = serviceProvider.GetRequiredService<IOptions<ApiVersioningOptions>>().Value.ApiVersionReader;

			// Act
			var versions = reader.Read(request);

			// Assert
			Assert.That(versions, Is.EqualTo(new[] { "2.0" }));
		}

		[Test]
		public void ApiExtensions_AddVersioning_UrlSegmentReader_ResolvesVersion_Test()
		{
			// Arrange
			var services = new ServiceCollection();
			services.AddVersioning();
			var serviceProvider = services.BuildServiceProvider();
			var context = new DefaultHttpContext { RequestServices = serviceProvider };
			context.Request.RouteValues["version"] = "2.0";
			context.ApiVersioningFeature.RawRequestedApiVersions = new[] { "2.0" };
			var request = context.Request;
			var reader = serviceProvider.GetRequiredService<IOptions<ApiVersioningOptions>>().Value.ApiVersionReader;

			// Act
			var versions = reader.Read(request);

			// Assert
			Assert.That(versions, Is.EqualTo(new[] { "2.0" }));
		}

		[Test]
		public void ApiExtensions_AddVersioning_HeaderReader_DoesNotResolveVersion_Test()
		{
			// Arrange
			var services = new ServiceCollection();
			services.AddVersioning();
			var serviceProvider = services.BuildServiceProvider();
			var context = new DefaultHttpContext { RequestServices = serviceProvider };
			var request = context.Request;
			request.Headers[HeaderConstants.ApiVersionHeaderName] = "2.0";
			var reader = serviceProvider.GetRequiredService<IOptions<ApiVersioningOptions>>().Value.ApiVersionReader;

			// Act
			var versions = reader.Read(request);

			// Assert
			Assert.That(versions, Is.Empty);
		}

		[Test]
		public void ApiExtensions_ApiWebApplicationBuilder_WithNullBuilder_ShouldThrow_Test()
		{
			// Arrange & Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				ApiExtensions.ApiWebApplicationBuilder(null));
			Assert.That(exception.Message, Does.Contain("builder"));
		}

		//[Test]
		//public void ApiExtensions_ApiWebApplicationBuilder_WithValidBuilder_ShouldRegisterServices_Test()
		//{
		//	// Arrange
		//	var builder = WebApplication.CreateBuilder();
		//	builder.ApplicationRegistration();

		//	// Act & Assert
		//	Assert.DoesNotThrow(() => builder.ApiWebApplicationBuilder());
		//}

		[Test]
		public void ApiExtensions_UseSecurity_WithNullWebApp_ShouldThrow_Test()
		{
			// Arrange & Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				ApiExtensions.UseSecurity(null));
			Assert.That(exception.Message, Does.Contain("webApp"));
		}

		[Test]
		public void ApiExtensions_ApiWebApplication_WithNullWebApp_ShouldThrow_Test()
		{
			// Arrange & Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				ApiExtensions.ApiWebApplication(null));
			Assert.That(exception.Message, Does.Contain("webApp"));
		}

		[Test]
		public void ApiExtensions_AddVersioning_IsExtensionMethod_Test()
		{
			// Arrange
			var services = new ServiceCollection();

			// Act & Assert
			Assert.DoesNotThrow(() => services.AddVersioning());
		}
	}
}
