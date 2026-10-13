using Microsoft.EntityFrameworkCore;
using TeamPulse.Application.Interfaces;
using TeamPulse.Infrastructure.Persistence;
using TeamPulse.Application.DTOs;

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
                .AsNoTracking()
                .Join(
                    _context.Questions.AsNoTracking(),
                    response => response.QuestionId,
                    question => question.Id,
                    (response, question) => new
                    {
                        Response = response,
                        Question = question
                    });

        if (from.HasValue)
        {
            query = query.Where(
                x => x.Response.AnsweredDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(
                x => x.Response.AnsweredDate <= to.Value);
        }

        return await query
            .GroupBy(x => x.Question.Category)
            .Select(group => new CategoryReportDto
            {
                Category = group.Key,
                ResponseCount = group.Count(),
                AverageRating =
                    group.Average(x => x.Response.Rating)
            })
            .OrderBy(x => x.Category)
            .ToListAsync();
    }
    public async Task<List<TrendReportDto>> GetTrendReportAsync(
    DateOnly? from,
    DateOnly? to)
    {
        var query = _context.PulseResponses
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
                AverageRating = group.Average(r => r.Rating)
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
                .AsNoTracking()
                .Join(
                    _context.Questions.AsNoTracking(),
                    response => response.QuestionId,
                    question => question.Id,
                    (response, question) => new
                    {
                        Response = response,
                        Question = question
                    });

        if (from.HasValue)
        {
            query = query.Where(
                x => x.Response.AnsweredDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(
                x => x.Response.AnsweredDate <= to.Value);
        }

        return await query
            .GroupBy(x => new
            {
                x.Question.Id,
                x.Question.QuestionText
            })
            .Select(group => new QuestionReportDto
            {
                QuestionId = group.Key.Id,
                Question = group.Key.QuestionText,
                ResponseCount = group.Count(),
                AverageRating =
                    group.Average(x => x.Response.Rating)
            })
            .OrderBy(x => x.QuestionId)
            .ToListAsync();
    }
}