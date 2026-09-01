using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PilotApi.Shared.Api.Extensions;
using PilotApi.Shared.Configuration;
using System;

namespace PilotApi.Shared.Tests.Api.Extensions
{
	[TestFixture]
	public class ApiExtensionsTests
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
		public void ApiExtensions_AddSecurity_WithValidServiceCollection_ShouldNotThrow_Test()
		{
			// Arrange
			var services = new ServiceCollection();

			// Act & Assert
			Assert.DoesNotThrow(() => services.AddSecurity(GetSecurityConfiguration()));
		}

		[Test]
		public void ApiExtensions_AddSecurity_WithNullServices_ShouldThrow_Test()
		{
			// Arrange & Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				ApiExtensions.AddSecurity(null, GetSecurityConfiguration()));
			Assert.That(exception.Message, Does.Contain("services"));
		}

		[Test]
		public void ApiExtensions_AddSecurity_WithNullSecurityConfiguration_ShouldThrow_Test()
		{
			// Arrange
			var services = new ServiceCollection();

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				services.AddSecurity(null));
			Assert.That(exception.Message, Does.Contain("securityConfiguration"));
		}

		[Test]
		public void ApiExtensions_AddSecurity_WithValidServiceCollection_ShouldRegisterAuthorizationHandlers_Test()
		{
			// Arrange
			var services = new ServiceCollection();
			services.AddLogging();

			// Act
			services.AddSecurity(GetSecurityConfiguration());
			var serviceProvider = services.BuildServiceProvider();

			// Assert
			Assert.NotNull(serviceProvider.GetService<IAuthorizationHandler>());
			Assert.NotNull(serviceProvider.GetService<IAuthorizationMiddlewareResultHandler>());
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
		public void ApiExtensions_UseSecurity_WithValidWebApp_ShouldNotThrow_Test()
		{
			// Arrange
			var builder = WebApplication.CreateBuilder();
			builder.Services.AddSecurity(GetSecurityConfiguration());
			var app = builder.Build();

			// Act & Assert
			Assert.DoesNotThrow(() => app.UseSecurity());
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
		public void ApiExtensions_ApiWebApplication_WithValidWebApp_ShouldNotThrow_Test()
		{
			// Arrange
			var builder = WebApplication.CreateBuilder();
			builder.Services.AddSecurity(GetSecurityConfiguration());
			var app = builder.Build();

			// Act & Assert
			Assert.DoesNotThrow(() => app.ApiWebApplication());
		}

		[Test]
		public void ApiExtensions_AddSecurity_IsExtensionMethod_Test()
		{
			// Arrange
			var services = new ServiceCollection();

			// Act & Assert
			Assert.DoesNotThrow(() => services.AddSecurity(GetSecurityConfiguration()));
		}

		[Test]
		public void ApiExtensions_AddVersioning_IsExtensionMethod_Test()
		{
			// Arrange
			var services = new ServiceCollection();

			// Act & Assert
			Assert.DoesNotThrow(() => services.AddVersioning());
		}

		[Test]
		public void ApiExtensions_ApiWebApplication_IsExtensionMethod_Test()
		{
			// Arrange
			var builder = WebApplication.CreateBuilder();
			builder.Services.AddSecurity(GetSecurityConfiguration());
			var app = builder.Build();

			// Act & Assert
			Assert.DoesNotThrow(() => app.ApiWebApplication());
		}

		[Test]
		public void ApiExtensions_UseSecurity_IsExtensionMethod_Test()
		{
			// Arrange
			var builder = WebApplication.CreateBuilder();
			builder.Services.AddSecurity(GetSecurityConfiguration());
			var app = builder.Build();

			// Act & Assert
			Assert.DoesNotThrow(() => app.UseSecurity());
		}
	}
}

