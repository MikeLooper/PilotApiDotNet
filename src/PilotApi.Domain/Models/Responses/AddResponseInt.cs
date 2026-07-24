namespace PilotApi.Domain.Models.Dto
{
	/// <summary>
	/// A response model for the Add method.
	/// </summary>
	public class AddResponseInt
	{
		/// <summary>
		/// Instantiate a <see cref="AddResponseInt"/> object.
		/// </summary>
		public AddResponseInt()
		{
		}

		/// <summary>
		/// Instantiate a <see cref="AddResponseInt"/> object.
		/// </summary>
		/// <param name="recordId">
		/// The Id of the new record, or a 0 if the add failed.
		/// </param>
		public AddResponseInt(long recordId)
		{
			this.Id = recordId;
		}

		/// <summary>
		/// Gets or sets the Id of the new record, or 0 if the add failed.
		/// </summary>
		public long Id { get; set; }

		/// <inheritdoc/>>
		public override string ToString()
		{
			return $"{nameof(this.Id)}={this.Id}";
		}
	}
}
