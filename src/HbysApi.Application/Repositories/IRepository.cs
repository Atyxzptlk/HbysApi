// IRepository.cs
using System.Linq.Expressions;

namespace HbysApi.Application.Repositories;

// IRepository interface for generic data access operations
// Genel veri erişim işlemleri için IRepository arayüzü
public interface IRepository<T> where T : class
{
    // Asynchronously get all entities
    // Tüm varlıkları asenkron olarak getirir
    Task<IEnumerable<T>> GetAllAsync();
    // Asynchronously get entity by id
    // Id ile asenkron olarak varlık getirir
    Task<T?> GetByIdAsync(Guid id);
    // Asynchronously add entity
    // Varlığı asenkron olarak ekler
    Task AddAsync(T entity);
    // Asynchronously update entity
    // Varlığı asenkron olarak günceller
    Task UpdateAsync(T entity);
    // Asynchronously delete entity by id
    // Id ile asenkron olarak varlık siler
    Task DeleteAsync(Guid id);
    // Asynchronously find entities by predicate
    // Şarta göre asenkron olarak varlıkları bulur
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
}
