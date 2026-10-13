using Moq;
using TeamPulse.Application.DTOs;
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
        var date = new DateOnly(2026, 10, 11);

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, date))
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
            await service.GetTodayQuestionAsync(userId, date);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetTodayQuestionAsync_AlreadyAnsweredToday_ReturnsNull()
    {
        var pulseResponseRepository =
            new Mock<IPulseResponseRepository>();

        var questionRepository =
            new Mock<IQuestionRepository>();

        var userId = 1L;
        var date = new DateOnly(2026, 10, 11);

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, date))
            .ReturnsAsync(true);

        var service = new PulseService(
            pulseResponseRepository.Object,
            questionRepository.Object);

        var result =
            await service.GetTodayQuestionAsync(userId, date);

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
        var date = new DateOnly(2026, 10, 11);

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, date))
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
            await service.GetTodayQuestionAsync(userId, date);

        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
    }

    [Fact]
    public async Task GetTodayQuestionAsync_CompletedQuestionCycle_ReturnsFirstQuestion()
    {
        var pulseResponseRepository =
            new Mock<IPulseResponseRepository>();

        var questionRepository =
            new Mock<IQuestionRepository>();

        var userId = 1L;
        var date = new DateOnly(2026, 10, 11);

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, date))
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
            await service.GetTodayQuestionAsync(userId, date);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task SubmitAnswerAsync_ValidAnswer_ReturnsSuccess()
    {
        var pulseResponseRepository =
            new Mock<IPulseResponseRepository>();

        var questionRepository =
            new Mock<IQuestionRepository>();

        var userId = 1L;
        var date = new DateOnly(2026, 10, 11);

        var request = new SubmitPulseAnswerRequest
        {
            QuestionId = 1,
            Rating = 4
        };

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, date))
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
            await service.SubmitAnswerAsync(userId, date, request);

        Assert.Equal(
            SubmitPulseAnswerResult.Success,
            result);

        pulseResponseRepository.Verify(
            r => r.AddAsync(It.Is<PulseResponse>(
                response =>
                    response.UserId == userId &&
                    response.QuestionId == 1 &&
                    response.Rating == 4 &&
                    response.AnsweredDate == date)),
            Times.Once);
    }

    [Fact]
    public async Task SubmitAnswerAsync_AlreadyAnswered_ReturnsAlreadyAnswered()
    {
        var pulseResponseRepository =
            new Mock<IPulseResponseRepository>();

        var questionRepository =
            new Mock<IQuestionRepository>();

        var userId = 1L;
        var date = new DateOnly(2026, 10, 11);

        var request = new SubmitPulseAnswerRequest
        {
            QuestionId = 1,
            Rating = 4
        };

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, date))
            .ReturnsAsync(true);

        var service = new PulseService(
            pulseResponseRepository.Object,
            questionRepository.Object);

        var result =
            await service.SubmitAnswerAsync(userId, date, request);

        Assert.Equal(
            SubmitPulseAnswerResult.AlreadyAnswered,
            result);

        pulseResponseRepository.Verify(
            r => r.AddAsync(It.IsAny<PulseResponse>()),
            Times.Never);
    }

    [Fact]
    public async Task SubmitAnswerAsync_InvalidRating_ReturnsInvalidRating()
    {
        var pulseResponseRepository =
            new Mock<IPulseResponseRepository>();

        var questionRepository =
            new Mock<IQuestionRepository>();

        var userId = 1L;
        var date = new DateOnly(2026, 10, 11);

        var request = new SubmitPulseAnswerRequest
        {
            QuestionId = 1,
            Rating = 6
        };

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, date))
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

        var result =
            await service.SubmitAnswerAsync(userId, date, request);

        Assert.Equal(
            SubmitPulseAnswerResult.InvalidRating,
            result);

        pulseResponseRepository.Verify(
            r => r.AddAsync(It.IsAny<PulseResponse>()),
            Times.Never);
    }

    [Fact]
    public async Task SubmitAnswerAsync_WrongQuestion_ReturnsInvalidQuestion()
    {
        var pulseResponseRepository =
            new Mock<IPulseResponseRepository>();

        var questionRepository =
            new Mock<IQuestionRepository>();

        var userId = 1L;
        var date = new DateOnly(2026, 10, 11);

        var request = new SubmitPulseAnswerRequest
        {
            QuestionId = 2,
            Rating = 4
        };

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, date))
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
            await service.SubmitAnswerAsync(userId, date, request);

        Assert.Equal(
            SubmitPulseAnswerResult.InvalidQuestion,
            result);

        pulseResponseRepository.Verify(
            r => r.AddAsync(It.IsAny<PulseResponse>()),
            Times.Never);
    }

    [Fact]
    public async Task GetTodayQuestionAsync_NoActiveQuestions_ReturnsNull()
    {
        var pulseResponseRepository =
            new Mock<IPulseResponseRepository>();

        var questionRepository =
            new Mock<IQuestionRepository>();

        var userId = 1L;
        var date = new DateOnly(2026, 10, 11);

        pulseResponseRepository
            .Setup(r => r.HasAnsweredOnDateAsync(userId, date))
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

        var result =
            await service.GetTodayQuestionAsync(userId, date);

        Assert.Null(result);
    }
}