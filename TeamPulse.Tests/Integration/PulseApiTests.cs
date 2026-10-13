using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace TeamPulse.Tests.Integration;

public class PulseApiTests :
    IClassFixture<TeamPulseWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PulseApiTests(
        TeamPulseWebApplicationFactory factory)
    {
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
    }

    [Fact]
    public async Task GetTodayQuestion_WithValidToken_ReturnsFirstQuestion()
    {
        var tokenResponse =
            await _client.GetAsync("/api/dev/token");

        tokenResponse.EnsureSuccessStatusCode();

        var tokenJson =
            await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();

        var token =
            tokenJson.GetProperty("accessToken").GetString();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

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
        var tokenResponse =
            await _client.GetAsync("/api/dev/token");

        tokenResponse.EnsureSuccessStatusCode();

        var tokenJson =
            await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();

        var token =
            tokenJson.GetProperty("accessToken").GetString();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

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
}