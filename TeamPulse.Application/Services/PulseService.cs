using TeamPulse.Application.Interfaces;
using TeamPulse.Domain.Entities;
using TeamPulse.Application.DTOs;

namespace TeamPulse.Application.Services;

public class PulseService
{
    private readonly IPulseResponseRepository _pulseResponseRepository;
    private readonly IQuestionRepository _questionRepository;

    public PulseService(
        IPulseResponseRepository pulseResponseRepository,
        IQuestionRepository questionRepository)
    {
        _pulseResponseRepository = pulseResponseRepository;
        _questionRepository = questionRepository;
    }

    public async Task<Question?> GetTodayQuestionAsync(
        long userId,
        DateOnly date)
    {
        var hasAnsweredToday =
            await _pulseResponseRepository.HasAnsweredOnDateAsync(
                userId,
                date);

        if (hasAnsweredToday)
        {
            return null;
        }

        var totalResponses =
            await _pulseResponseRepository.CountByUserAsync(userId);

        var activeQuestions =
            await _questionRepository.GetActiveOrderedAsync();

        if (activeQuestions.Count == 0)
        {
            return null;
        }

        var nextQuestionIndex =
            totalResponses % activeQuestions.Count;

        return activeQuestions[nextQuestionIndex];
    }

    public async Task<SubmitPulseAnswerResult> SubmitAnswerAsync(
    long userId,
    DateOnly date,
    SubmitPulseAnswerRequest request)
    {
        var hasAnsweredToday =
            await _pulseResponseRepository.HasAnsweredOnDateAsync(
                userId,
                date);

        if (hasAnsweredToday)
        {
            return SubmitPulseAnswerResult.AlreadyAnswered;
        }

        var totalResponses =
            await _pulseResponseRepository.CountByUserAsync(userId);

        var activeQuestions =
            await _questionRepository.GetActiveOrderedAsync();

        if (activeQuestions.Count == 0)
        {
            return SubmitPulseAnswerResult.InvalidQuestion;
        }

        var expectedQuestionIndex =
            totalResponses % activeQuestions.Count;

        var expectedQuestion =
            activeQuestions[expectedQuestionIndex];

        if (request.QuestionId != expectedQuestion.Id)
        {
            return SubmitPulseAnswerResult.InvalidQuestion;
        }

        if (request.Rating < 1 || request.Rating > 5)
        {
            return SubmitPulseAnswerResult.InvalidRating;
        }

        var response = new PulseResponse
        {
            UserId = userId,
            QuestionId = request.QuestionId,
            Rating = request.Rating,
            AnsweredDate = date,
            CreatedDate = DateTime.UtcNow
        };

        await _pulseResponseRepository.AddAsync(response);

        return SubmitPulseAnswerResult.Success;
    }
}