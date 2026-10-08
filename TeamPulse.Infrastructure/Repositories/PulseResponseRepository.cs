using Microsoft.EntityFrameworkCore;
using TeamPulse.Application.Interfaces;
using TeamPulse.Infrastructure.Persistence;

namespace TeamPulse.Infrastructure.Repositories;

public class PulseResponseRepository : IPulseResponseRepository
{
    private readonly AppDbContext _context;

    public PulseResponseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasAnsweredOnDateAsync(long userId, DateOnly date)
    {
        return await _context.PulseResponses
            .AsNoTracking()
            .AnyAsync(r =>
                r.UserId == userId &&
                r.AnsweredDate == date);
    }

    public async Task<int> CountByUserAsync(long userId)
    {
        return await _context.PulseResponses
            .AsNoTracking()
            .CountAsync(r => r.UserId == userId);
    }
}