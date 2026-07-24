namespace PilotApi.Domain.Models.Dto
{
	/// <summary>
	/// A result for a data source retrieve.
	/// </summary>
	public class RetrieveResponse<TReturn>
	{
		/// <summary>
		/// Instantiate a <see cref="RetrieveResponse{TReturn}"/> object.
		/// </summary>
		public RetrieveResponse()
		{
		}

		/// <summary>
		/// Instantiate a <see cref="RetrieveResponse{TReturn}"/> object.
		/// </summary>
		/// <param name="result">
		/// The result of the retrieval.
		/// </param>
		/// <param name="errorMessage">
		/// a message if an error occurred.
		/// Default = Null.
		/// </param>
		public RetrieveResponse(
			TReturn result,
			string? errorMessage = null)
		{
			this.Result = result;
			this.ErrorMessage = errorMessage;
		}

		/// <summary>
		/// Gets or sets a message if an error occurred.
		/// Default = Null.
		/// </summary>
		public string? ErrorMessage { get; set; }

		/// <summary>
		/// Gets or sets a flag that indicates whether an error occurred.
		/// Default = False.
		/// </summary>
		public bool IsError
		{
			get
			{
				return !string.IsNullOrWhiteSpace(ErrorMessage);
			}
		}

		/// <summary>
		/// Gets or sets the result of the retrieval.
		/// </summary>
		public TReturn? Result { get; set; }

		/// <inheritdoc/>>
		public override string ToString()
		{
			return $"{nameof(this.ErrorMessage)}={this.ErrorMessage}, " +
				$"{nameof(this.IsError)}={this.IsError}, " +
				$"{nameof(this.Result)}=[{this.Result}]";
		}
	}
}
