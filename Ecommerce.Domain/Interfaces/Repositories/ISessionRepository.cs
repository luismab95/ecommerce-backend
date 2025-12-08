using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Interfaces.Repositories;


public interface ISessionRepository
{
    Task<Session?> GetSessionAsync(int id);
    Task<Session> AddAsync(Session session);
    Task UpdateAsync(Session session);

}
