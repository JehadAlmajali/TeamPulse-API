using Microsoft.EntityFrameworkCore;
using TeamPulse.Application.DTOs;
using TeamPulse.Application.Interfaces;
using TeamPulse.Infrastructure.Persistence;

namespace TeamPulse.Infrastructure.Repositories;

public class ReportingRepository : IReportingRepository
{
    private readonly AppDbContext _context;

    public ReportingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetTotalResponsesAsync(
        DateOnly? from,
        DateOnly? to)
    {
        var query =
            _context.PulseResponses.AsNoTracking();

        if (from.HasValue)
        {
            query = query.Where(
                r => r.AnsweredDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(
                r => r.AnsweredDate <= to.Value);
        }

        return await query.CountAsync();
    }

    public async Task<double> GetAverageRatingAsync(
        DateOnly? from,
        DateOnly? to)
    {
        var query =
            _context.PulseResponses.AsNoTracking();

        if (from.HasValue)
        {
            query = query.Where(
                r => r.AnsweredDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(
                r => r.AnsweredDate <= to.Value);
        }

        if (!await query.AnyAsync())
        {
            return 0;
        }

        return await query.AverageAsync(
            r => r.Rating);
    }

    public async Task<List<CategoryReportDto>> GetCategoryReportAsync(
        DateOnly? from,
        DateOnly? to)
    {
        var query =
            _context.PulseResponses
                .AsNoTracking();

        if (from.HasValue)
        {
            query = query.Where(
                r => r.AnsweredDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(
                r => r.AnsweredDate <= to.Value);
        }

        return await query
            .GroupBy(r => r.Question.Category)
            .Select(group => new CategoryReportDto
            {
                Category = group.Key,
                ResponseCount = group.Count(),
                AverageRating =
                    group.Average(r => r.Rating)
            })
            .OrderBy(x => x.Category)
            .ToListAsync();
    }

    public async Task<List<TrendReportDto>> GetTrendReportAsync(
        DateOnly? from,
        DateOnly? to)
    {
        var query =
            _context.PulseResponses
                .AsNoTracking();

        if (from.HasValue)
        {
            query = query.Where(
                r => r.AnsweredDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(
                r => r.AnsweredDate <= to.Value);
        }

        return await query
            .GroupBy(r => r.AnsweredDate)
            .Select(group => new TrendReportDto
            {
                Date = group.Key,
                ResponseCount = group.Count(),
                AverageRating =
                    group.Average(r => r.Rating)
            })
            .OrderBy(x => x.Date)
            .ToListAsync();
    }

    public async Task<List<QuestionReportDto>> GetQuestionReportAsync(
        DateOnly? from,
        DateOnly? to)
    {
        var query =
            _context.PulseResponses
                .AsNoTracking();

        if (from.HasValue)
        {
            query = query.Where(
                r => r.AnsweredDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(
                r => r.AnsweredDate <= to.Value);
        }

        return await query
            .GroupBy(r => new
            {
                r.Question.Id,
                r.Question.QuestionText
            })
            .Select(group => new QuestionReportDto
            {
                QuestionId = group.Key.Id,
                Question = group.Key.QuestionText,
                ResponseCount = group.Count(),
                AverageRating =
                    group.Average(r => r.Rating)
            })
            .OrderBy(x => x.QuestionId)
            .ToListAsync();
    }
}