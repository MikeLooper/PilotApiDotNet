using PilotApi.Domain.Contracts.Base;
using PilotApi.Domain.Contracts.Entities.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PilotApi.Services.Contracts
{
	/// <summary>
	/// An interface that defines methods for mapping between data transfer objects (DTOs) and entity models.
	/// </summary>
	public interface IDataMapperHandler
	{
		/// <summary>
		/// Maps a list of entity models to a list of data transfer objects (DTOs).
		/// </summary>
		/// <typeparam name="TDto">
		/// A type that implements the IDtoBase interface, representing the data transfer object.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// A type that implements the IEntityBase interface, representing the entity model.
		/// </typeparam>
		/// <param name="entities">
		/// A collection of entity models to be mapped to DTOs. This parameter can be null.
		/// </param>
		/// <returns>
		/// A collection of DTOs mapped from the provided entity models, or null if the input collection is null.
		/// </returns>
		Task<IEnumerable<TDto>?> MapEntityToDtoListAsync<TDto, TEntity>(IEnumerable<TEntity>? entities) where TDto : IDtoBase where TEntity : IEntityBase;

		/// <summary>
		/// Maps a single entity model to a data transfer object (DTO).
		/// </summary>
		/// <typeparam name="TDto">
		/// A type that implements the IDtoBase interface, representing the data transfer object.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// A type that implements the IEntityBase interface, representing the entity model.
		/// </typeparam>
		/// <param name="entity">
		/// A single entity model to be mapped to a DTO. This parameter can be null.
		/// </param>
		/// <returns>
		/// A DTO mapped from the provided entity model, or null if the input entity is null.
		/// </returns>
		Task<TDto?> MapEntityToDtoAsync<TDto, TEntity>(TEntity? entity) where TDto : IDtoBase where TEntity : IEntityBase;

		/// <summary>
		/// Maps a single data transfer object (DTO) to an entity model.
		/// </summary>
		/// <typeparam name="TDto">
		/// A type that implements the IDtoBase interface, representing the data transfer object.
		/// </typeparam>
		/// <typeparam name="TEntity">
		/// A type that implements the IEntityBase interface, representing the entity model.
		/// </typeparam>
		/// <param name="dto">
		/// A single data transfer object to be mapped to an entity model. This parameter can be null.
		/// </param>
		/// <returns>
		/// An entity model mapped from the provided data transfer object, or null if the input DTO is null.
		/// </returns>
		Task<TEntity?> MapDtoToEntityAsync<TDto, TEntity>(TDto? dto) where TDto : IDtoBase where TEntity : IEntityBase;
	}
}
