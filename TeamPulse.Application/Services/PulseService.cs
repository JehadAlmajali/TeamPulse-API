using TeamPulse.Application.DTOs;
using TeamPulse.Application.Exceptions;
using TeamPulse.Application.Interfaces;
using TeamPulse.Domain.Entities;

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

    public async Task<TodayQuestionDto?> GetTodayQuestionAsync(
        long userId)
    {
        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        var hasAnsweredToday =
            await _pulseResponseRepository
                .HasAnsweredOnDateAsync(userId, today);

        if (hasAnsweredToday)
        {
            return null;
        }

        var totalResponses =
            await _pulseResponseRepository
                .CountByUserAsync(userId);

        var activeQuestions =
            await _questionRepository
                .GetActiveOrderedAsync();

        if (activeQuestions.Count == 0)
        {
            throw new AppException("NO_ACTIVE_QUESTIONS");
        }

        var nextQuestionIndex =
            totalResponses % activeQuestions.Count;

        var question =
            activeQuestions[nextQuestionIndex];

        return new TodayQuestionDto
        {
            QuestionId = question.Id,
            Question = question.QuestionText
        };
    }

    public async Task SubmitAnswerAsync(
        long userId,
        SubmitPulseAnswerRequest request)
    {
        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        var hasAnsweredToday =
            await _pulseResponseRepository
                .HasAnsweredOnDateAsync(userId, today);

        if (hasAnsweredToday)
        {
            throw new AppException("ALREADY_ANSWERED");
        }

        var totalResponses =
            await _pulseResponseRepository
                .CountByUserAsync(userId);

        var activeQuestions =
            await _questionRepository
                .GetActiveOrderedAsync();

        if (activeQuestions.Count == 0)
        {
            throw new AppException("NO_ACTIVE_QUESTIONS");
        }

        var expectedQuestionIndex =
            totalResponses % activeQuestions.Count;

        var expectedQuestion =
            activeQuestions[expectedQuestionIndex];

        if (request.QuestionId != expectedQuestion.Id)
        {
            throw new AppException("INVALID_QUESTION");
        }

        if (request.Rating < 1 || request.Rating > 5)
        {
            throw new AppException("INVALID_RATING");
        }

        var response = new PulseResponse
        {
            UserId = userId,
            QuestionId = request.QuestionId,
            Rating = request.Rating,
            AnsweredDate = today,
            CreatedDate = DateTime.UtcNow
        };

        await _pulseResponseRepository.AddAsync(response);
    }
}