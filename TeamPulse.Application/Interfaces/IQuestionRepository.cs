using TeamPulse.Domain.Entities;

namespace TeamPulse.Application.Interfaces;

public interface IQuestionRepository
{
    Task<List<Question>> GetAllAsync();
    Task<List<Question>> GetActiveOrderedAsync();
}