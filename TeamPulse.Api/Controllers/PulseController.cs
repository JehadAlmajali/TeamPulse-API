using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamPulse.Application.Services;
using TeamPulse.Application.DTOs;

namespace TeamPulse.Api.Controllers;

[ApiController]
[Route("api/pulse")]
[Authorize]
public class PulseController : ControllerBase
{
    private readonly PulseService _pulseService;

    public PulseController(PulseService pulseService)
    {
        _pulseService = pulseService;
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetTodayQuestion()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;

        if (!long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var question =
            await _pulseService.GetTodayQuestionAsync(userId, today);

        if (question is null)
        {
            return NoContent();
        }

        return Ok(new
        {
            questionId = question.Id,
            question = question.QuestionText
        });
    }

    [HttpPost("answer")]
    public async Task<IActionResult> SubmitAnswer(
    SubmitPulseAnswerRequest request)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;

        if (!long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var result =
            await _pulseService.SubmitAnswerAsync(
                userId,
                today,
                request);

        return result switch
        {
            SubmitPulseAnswerResult.Success =>
                Ok(),

            SubmitPulseAnswerResult.AlreadyAnswered =>
                Conflict("Today's response has already been submitted."),

            SubmitPulseAnswerResult.InvalidQuestion =>
                BadRequest("The submitted question is not the expected question."),

            SubmitPulseAnswerResult.InvalidRating =>
                BadRequest("Rating must be between 1 and 5."),

            _ =>
                StatusCode(500)
        };
    }
}