namespace PilotApi.Domain.Models.Dto
{
	/// <summary>
	/// A response model for the Add method.
	/// </summary>
	public class AddResponseString
	{
		/// <summary>
		/// Instantiate a <see cref="AddResponseString"/> object.
		/// </summary>
		public AddResponseString()
		{
		}

		/// <summary>
		/// Instantiate a <see cref="AddResponseString"/> object.
		/// </summary>
		/// <param name="recordId">
		/// The Id of the new record, or a 0 if the add failed.
		/// </param>
		public AddResponseString(string recordId)
		{
			this.Id = recordId;
		}

		/// <summary>
		/// Gets or sets the Id of the new record, or 0 if the add failed.
		/// </summary>
		public string Id { get; set; }

		/// <inheritdoc/>>
		public override string ToString()
		{
			return $"{nameof(this.Id)}={this.Id}";
		}
	}
}
