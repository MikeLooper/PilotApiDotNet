using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace PilotApi.Integration.Tests.Testing.Doubles
{
	/// <summary>
	/// A fake <see cref="DbDataReader"/> backed by an in-memory <see cref="DataTable"/>, allowing Dapper's
	/// real materialization pipeline (<c>QueryAsync&lt;TEntity&gt;</c>, etc.) to run against canned result rows.
	/// </summary>
	public class FakeDbDataReader : DbDataReader
	{
		private readonly DataTable table;
		private int rowIndex = -1;

		/// <summary>
		/// Instantiates a new instance of the <see cref="FakeDbDataReader"/> class.
		/// </summary>
		public FakeDbDataReader(DataTable table)
		{
			this.table = table;
		}

		/// <inheritdoc/>
		public override int Depth => 0;

		/// <inheritdoc/>
		public override int FieldCount => this.table.Columns.Count;

		/// <inheritdoc/>
		public override bool HasRows => this.table.Rows.Count > 0;

		/// <inheritdoc/>
		public override bool IsClosed { get; } = false;

		/// <inheritdoc/>
		public override int RecordsAffected => this.table.Rows.Count;

		/// <inheritdoc/>
		public override object this[int ordinal] => this.table.Rows[this.rowIndex][ordinal];

		/// <inheritdoc/>
		public override object this[string name] => this.table.Rows[this.rowIndex][name];

		/// <inheritdoc/>
		public override bool GetBoolean(int ordinal) => (bool)this.GetValue(ordinal);

		/// <inheritdoc/>
		public override byte GetByte(int ordinal) => (byte)this.GetValue(ordinal);

		/// <inheritdoc/>
		public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length) => throw new NotSupportedException();

		/// <inheritdoc/>
		public override char GetChar(int ordinal) => (char)this.GetValue(ordinal);

		/// <inheritdoc/>
		public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length) => throw new NotSupportedException();

		/// <inheritdoc/>
		public override string GetDataTypeName(int ordinal) => this.table.Columns[ordinal].DataType.Name;

		/// <inheritdoc/>
		public override DateTime GetDateTime(int ordinal) => (DateTime)this.GetValue(ordinal);

		/// <inheritdoc/>
		public override decimal GetDecimal(int ordinal) => (decimal)this.GetValue(ordinal);

		/// <inheritdoc/>
		public override double GetDouble(int ordinal) => (double)this.GetValue(ordinal);

		/// <inheritdoc/>
		public override Type GetFieldType(int ordinal) => this.table.Columns[ordinal].DataType;

		/// <inheritdoc/>
		public override float GetFloat(int ordinal) => (float)this.GetValue(ordinal);

		/// <inheritdoc/>
		public override Guid GetGuid(int ordinal) => (Guid)this.GetValue(ordinal);

		/// <inheritdoc/>
		public override short GetInt16(int ordinal) => (short)this.GetValue(ordinal);

		/// <inheritdoc/>
		public override int GetInt32(int ordinal) => (int)this.GetValue(ordinal);

		/// <inheritdoc/>
		public override long GetInt64(int ordinal) => (long)this.GetValue(ordinal);

		/// <inheritdoc/>
		public override string GetName(int ordinal) => this.table.Columns[ordinal].ColumnName;

		/// <inheritdoc/>
		public override int GetOrdinal(string name) => this.table.Columns[name]!.Ordinal;

		/// <inheritdoc/>
		public override string GetString(int ordinal) => (string)this.GetValue(ordinal);

		/// <inheritdoc/>
		public override object GetValue(int ordinal) => this.table.Rows[this.rowIndex][ordinal];

		/// <inheritdoc/>
		public override int GetValues(object[] values)
		{
			var row = this.table.Rows[this.rowIndex];
			var count = Math.Min(values.Length, row.ItemArray.Length);
			for (var index = 0; index < count; index++)
			{
				values[index] = row[index];
			}

			return count;
		}

		/// <inheritdoc/>
		public override bool IsDBNull(int ordinal) => this.table.Rows[this.rowIndex].IsNull(ordinal);

		/// <inheritdoc/>
		public override bool NextResult() => false;

		/// <inheritdoc/>
		public override bool Read()
		{
			this.rowIndex++;

			return this.rowIndex < this.table.Rows.Count;
		}

		/// <inheritdoc/>
		public override IEnumerator GetEnumerator() => this.table.Rows.GetEnumerator();
	}
}
