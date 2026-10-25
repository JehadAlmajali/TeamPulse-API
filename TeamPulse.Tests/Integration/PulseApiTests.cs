using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TeamPulse.Infrastructure.Persistence;

namespace TeamPulse.Tests.Integration;

public class PulseApiTests :
    IClassFixture<TeamPulseWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TeamPulseWebApplicationFactory _factory;

    public PulseApiTests(
        TeamPulseWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task SubmitAnswer_WithoutToken_ReturnsUnauthorized()
    {
        var response =
            await _client.PostAsJsonAsync(
                "/api/pulse/answer",
                new
                {
                    questionId = 1,
                    rating = 4
                });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);

        var body =
            await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            401,
            body.GetProperty("status").GetInt32());

        Assert.Equal(
            "Authentication required.",
            body.GetProperty("message").GetString());
    }

    [Fact]
    public async Task GetTodayQuestion_WithValidToken_ReturnsFirstQuestion()
    {
        await ResetPulseResponsesAsync();
        await AuthenticateAsync();

        var response =
            await _client.GetAsync("/api/pulse/today");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var body =
            await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            1,
            body.GetProperty("questionId").GetInt64());

        Assert.Equal(
            "Question 1",
            body.GetProperty("question").GetString());
    }

    [Fact]
    public async Task SubmitAnswer_WithValidToken_SavesResponseAndReturnsOk()
    {
        await ResetPulseResponsesAsync();
        await AuthenticateAsync();

        var response =
            await _client.PostAsJsonAsync(
                "/api/pulse/answer",
                new
                {
                    questionId = 1,
                    rating = 4
                });

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task SubmitAnswer_InvalidRating_ReturnsBadRequest()
    {
        await ResetPulseResponsesAsync();
        await AuthenticateAsync();

        var response =
            await _client.PostAsJsonAsync(
                "/api/pulse/answer",
                new
                {
                    questionId = 1,
                    rating = 6
                });

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        var body =
            await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            400,
            body.GetProperty("status").GetInt32());

        Assert.Equal(
            "Rating must be between 1 and 5.",
            body.GetProperty("message").GetString());
    }

    [Fact]
    public async Task SubmitAnswer_WrongQuestion_ReturnsBadRequest()
    {
        await ResetPulseResponsesAsync();
        await AuthenticateAsync();

        var response =
            await _client.PostAsJsonAsync(
                "/api/pulse/answer",
                new
                {
                    questionId = 2,
                    rating = 4
                });

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        var body =
            await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            400,
            body.GetProperty("status").GetInt32());

        Assert.Equal(
            "The submitted question is not the expected question.",
            body.GetProperty("message").GetString());
    }

    [Fact]
    public async Task SubmitAnswer_TwiceSameDay_ReturnsConflict()
    {
        await ResetPulseResponsesAsync();
        await AuthenticateAsync();

        var firstResponse =
            await _client.PostAsJsonAsync(
                "/api/pulse/answer",
                new
                {
                    questionId = 1,
                    rating = 4
                });

        Assert.Equal(
            HttpStatusCode.OK,
            firstResponse.StatusCode);

        var secondResponse =
            await _client.PostAsJsonAsync(
                "/api/pulse/answer",
                new
                {
                    questionId = 1,
                    rating = 5
                });

        Assert.Equal(
            HttpStatusCode.Conflict,
            secondResponse.StatusCode);

        var body =
            await secondResponse.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            409,
            body.GetProperty("status").GetInt32());

        Assert.Equal(
            "Today's response has already been submitted.",
            body.GetProperty("message").GetString());
    }

    private async Task AuthenticateAsync()
    {
        var tokenResponse =
            await _client.GetAsync("/api/dev/token");

        tokenResponse.EnsureSuccessStatusCode();

        var tokenJson =
            await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();

        var token =
            tokenJson.GetProperty("accessToken").GetString();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);
    }

    private async Task ResetPulseResponsesAsync()
    {
        using var scope =
            _factory.Services.CreateScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        context.PulseResponses.RemoveRange(
            context.PulseResponses);

        await context.SaveChangesAsync();
    }
}