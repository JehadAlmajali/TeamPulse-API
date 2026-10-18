using Moq;
using TeamPulse.Application.DTOs;
using TeamPulse.Application.Exceptions;
using TeamPulse.Application.Interfaces;
using TeamPulse.Application.Services;
using TeamPulse.Domain.Entities;

namespace TeamPulse.Tests.Services;

public class PulseServiceTests
{
    [Fact]
    public async Task GetTodayQuestionAsync_NoPreviousResponses_ReturnsFirstQuestion()
    {
        var pulseResponseRepository =
            new Mock<IPulseResponseRepository>();

        var questionRepository =
            new Mock<IQuestionRepository>();

        var userId = 1L;
        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, today))
            .ReturnsAsync(false);

        pulseResponseRepository
            .Setup(r => r.CountByUserAsync(userId))
            .ReturnsAsync(0);

        questionRepository
            .Setup(r => r.GetActiveOrderedAsync())
            .ReturnsAsync(new List<Question>
            {
                new Question
                {
                    Id = 1,
                    DisplayOrder = 1,
                    QuestionText = "Question 1"
                },

                new Question
                {
                    Id = 2,
                    DisplayOrder = 2,
                    QuestionText = "Question 2"
                }
            });

        var service = new PulseService(
            pulseResponseRepository.Object,
            questionRepository.Object);

        var result =
            await service.GetTodayQuestionAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(1, result.QuestionId);
    }

    [Fact]
    public async Task GetTodayQuestionAsync_AlreadyAnsweredToday_ReturnsNull()
    {
        var pulseResponseRepository =
            new Mock<IPulseResponseRepository>();

        var questionRepository =
            new Mock<IQuestionRepository>();

        var userId = 1L;
        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, today))
            .ReturnsAsync(true);

        var service = new PulseService(
            pulseResponseRepository.Object,
            questionRepository.Object);

        var result =
            await service.GetTodayQuestionAsync(userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetTodayQuestionAsync_OnePreviousResponse_ReturnsSecondQuestion()
    {
        var pulseResponseRepository =
            new Mock<IPulseResponseRepository>();

        var questionRepository =
            new Mock<IQuestionRepository>();

        var userId = 1L;
        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, today))
            .ReturnsAsync(false);

        pulseResponseRepository
            .Setup(r => r.CountByUserAsync(userId))
            .ReturnsAsync(1);

        questionRepository
            .Setup(r => r.GetActiveOrderedAsync())
            .ReturnsAsync(new List<Question>
            {
                new Question
                {
                    Id = 1,
                    DisplayOrder = 1,
                    QuestionText = "Question 1"
                },

                new Question
                {
                    Id = 2,
                    DisplayOrder = 2,
                    QuestionText = "Question 2"
                }
            });

        var service = new PulseService(
            pulseResponseRepository.Object,
            questionRepository.Object);

        var result =
            await service.GetTodayQuestionAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(2, result.QuestionId);
    }

    [Fact]
    public async Task GetTodayQuestionAsync_CompletedQuestionCycle_ReturnsFirstQuestion()
    {
        var pulseResponseRepository =
            new Mock<IPulseResponseRepository>();

        var questionRepository =
            new Mock<IQuestionRepository>();

        var userId = 1L;
        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, today))
            .ReturnsAsync(false);

        pulseResponseRepository
            .Setup(r => r.CountByUserAsync(userId))
            .ReturnsAsync(2);

        questionRepository
            .Setup(r => r.GetActiveOrderedAsync())
            .ReturnsAsync(new List<Question>
            {
                new Question
                {
                    Id = 1,
                    DisplayOrder = 1,
                    QuestionText = "Question 1"
                },

                new Question
                {
                    Id = 2,
                    DisplayOrder = 2,
                    QuestionText = "Question 2"
                }
            });

        var service = new PulseService(
            pulseResponseRepository.Object,
            questionRepository.Object);

        var result =
            await service.GetTodayQuestionAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(1, result.QuestionId);
    }

    [Fact]
    public async Task SubmitAnswerAsync_ValidAnswer_SavesResponse()
    {
        var pulseResponseRepository =
            new Mock<IPulseResponseRepository>();

        var questionRepository =
            new Mock<IQuestionRepository>();

        var userId = 1L;
        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        var request = new SubmitPulseAnswerRequest
        {
            QuestionId = 1,
            Rating = 4
        };

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, today))
            .ReturnsAsync(false);

        pulseResponseRepository
            .Setup(r => r.CountByUserAsync(userId))
            .ReturnsAsync(0);

        questionRepository
            .Setup(r => r.GetActiveOrderedAsync())
            .ReturnsAsync(new List<Question>
            {
                new Question
                {
                    Id = 1,
                    DisplayOrder = 1,
                    QuestionText = "Question 1"
                },

                new Question
                {
                    Id = 2,
                    DisplayOrder = 2,
                    QuestionText = "Question 2"
                }
            });

        var service = new PulseService(
            pulseResponseRepository.Object,
            questionRepository.Object);

        await service.SubmitAnswerAsync(
            userId,
            request);

        pulseResponseRepository.Verify(
            r => r.AddAsync(It.Is<PulseResponse>(
                response =>
                    response.UserId == userId &&
                    response.QuestionId == 1 &&
                    response.Rating == 4 &&
                    response.AnsweredDate == today)),
            Times.Once);
    }

    [Fact]
    public async Task SubmitAnswerAsync_AlreadyAnswered_ThrowsAppException()
    {
        var pulseResponseRepository =
            new Mock<IPulseResponseRepository>();

        var questionRepository =
            new Mock<IQuestionRepository>();

        var userId = 1L;
        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        var request = new SubmitPulseAnswerRequest
        {
            QuestionId = 1,
            Rating = 4
        };

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, today))
            .ReturnsAsync(true);

        var service = new PulseService(
            pulseResponseRepository.Object,
            questionRepository.Object);

        var exception =
            await Assert.ThrowsAsync<AppException>(
                () => service.SubmitAnswerAsync(
                    userId,
                    request));

        Assert.Equal(
            "ALREADY_ANSWERED",
            exception.Code);

        pulseResponseRepository.Verify(
            r => r.AddAsync(It.IsAny<PulseResponse>()),
            Times.Never);
    }

    [Fact]
    public async Task SubmitAnswerAsync_InvalidRating_ThrowsAppException()
    {
        var pulseResponseRepository =
            new Mock<IPulseResponseRepository>();

        var questionRepository =
            new Mock<IQuestionRepository>();

        var userId = 1L;
        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        var request = new SubmitPulseAnswerRequest
        {
            QuestionId = 1,
            Rating = 6
        };

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, today))
            .ReturnsAsync(false);

        pulseResponseRepository
            .Setup(r => r.CountByUserAsync(userId))
            .ReturnsAsync(0);

        questionRepository
            .Setup(r => r.GetActiveOrderedAsync())
            .ReturnsAsync(new List<Question>
            {
                new Question
                {
                    Id = 1,
                    DisplayOrder = 1,
                    QuestionText = "Question 1"
                }
            });

        var service = new PulseService(
            pulseResponseRepository.Object,
            questionRepository.Object);

        var exception =
            await Assert.ThrowsAsync<AppException>(
                () => service.SubmitAnswerAsync(
                    userId,
                    request));

        Assert.Equal(
            "INVALID_RATING",
            exception.Code);

        pulseResponseRepository.Verify(
            r => r.AddAsync(It.IsAny<PulseResponse>()),
            Times.Never);
    }

    [Fact]
    public async Task SubmitAnswerAsync_WrongQuestion_ThrowsAppException()
    {
        var pulseResponseRepository =
            new Mock<IPulseResponseRepository>();

        var questionRepository =
            new Mock<IQuestionRepository>();

        var userId = 1L;
        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        var request = new SubmitPulseAnswerRequest
        {
            QuestionId = 2,
            Rating = 4
        };

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, today))
            .ReturnsAsync(false);

        pulseResponseRepository
            .Setup(r => r.CountByUserAsync(userId))
            .ReturnsAsync(0);

        questionRepository
            .Setup(r => r.GetActiveOrderedAsync())
            .ReturnsAsync(new List<Question>
            {
                new Question
                {
                    Id = 1,
                    DisplayOrder = 1,
                    QuestionText = "Question 1"
                },

                new Question
                {
                    Id = 2,
                    DisplayOrder = 2,
                    QuestionText = "Question 2"
                }
            });

        var service = new PulseService(
            pulseResponseRepository.Object,
            questionRepository.Object);

        var exception =
            await Assert.ThrowsAsync<AppException>(
                () => service.SubmitAnswerAsync(
                    userId,
                    request));

        Assert.Equal(
            "INVALID_QUESTION",
            exception.Code);

        pulseResponseRepository.Verify(
            r => r.AddAsync(It.IsAny<PulseResponse>()),
            Times.Never);
    }

    [Fact]
    public async Task GetTodayQuestionAsync_NoActiveQuestions_ThrowsAppException()
    {
        var pulseResponseRepository =
            new Mock<IPulseResponseRepository>();

        var questionRepository =
            new Mock<IQuestionRepository>();

        var userId = 1L;
        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, today))
            .ReturnsAsync(false);

        pulseResponseRepository
            .Setup(r => r.CountByUserAsync(userId))
            .ReturnsAsync(0);

        questionRepository
            .Setup(r => r.GetActiveOrderedAsync())
            .ReturnsAsync(new List<Question>());

        var service = new PulseService(
            pulseResponseRepository.Object,
            questionRepository.Object);

        var exception =
            await Assert.ThrowsAsync<AppException>(
                () => service.GetTodayQuestionAsync(userId));

        Assert.Equal(
            "NO_ACTIVE_QUESTIONS",
            exception.Code);
    }
}