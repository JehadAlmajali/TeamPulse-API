using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TeamPulse.Application.DTOs;
using TeamPulse.Domain.Entities;
using TeamPulse.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TeamPulse.Tests.Integration;

public class ReportsApiTests : IClassFixture<TeamPulseWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TeamPulseWebApplicationFactory _factory;

    public ReportsApiTests(
        TeamPulseWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSummary_WithoutToken_ReturnsUnauthorized()
    {
        var response =
            await _client.GetAsync("/api/reports/summary");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task GetSummary_WithAdminToken_ReturnsOk()
    {
        var tokenResponse =
            await _client.GetAsync("/api/dev/token");

        var tokenJson =
            await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();

        var token =
            tokenJson.GetProperty("accessToken").GetString();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                token);

        var response =
            await _client.GetAsync("/api/reports/summary");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Theory]
    [InlineData("/api/reports/categories")]
    [InlineData("/api/reports/trend")]
    [InlineData("/api/reports/questions")]
    public async Task ReportEndpoint_WithAdminToken_ReturnsOk(
    string endpoint)
    {
        var tokenResponse =
            await _client.GetAsync("/api/dev/token");

        var tokenJson =
            await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();

        var token =
            tokenJson.GetProperty("accessToken").GetString();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                token);

        var response =
            await _client.GetAsync(endpoint);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task GetSummary_WithKnownResponses_ReturnsCorrectValues()
    {
        var testDate = new DateOnly(2026, 1, 15);

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext =
                scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.PulseResponses.AddRange(
                new PulseResponse
                {
                    UserId = 1001,
                    QuestionId = 1,
                    Rating = 4,
                    AnsweredDate = testDate,
                    CreatedDate = DateTime.UtcNow
                },
                new PulseResponse
                {
                    UserId = 1002,
                    QuestionId = 2,
                    Rating = 2,
                    AnsweredDate = testDate,
                    CreatedDate = DateTime.UtcNow
                });

            await dbContext.SaveChangesAsync();
        }

        var tokenResponse =
            await _client.GetAsync("/api/dev/token");

        var tokenJson =
            await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();

        var token =
            tokenJson.GetProperty("accessToken").GetString();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                token);

        var response =
            await _client.GetAsync(
                "/api/reports/summary?from=2026-01-15&to=2026-01-15");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var report =
            await response.Content.ReadFromJsonAsync<ReportSummaryDto>();

        Assert.NotNull(report);
        Assert.Equal(2, report.TotalResponses);
        Assert.Equal(3, report.AverageRating);
    }

    [Fact]
    public async Task GetSummary_WithEmployeeToken_ReturnsForbidden()
    {
        var configuration =
            _factory.Services.GetRequiredService<IConfiguration>();

        var issuer = configuration["Jwt:Issuer"]!;
        var audience = configuration["Jwt:Audience"]!;
        var key = configuration["Jwt:Key"]!;

        var claims = new[]
        {
        new Claim("userId", "10"),
        new Claim("name", "Test Employee"),
        new Claim("role", "Employee")
    };

        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);

        var tokenString =
            new JwtSecurityTokenHandler().WriteToken(token);

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                tokenString);

        var response =
            await _client.GetAsync("/api/reports/summary");

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}