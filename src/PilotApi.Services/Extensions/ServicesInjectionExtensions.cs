using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PilotApi.Domain.Contracts.DataSource;
using PilotApi.Domain.Contracts.Services;
using PilotApi.Repositories.Contracts.Repository;
using PilotApi.Repositories.DataSource;
using PilotApi.Repositories.Handlers;
using PilotApi.Repositories.Repositories;
using PilotApi.Services.Contracts;
using PilotApi.Services.Handlers;
using PilotApi.Services.Security;
using PilotApi.Services.Services;
using PilotApi.Shared.Configuration;
using PilotApi.Shared.Contracts.Configuration;
using System;

namespace PilotApi.Services.Extensions
{
	/// <summary>
	/// Extension methods for the services layer.
	/// </summary>
	public static class ServicesInjectionExtensions
	{
		/// <summary>
		/// Register configuration injection objects.
		/// </summary>
		/// <param name="builder">
		/// A <see cref="WebApplicationBuilder"/> object.
		/// </param>
		/// <example>
		/// Example usage:
		/// <code>
		/// // app: create
		/// var webAppBuilder = WebApplication.CreateBuilder(args);
		/// 
		/// // custom: setup
		/// webAppBuilder.Services.ServicesConfiguration();
		/// </code>
		/// </example>
		public static void ServicesConfiguration(this WebApplicationBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentException($"Invalid argument : {nameof(builder)}. "
					+ $"A valid object type of: '{typeof(WebApplicationBuilder)}' is needed to continue. ({nameof(ServicesInjectionExtensions)})");
			}

			// read configuration
			var applicationSettings = new ApplicationConfiguration();
			builder.Configuration.GetSection("Application").Bind(applicationSettings);
			applicationSettings.Validate();

			builder.Services.AddSingleton<IApplicationConfiguration>(applicationSettings);
		}

		/// <summary>
		/// Register injection objects.
		/// </summary>
		/// <param name="builder">
		/// A <see cref="WebApplicationBuilder"/> object.
		/// </param>
		/// <example>
		/// Example usage:
		/// <code>
		/// // app: create
		/// var webAppBuilder = WebApplication.CreateBuilder(args);
		/// 
		/// // custom: setup
		/// webAppBuilder.Services.ServicesRegistration();
		/// </code>
		/// </example>
		public static void ServicesRegistration(this WebApplicationBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentException($"Invalid argument : {nameof(builder)}. "
					+ $"A valid object type of: '{typeof(WebApplicationBuilder)}' is needed to continue. ({nameof(ServicesInjectionExtensions)})");
			}

			// register services
			builder.Services.AddTransient<IDataSourceContext, DataSourceContext>();
			builder.Services.AddTransient<IDataMapperHandler, DataMapperHandler>();
			builder.Services.AddTransient<IEntityUpdateHandler, EntityUpdateHandler>();

			builder.Services.AddTransient<ICategoriesRepository, CategoriesRepository>();
			builder.Services.AddTransient<ICustomersRepository, CustomersRepository>();
			builder.Services.AddTransient<IEmployeesRepository, EmployeesRepository>();
			builder.Services.AddTransient<IOrderDetailsRepository, OrderDetailsRepository>();
			builder.Services.AddTransient<IOrdersRepository, OrdersRepository>();
			builder.Services.AddTransient<IProductsRepository, ProductsRepository>();
			builder.Services.AddTransient<IShippersRepository, ShippersRepository>();
			builder.Services.AddTransient<ISuppliersRepository, SuppliersRepository>();
			builder.Services.AddTransient<IUserRolesRepository, UserRolesRepository>();

			builder.Services.AddScoped<IClaimsTransformation, PreferredUsernameRoleClaimsTransformation>();

			builder.Services.AddTransient<ICategoriesService, CategoriesService>();
			builder.Services.AddTransient<ICustomersService, CustomersService>();
			builder.Services.AddTransient<IEmployeesService, EmployeesService>();
			builder.Services.AddTransient<IOrderDetailsService, OrderDetailsService>();
			builder.Services.AddTransient<IOrdersService, OrdersService>();
			builder.Services.AddTransient<IProductsService, ProductsService>();
			builder.Services.AddTransient<IShippersService, ShippersService>();
			builder.Services.AddTransient<ISuppliersService, SuppliersService>();
		}
	}
}
