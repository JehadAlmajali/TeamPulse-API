using Microsoft.EntityFrameworkCore;
using TeamPulse.Application.Interfaces;
using TeamPulse.Domain.Entities;
using TeamPulse.Infrastructure.Persistence;

namespace TeamPulse.Infrastructure.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly AppDbContext _context;

    public QuestionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Question>> GetAllAsync()
    {
        return await _context.Questions
            .AsNoTracking()
            .OrderBy(q => q.DisplayOrder)
            .ToListAsync();
    }

    public async Task<List<Question>> GetActiveOrderedAsync()
    {
        return await _context.Questions
            .AsNoTracking()
            .Where(q => q.IsActive)
            .OrderBy(q => q.DisplayOrder)
            .ToListAsync();
    }
}