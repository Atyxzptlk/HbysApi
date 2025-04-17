using HbysApi.Domain.Entities; // Contact'ın bulunduğu namespace

namespace HbysApi.Application.Services;

public interface IContactService : IService<Contact>
{
    Task<IEnumerable<Contact>> SearchByNameAsync(string name);
}
