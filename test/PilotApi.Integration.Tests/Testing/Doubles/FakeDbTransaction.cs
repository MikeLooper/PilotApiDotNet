using System.Data;
using System.Data.Common;

namespace PilotApi.Integration.Tests.Testing.Doubles
{
	/// <summary>
	/// A fake ADO.NET transaction paired with <see cref="FakeDbConnection"/>, used to allow real
	/// <see cref="PilotApi.Repositories.DataSource.DataSourceContext"/> commit/rollback flow to run in tests.
	/// </summary>
	public class FakeDbTransaction : DbTransaction
	{
		private readonly FakeDbConnection connection;

		/// <summary>
		/// Instantiates a new instance of the <see cref="FakeDbTransaction"/> class.
		/// </summary>
		public FakeDbTransaction(FakeDbConnection connection, IsolationLevel isolationLevel)
		{
			this.connection = connection;
			this.IsolationLevel = isolationLevel;
		}

		/// <summary>
		/// Gets a value indicating whether <see cref="Commit"/> was called.
		/// </summary>
		public bool CommitCalled { get; private set; }

		/// <summary>
		/// Gets a value indicating whether <see cref="Rollback"/> was called.
		/// </summary>
		public bool RollbackCalled { get; private set; }

		/// <inheritdoc/>
		public override IsolationLevel IsolationLevel { get; }

		/// <inheritdoc/>
		protected override DbConnection DbConnection => this.connection;

		/// <inheritdoc/>
		public override void Commit()
		{
			this.CommitCalled = true;
		}

		/// <inheritdoc/>
		public override void Rollback()
		{
			this.RollbackCalled = true;
		}
	}
}
