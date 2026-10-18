using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamPulse.Application.DTOs;
using TeamPulse.Application.Services;

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
        var userId =
            long.Parse(User.FindFirst("userId")!.Value);

        var question =
            await _pulseService.GetTodayQuestionAsync(userId);

        return question is null
            ? NoContent()
            : Ok(question);
    }

    [HttpPost("answer")]
    public async Task<IActionResult> SubmitAnswer(
        SubmitPulseAnswerRequest request)
    {
        var userId =
            long.Parse(User.FindFirst("userId")!.Value);

        await _pulseService.SubmitAnswerAsync(
            userId,
            request);

        return Ok();
    }
}