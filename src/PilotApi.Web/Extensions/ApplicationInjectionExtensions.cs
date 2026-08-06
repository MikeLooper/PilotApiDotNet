using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PilotApi.Services.Extensions;
using System;

namespace PilotApi.Web.Extensions
{
	/// <summary>
	/// Extension methods for the application layer.
	/// </summary>
	public static class ApplicationInjectionExtensions
	{
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
		/// webAppBuilder.ApplicationRegistration();
		/// </code>
		/// </example>
		public static void ApplicationRegistration(this WebApplicationBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentException($"Invalid argument : {nameof(builder)}. "
					+ $"A valid object type of: '{typeof(IServiceCollection)}' is needed to continue. ({nameof(ApplicationInjectionExtensions)})");
			}

			builder.ServicesConfiguration();
			builder.ServicesRegistration();
		}
	}
}
