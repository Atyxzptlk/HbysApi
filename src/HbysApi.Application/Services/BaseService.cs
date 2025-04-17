using HbysApi.Application.Repositories;
using HbysApi.Domain.Entities; // IEntity'nin bulunduğu namespace

namespace HbysApi.Application.Services;

public class BaseService<T> : IService<T> where T : class, IEntity
{
    private readonly IRepository<T> _repository;

    public BaseService(IRepository<T> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            throw new InvalidOperationException($"Entity of type {typeof(T).Name} with ID {id} not found.");
        }

        return entity;
    }
}
