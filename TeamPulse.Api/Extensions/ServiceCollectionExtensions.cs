using TeamPulse.Application.Interfaces;
using TeamPulse.Application.Services;
using TeamPulse.Infrastructure.Repositories;

namespace TeamPulse.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTeamPulseServices(
        this IServiceCollection services)
    {
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IPulseResponseRepository, PulseResponseRepository>();
        services.AddScoped<IReportingRepository, ReportingRepository>();

        services.AddScoped<QuestionService>();
        services.AddScoped<PulseService>();
        services.AddScoped<ReportingService>();

        return services;
    }
}