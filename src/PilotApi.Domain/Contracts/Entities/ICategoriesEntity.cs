using PilotApi.Domain.Contracts.Entities.Base;

namespace PilotApi.Domain.Contracts.Entities
{
	/// <summary>
	/// An interface for an entity that represents a category in a database.
	/// </summary>
	public interface ICategoriesEntity : IEntityBase
	{
		/// <summary>
		/// Gets or sets the unique identifier for the category.
		/// </summary>
		int? CategoryID { get; set; }

		/// <summary>
		/// Gets or sets the name of the category.
		/// </summary>
		string? CategoryName { get; set; }

		/// <summary>
		/// Gets or sets the description of the category.
		/// </summary>
		string? Description { get; set; }

		/// <summary>
		/// Gets or sets the picture of the category as a byte array.
		/// </summary>
		byte[]? Picture { get; set; }
	}
}
