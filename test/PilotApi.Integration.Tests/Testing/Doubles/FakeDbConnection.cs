using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Integration.Tests.Testing.Doubles
{
	/// <summary>
	/// A fake ADO.NET connection that allows real Dapper/SQL execution paths (real <see cref="PilotApi.Repositories.Repositories.Base.RepositoryBase{TEntity}"/>,
	/// real SQL builder generated statements, real <see cref="PilotApi.Repositories.DataSource.DataSourceContext"/> transaction handling) to run
	/// end-to-end in a test without requiring a live database server.
	/// It records every SQL statement/parameter set that is executed, and returns pre-configured results based on the
	/// SQL command's leading verb (SELECT / INSERT / UPDATE / DELETE).
	/// </summary>
	public class FakeDbConnection : DbConnection
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="FakeDbConnection"/> class.
		/// </summary>
		public FakeDbConnection()
		{
			this.ExecutedCommands = new List<FakeExecutedCommand>();
		}

		/// <summary>
		/// Gets the list of SQL statements (and their parameters) that were executed against this fake connection.
		/// </summary>
		public List<FakeExecutedCommand> ExecutedCommands { get; }

		/// <summary>
		/// Gets or sets the <see cref="DataTable"/> that will be returned for the next query (SELECT) execution.
		/// </summary>
		public DataTable? NextQueryResult { get; set; }

		/// <summary>
		/// Gets or sets the scalar value that will be returned for the next ExecuteScalar call (e.g. an identity/next-id value).
		/// </summary>
		public object? NextScalarResult { get; set; }

		/// <summary>
		/// Gets or sets the number of affected rows that will be returned for the next non-query (INSERT/UPDATE/DELETE) execution.
		/// </summary>
		public int NextNonQueryResult { get; set; } = 1;

		/// <inheritdoc/>
		public override string ConnectionString { get; set; } = "FakeConnectionString";

		/// <inheritdoc/>
		public override string Database => "FakeDatabase";

		/// <inheritdoc/>
		public override string DataSource => "FakeDataSource";

		/// <inheritdoc/>
		public override string ServerVersion => "1.0";

		/// <inheritdoc/>
		public override ConnectionState State => this.currentState;

		private ConnectionState currentState = ConnectionState.Closed;

		/// <inheritdoc/>
		public override void ChangeDatabase(string databaseName)
		{
		}

		/// <inheritdoc/>
		public override void Close()
		{
			this.currentState = ConnectionState.Closed;
		}

		/// <inheritdoc/>
		public override void Open()
		{
			this.currentState = ConnectionState.Open;
		}

		/// <inheritdoc/>
		public override Task OpenAsync(CancellationToken cancellationToken)
		{
			this.Open();

			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			return new FakeDbTransaction(this, isolationLevel);
		}

		/// <inheritdoc/>
		protected override DbCommand CreateDbCommand()
		{
			return new FakeDbCommand(this);
		}
	}

	/// <summary>
	/// Represents a single executed command against the <see cref="FakeDbConnection"/>, for assertion purposes.
	/// </summary>
	public class FakeExecutedCommand
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="FakeExecutedCommand"/> class.
		/// </summary>
		public FakeExecutedCommand(string commandText, IReadOnlyDictionary<string, object?> parameters)
		{
			this.CommandText = commandText;
			this.Parameters = parameters;
		}

		/// <summary>
		/// Gets the SQL command text that was executed.
		/// </summary>
		public string CommandText { get; }

		/// <summary>
		/// Gets the parameters that were supplied with the command.
		/// </summary>
		public IReadOnlyDictionary<string, object?> Parameters { get; }
	}
}
