using Microsoft.Extensions.Logging;
using PilotApi.Domain.Contracts.Base;
using PilotApi.Domain.Contracts.Services.Base;
using PilotApi.Domain.Models.Dto;
using PilotApi.Repositories.Contracts.Repository.Base;
using PilotApi.Repositories.Models.Base;
using PilotApi.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PilotApi.Services.Services.Base
{
	/// <summary>
	/// A base class for services.
	/// </summary>
	/// <typeparam name="TDto">
	/// The type for DTO classes.
	/// </typeparam>
	/// <typeparam name="TEntity">
	/// The type for Entity classes.
	/// </typeparam>
	public abstract class ServiceBase<TDto, TEntity> : IServiceBase<TDto> where TDto : IDtoBase where TEntity : EntityBase
	{
		/// <summary>
		/// Instantiates a new instance of the <see cref="ServiceBase{Dto, Entity}"/> class.
		/// </summary>
		/// <param name="loggerFactory">
		/// A logger factory for creating loggers.
		/// </param>
		/// <param name="repository">
		/// A repository for performing CRUD operations on entities.
		/// </param>
		/// <param name="dataMapperHandler">
		/// A data mapper handler for mapping between DTOs and entities.
		/// </param>
		protected ServiceBase(
			ILoggerFactory loggerFactory,
			IRepositoryBase<TEntity> repository,
			IDataMapperHandler dataMapperHandler)
		{
			this.Logger = loggerFactory.CreateLogger(GetType());
			this.Repository = repository;
			this.DataMapperHandler = dataMapperHandler;
		}

		/// <summary>
		/// Gets the data mapper handler for mapping between DTOs and entities.
		/// </summary>
		protected IDataMapperHandler DataMapperHandler { get; }

		/// <summary>
		/// Gets the logger for logging information and errors.
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Gets the repository for performing CRUD operations on entities.
		/// </summary>
		protected IRepositoryBase<TEntity> Repository { get; }

		/// <inheritdoc/>
		public async Task<RetrieveResponse<bool>> DeleteAsync<TType>(TType[] ids, CancellationToken cancellationToken = default)
		{
			if (ids.Length < 1)
			{
				throw new ArgumentException($"The supplied ids list ({ids} must contain at least one item ({this.GetType().Name})");
			}

			return await this.Repository.DeleteAsync(ids, cancellationToken);
		}

		/// <inheritdoc/>
		public async Task<RetrieveResponse<List<TDto>>?> GetAllAsync(CancellationToken cancellationToken = default)
		{
			var retrieveResponse = await this.Repository.GetAllAsync(cancellationToken);
			var mapped = await this.DataMapperHandler.MapEntityToDtoList<TDto, TEntity>(retrieveResponse.Result);
			return new RetrieveResponse<List<TDto>>(mapped?.ToList(), retrieveResponse.ErrorMessage);
		}

		/// <inheritdoc/>
		public async Task<RetrieveResponse<TDto>?> GetByIdAsync<TType>(TType[] ids, CancellationToken cancellationToken = default)
		{
			if (ids.Length < 1)
			{
				throw new ArgumentException($"The supplied ids list ({ids} must contain at least one item ({this.GetType().Name})");
			}

			var retrieveResponse = await this.Repository.GetAsync(ids, cancellationToken);
			var mapped = await this.DataMapperHandler.MapEntityToDto<TDto, TEntity>(retrieveResponse.Result);
			return new RetrieveResponse<TDto>(mapped, retrieveResponse.ErrorMessage);
		}

		/// <inheritdoc/>
		public async Task<RetrieveResponse<TReturn>?> InsertAsync<TReturn>(TDto model, CancellationToken cancellationToken = default)
		{
			if (model == null)
			{
				throw new ArgumentException($"Invalid argument: {nameof(model)}");
			}

			var mapped = await this.DataMapperHandler.MapDtoToEntity<TDto, TEntity>(model);
			var result = await this.Repository.InsertAsync<TReturn>(mapped, cancellationToken);
			return result;
		}

		/// <inheritdoc/>
		public async Task<RetrieveResponse<List<TDto>>?> QueryAsync(string query, object? parameters = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				throw new ArgumentException($"Invalid argument: {nameof(query)}");
			}

			var retrieveResponse = await this.Repository.QueryAsync(query, parameters, cancellationToken);
			var mapped = await this.DataMapperHandler.MapEntityToDtoList<TDto, TEntity>(retrieveResponse.Result);
			return new RetrieveResponse<List<TDto>>(mapped?.ToList(), retrieveResponse.ErrorMessage);
		}

		/// <inheritdoc/>
		public async Task<RetrieveResponse<TDto>?> QueryFirstAsync(string query, object? parameters = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				throw new ArgumentException($"Invalid argument: {nameof(query)}");
			}

			var retrieveResponse = await this.Repository.QueryFirstAsync(query, parameters, cancellationToken);
			var mapped = await this.DataMapperHandler.MapEntityToDto<TDto, TEntity>(retrieveResponse.Result);
			return new RetrieveResponse<TDto>(mapped, retrieveResponse.ErrorMessage);
		}

		/// <inheritdoc/>
		public async Task<RetrieveResponse<TDto>?> QuerySingleAsync(string query, object? parameters = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				throw new ArgumentException($"Invalid argument: {nameof(query)}");
			}

			var retrieveResponse = await this.Repository.QuerySingleAsync(query, parameters, cancellationToken);
			var mapped = await this.DataMapperHandler.MapEntityToDto<TDto, TEntity>(retrieveResponse.Result);
			return new RetrieveResponse<TDto>(mapped, retrieveResponse.ErrorMessage);
		}

		/// <inheritdoc/>
		public async Task<RetrieveResponse<bool>> UpdateAsync(TDto model, CancellationToken cancellationToken = default)
		{
			if (model == null)
			{
				throw new ArgumentException($"Invalid argument: {nameof(model)}");
			}

			var mapped = await this.DataMapperHandler.MapDtoToEntity<TDto, TEntity>(model);
			var result = await this.Repository.UpdateAsync(mapped, cancellationToken);
			return result;
		}
	}
}
