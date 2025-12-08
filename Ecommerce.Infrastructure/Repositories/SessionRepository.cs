using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces.Repositories;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories;


public class SessionRepository : ISessionRepository
{

    private readonly ApplicationDbContext _context;

    public SessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Session> AddAsync(Session session)
    {
        var entityEntry = await _context.Sessions.AddAsync(session);
        await _context.SaveChangesAsync();
        return entityEntry.Entity;
    }

    public async Task<Session?> GetSessionAsync(int id)
    {
        return await _context.Sessions.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
    }

    public async Task UpdateAsync(Session session)
    {
        _context.Sessions.Update(session);
        await _context.SaveChangesAsync();
    }
}
