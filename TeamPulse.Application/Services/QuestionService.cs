using TeamPulse.Application.Interfaces;
using TeamPulse.Domain.Entities;

namespace TeamPulse.Application.Services;

public class QuestionService
{
    private readonly IQuestionRepository _questionRepository;

    public QuestionService(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<List<Question>> GetAllQuestionsAsync()
    {
        return await _questionRepository.GetAllAsync();
    }
}