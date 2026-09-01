using PilotApi.Shared.Configuration;
using PilotApi.Shared.Constants;
using PilotApi.TestingShared.Doubles;
using System.Collections.Generic;

namespace PilotApi.TestingShared.Utilities
{
	/// <summary>
	/// Utility method for working with shared testing doubles in unit tests.
	/// </summary>
	public static class TestingSharedDoublesUtilities
	{
		/// <summary>
		/// Creates a mock application configuration for testing purposes.
		/// </summary>
		/// <param name="dataSourceType">
		/// A <see cref="DataSourceTypes"/> value that specifies the type of data source to use in the mock configuration.
		/// </param>
		/// <returns>
		/// An <see cref="ApplicationConfiguration"/> instance that can be used in unit tests.
		/// </returns>
		public static ApplicationConfiguration GetApplicationConfiguration(DataSourceTypes dataSourceType)
		{
			return new ApplicationConfiguration
			{
				DataConnections = new List<DataConnectionConfiguration>
				{
					new DataConnectionConfiguration
					{
						Active = true,
						DataSourceName = "NorthwindConnection",
						Host = "localhost",
						Password = "secret",
						UserName = "sa",
						Port = 1433,
						ConnectTimeout = 15
					}
				},
				DataSources = new List<DataSourceConfiguration>
				{
					new DataSourceConfiguration
					{
						Active = true,
						DataSource = "Northwind",
						DataSourceEnum = dataSourceType,
						DataSourceName = "NorthwindConnection",
						DataSourceType = dataSourceType.ToString(),
						Schema = "dbo"
					}
				},
				Security = new SecurityConfiguration
				{
					Active = true,
					BaseUrl = "http://localhost:55001",
					Realm = "local-realm",
					ClientId = "local-client",
					RequireHttpsMetadata = false
				},
				OpenApi = new OpenApiConfiguration
				{
					Active = true,
					Title = "PilotApi",
					Version = "1.0.0",
					Description = "description",
					Summary = "summary",
					License = "MIT",
					Contact = new OpenApiContactConfiguration
					{
						Active = true,
						Name = "Support",
						Email = "support@example.com",
						URL = "https://example.com"
					}
				},
				OpenTelemetry = new OpenTelemetryConfiguration
				{
					Active = true,
					Server = "localhost",
					Port = 4318
				}
			};
		}

		/// <summary>
		/// A mock logger for testing purposes.
		/// </summary>
		/// <returns>
		/// A <see cref="MockLogger"/> instance that can be used in unit tests.
		/// </returns>
		public static MockLogger GetMockLogger()
		{
			return new MockLogger();
		}

		/// <summary>
		/// A mock logger factory for testing purposes.
		/// </summary>
		/// <returns>
		/// A <see cref="MockLoggerFactory"/> instance that can be used in unit tests.
		/// </returns>
		public static MockLoggerFactory GetMockLoggerFactory()
		{
			return new MockLoggerFactory();
		}
	}
}
