using HbysApi.Domain.Entities; // IEntity'nin bulunduğu namespace

namespace HbysApi.Application.Services;

// IService: Interface for generic service operations
// IService: Genel servis işlemleri için arayüz
public interface IService<T> where T : IEntity
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(Guid id);
}
