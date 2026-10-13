using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TeamPulse.Domain.Entities;
using TeamPulse.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace TeamPulse.Tests.Integration;

public class TeamPulseWebApplicationFactory
    : WebApplicationFactory<Program>
{

    private readonly string _databaseName =
        $"TeamPulseTests_{Guid.NewGuid()}";
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            var serviceProvider =
                services.BuildServiceProvider();

            using var scope =
                serviceProvider.CreateScope();

            var context =
                scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Database.EnsureCreated();

            if (!context.Questions.Any())
            {
                context.Questions.AddRange(
                    new Question
                    {
                        Id = 1,
                        DisplayOrder = 1,
                        QuestionText = "Question 1",
                        Category = "Test",
                        IsActive = true,
                        CreatedBy = 0,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Question
                    {
                        Id = 2,
                        DisplayOrder = 2,
                        QuestionText = "Question 2",
                        Category = "Test",
                        IsActive = true,
                        CreatedBy = 0,
                        CreatedDate = DateTime.UtcNow
                    });

                context.SaveChanges();
            }
        });
    }
}