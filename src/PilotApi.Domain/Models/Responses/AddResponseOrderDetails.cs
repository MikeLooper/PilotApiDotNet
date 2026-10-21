namespace PilotApi.Domain.Models.Responses
{
	/// <summary>
	/// A response model for the Add method (OrderDetails).
	/// </summary>
	public class AddResponseOrderDetails
	{
		/// <summary>
		/// Instantiate a <see cref="AddResponseOrderDetails"/> object.
		/// </summary>
		public AddResponseOrderDetails()
		{
		}

		/// <summary>
		/// Instantiate a <see cref="AddResponseOrderDetails"/> object.
		/// </summary>
		/// <param name="orderId">
		/// The OrderId of the new record.
		/// </param>
		/// <param name="productId">
		/// The ProductId of the new record.
		/// </param>
		public AddResponseOrderDetails(int orderId, int productId)
		{
			this.OrderId = orderId;
			this.ProductId = productId;
		}

		/// <summary>
		/// Gets or sets the OrderId of the new record, or 0 if the add failed.
		/// </summary>
		public int OrderId { get; set; }

		/// <summary>
		/// Gets or sets the ProductId of the new record, or 0 if the add failed.
		/// </summary>
		public int ProductId { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{nameof(this.OrderId)}={this.OrderId}, {nameof(this.ProductId)}={this.ProductId}";
		}
	}
}
