using TeamPulse.Domain.Entities;

namespace TeamPulse.Application.Interfaces;

public interface IPulseResponseRepository
{
    Task<bool> HasAnsweredOnDateAsync(long userId, DateOnly date);

    Task<int> CountByUserAsync(long userId);

    Task AddAsync(PulseResponse response);
}