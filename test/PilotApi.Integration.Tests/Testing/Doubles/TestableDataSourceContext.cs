using Microsoft.Extensions.Logging;
using PilotApi.Repositories.DataSource;
using PilotApi.Shared.Contracts.Configuration;
using PilotApi.Shared.Handlers;

namespace PilotApi.Integration.Tests.Testing.Doubles
{
	/// <summary>
	/// A real <see cref="DataSourceContext"/> subclass used to inject a <see cref="FakeDbConnection"/> as the
	/// underlying ADO.NET connection, so that repository integration tests exercise the real connection/transaction
	/// lifecycle logic (open, begin transaction, commit, rollback, close, dispose) without needing a live database server.
	/// </summary>
	public class TestableDataSourceContext : DataSourceContext
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="TestableDataSourceContext"/> class.
		/// </summary>
		public TestableDataSourceContext(
			ILoggerFactory loggerFactory,
			IApplicationConfiguration applicationConfiguration,
			ISqlBuilder sqlBuilder,
			FakeDbConnection fakeDbConnection)
			: base(loggerFactory, applicationConfiguration, sqlBuilder)
		{
			this.DataSourceConnectionInternal = fakeDbConnection;
		}
	}
}
