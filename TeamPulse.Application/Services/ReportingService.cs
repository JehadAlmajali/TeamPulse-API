using TeamPulse.Application.DTOs;
using TeamPulse.Application.Interfaces;

namespace TeamPulse.Application.Services;

public class ReportingService
{
    private readonly IReportingRepository _reportingRepository;

    public ReportingService(
        IReportingRepository reportingRepository)
    {
        _reportingRepository = reportingRepository;
    }

    public async Task<ReportSummaryDto> GetSummaryAsync(
        DateOnly? from,
        DateOnly? to)
    {
        var totalResponses =
            await _reportingRepository.GetTotalResponsesAsync(
                from,
                to);

        var averageRating =
            await _reportingRepository.GetAverageRatingAsync(
                from,
                to);

        return new ReportSummaryDto
        {
            TotalResponses = totalResponses,
            AverageRating = averageRating
        };
    }
    public async Task<List<CategoryReportDto>> GetCategoryReportAsync(
    DateOnly? from,
    DateOnly? to)
    {
        return await _reportingRepository.GetCategoryReportAsync(
            from,
            to);
    }
    public async Task<List<TrendReportDto>> GetTrendReportAsync(
    DateOnly? from,
    DateOnly? to)
    {
        return await _reportingRepository.GetTrendReportAsync(
            from,
            to);
    }
    public async Task<List<QuestionReportDto>> GetQuestionReportAsync(
    DateOnly? from,
    DateOnly? to)
    {
        return await _reportingRepository.GetQuestionReportAsync(
            from,
            to);
    }
}