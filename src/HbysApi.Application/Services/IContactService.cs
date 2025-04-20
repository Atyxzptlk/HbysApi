// IContactService: Interface for contact-specific service operations
// IContactService: Kişi/iletişim özel servis işlemleri için arayüz
using HbysApi.Domain.Entities; // Contact'ın bulunduğu namespace

namespace HbysApi.Application.Services;

public interface IContactService : IService<Contact>
{
    Task<IEnumerable<Contact>> SearchByNameAsync(string name);
}
