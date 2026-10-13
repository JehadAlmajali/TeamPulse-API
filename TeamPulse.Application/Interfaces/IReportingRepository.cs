namespace TeamPulse.Application.Interfaces;
using TeamPulse.Application.DTOs;

public interface IReportingRepository
{
    Task<int> GetTotalResponsesAsync(
        DateOnly? from,
        DateOnly? to);

    Task<double> GetAverageRatingAsync(
        DateOnly? from,
        DateOnly? to);

    Task<List<CategoryReportDto>> GetCategoryReportAsync(
        DateOnly? from,
        DateOnly? to);

    Task<List<TrendReportDto>> GetTrendReportAsync(
        DateOnly? from,
        DateOnly? to);
    Task<List<QuestionReportDto>> GetQuestionReportAsync(
        DateOnly? from,
        DateOnly? to);
}