using HbysApi.Domain.Entities; // IEntity'nin bulunduğu namespace

namespace HbysApi.Application.Services;

public interface IService<T> where T : IEntity
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(Guid id);
}
