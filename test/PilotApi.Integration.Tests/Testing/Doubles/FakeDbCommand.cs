using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Integration.Tests.Testing.Doubles
{
	/// <summary>
	/// A fake ADO.NET command that records the executed SQL/parameters on its owning <see cref="FakeDbConnection"/>
	/// and returns pre-configured results, so that Dapper's real query/execute pipeline can run against it.
	/// </summary>
	public class FakeDbCommand : DbCommand
	{
		private readonly FakeDbConnection connection;
		private readonly FakeDbParameterCollection parameterCollection = new();

		/// <summary>
		/// Instantiates a new instance of the <see cref="FakeDbCommand"/> class.
		/// </summary>
		/// <param name="connection">
		/// The owning fake connection.
		/// </param>
		public FakeDbCommand(FakeDbConnection connection)
		{
			this.connection = connection;
		}

		/// <inheritdoc/>
		public override string CommandText { get; set; } = string.Empty;

		/// <inheritdoc/>
		public override int CommandTimeout { get; set; }

		/// <inheritdoc/>
		public override CommandType CommandType { get; set; } = CommandType.Text;

		/// <inheritdoc/>
		public override bool DesignTimeVisible { get; set; }

		/// <inheritdoc/>
		public override UpdateRowSource UpdatedRowSource { get; set; }

		/// <inheritdoc/>
		protected override DbConnection? DbConnection { get; set; }

		/// <inheritdoc/>
		protected override DbParameterCollection DbParameterCollection => this.parameterCollection;

		/// <inheritdoc/>
		protected override DbTransaction? DbTransaction { get; set; }

		/// <inheritdoc/>
		public override void Cancel()
		{
		}

		/// <inheritdoc/>
		public override int ExecuteNonQuery()
		{
			this.RecordExecution();

			return this.connection.NextNonQueryResult;
		}

		/// <inheritdoc/>
		public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
		{
			return Task.FromResult(this.ExecuteNonQuery());
		}

		/// <inheritdoc/>
		public override object? ExecuteScalar()
		{
			this.RecordExecution();

			if (this.connection.NextScalarResult != null)
			{
				return this.connection.NextScalarResult;
			}

			if (this.connection.NextQueryResult != null &&
				this.connection.NextQueryResult.Rows.Count > 0 &&
				this.connection.NextQueryResult.Columns.Count > 0)
			{
				return this.connection.NextQueryResult.Rows[0][0];
			}

			return null;
		}

		/// <inheritdoc/>
		public override Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken)
		{
			return Task.FromResult(this.ExecuteScalar());
		}

		/// <inheritdoc/>
		public override void Prepare()
		{
		}

		/// <inheritdoc/>
		protected override DbParameter CreateDbParameter()
		{
			return new FakeDbParameter();
		}

		/// <inheritdoc/>
		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			this.RecordExecution();

			return new FakeDbDataReader(this.connection.NextQueryResult ?? new DataTable());
		}

		/// <inheritdoc/>
		protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
		{
			return Task.FromResult<DbDataReader>(this.ExecuteDbDataReader(behavior));
		}

		private void RecordExecution()
		{
			var parameters = new Dictionary<string, object?>();
			foreach (FakeDbParameter parameter in this.parameterCollection)
			{
				parameters[parameter.ParameterName] = parameter.Value;
			}

			this.connection.ExecutedCommands.Add(new FakeExecutedCommand(this.CommandText, parameters));
		}
	}

	/// <summary>
	/// A minimal <see cref="DbParameter"/> implementation used by <see cref="FakeDbCommand"/>.
	/// </summary>
	public class FakeDbParameter : DbParameter
	{
		/// <inheritdoc/>
		public override DbType DbType { get; set; }

		/// <inheritdoc/>
		public override ParameterDirection Direction { get; set; } = ParameterDirection.Input;

		/// <inheritdoc/>
		public override bool IsNullable { get; set; }

		/// <inheritdoc/>
		public override string ParameterName { get; set; } = string.Empty;

		/// <inheritdoc/>
		public override string SourceColumn { get; set; } = string.Empty;

		/// <inheritdoc/>
		public override object? Value { get; set; }

		/// <inheritdoc/>
		public override bool SourceColumnNullMapping { get; set; }

		/// <inheritdoc/>
		public override int Size { get; set; }

		/// <inheritdoc/>
		public override void ResetDbType()
		{
		}
	}

	/// <summary>
	/// A minimal <see cref="DbParameterCollection"/> implementation used by <see cref="FakeDbCommand"/>.
	/// </summary>
	public class FakeDbParameterCollection : DbParameterCollection
	{
		private readonly List<FakeDbParameter> items = new();

		/// <inheritdoc/>
		public override int Count => this.items.Count;

		/// <inheritdoc/>
		public override object SyncRoot => this;

		/// <inheritdoc/>
		public override int Add(object value)
		{
			this.items.Add((FakeDbParameter)value);

			return this.items.Count - 1;
		}

		/// <inheritdoc/>
		public override void AddRange(Array values)
		{
			foreach (var value in values)
			{
				this.Add(value);
			}
		}

		/// <inheritdoc/>
		public override void Clear()
		{
			this.items.Clear();
		}

		/// <inheritdoc/>
		public override bool Contains(object value)
		{
			return this.items.Contains((FakeDbParameter)value);
		}

		/// <inheritdoc/>
		public override bool Contains(string value)
		{
			return this.items.Exists(f => f.ParameterName == value);
		}

		/// <inheritdoc/>
		public override void CopyTo(Array array, int index)
		{
			((System.Collections.ICollection)this.items).CopyTo(array, index);
		}

		/// <inheritdoc/>
		public override System.Collections.IEnumerator GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		/// <inheritdoc/>
		public override int IndexOf(object value)
		{
			return this.items.IndexOf((FakeDbParameter)value);
		}

		/// <inheritdoc/>
		public override int IndexOf(string parameterName)
		{
			return this.items.FindIndex(f => f.ParameterName == parameterName);
		}

		/// <inheritdoc/>
		public override void Insert(int index, object value)
		{
			this.items.Insert(index, (FakeDbParameter)value);
		}

		/// <inheritdoc/>
		public override void Remove(object value)
		{
			this.items.Remove((FakeDbParameter)value);
		}

		/// <inheritdoc/>
		public override void RemoveAt(int index)
		{
			this.items.RemoveAt(index);
		}

		/// <inheritdoc/>
		public override void RemoveAt(string parameterName)
		{
			var index = this.IndexOf(parameterName);
			if (index >= 0)
			{
				this.items.RemoveAt(index);
			}
		}

		/// <inheritdoc/>
		protected override DbParameter GetParameter(int index)
		{
			return this.items[index];
		}

		/// <inheritdoc/>
		protected override DbParameter GetParameter(string parameterName)
		{
			return this.items.Find(f => f.ParameterName == parameterName)!;
		}

		/// <inheritdoc/>
		protected override void SetParameter(int index, DbParameter value)
		{
			this.items[index] = (FakeDbParameter)value;
		}

		/// <inheritdoc/>
		protected override void SetParameter(string parameterName, DbParameter value)
		{
			var index = this.IndexOf(parameterName);
			if (index >= 0)
			{
				this.items[index] = (FakeDbParameter)value;
			}
			else
			{
				this.items.Add((FakeDbParameter)value);
			}
		}
	}
}
