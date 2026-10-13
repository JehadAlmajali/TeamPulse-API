using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamPulse.Application.Services;

namespace TeamPulse.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly ReportingService _reportingService;

    public ReportsController(
        ReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        DateOnly? from,
        DateOnly? to)
    {
        var summary =
            await _reportingService.GetSummaryAsync(
                from,
                to);

        return Ok(summary);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(
    DateOnly? from,
    DateOnly? to)
    {
        var categories =
            await _reportingService.GetCategoryReportAsync(
                from,
                to);

        return Ok(categories);
    }

    [HttpGet("trend")]
    public async Task<IActionResult> GetTrend(
    DateOnly? from,
    DateOnly? to)
    {
        var trend =
            await _reportingService.GetTrendReportAsync(
                from,
                to);

        return Ok(trend);
    }
    [HttpGet("questions")]
    public async Task<IActionResult> GetQuestionsReport(
    DateOnly? from,
    DateOnly? to)
    {
        var questions =
            await _reportingService.GetQuestionReportAsync(
                from,
                to);

        return Ok(questions);
    }
}