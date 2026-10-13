using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace TeamPulse.Tests.Integration;

public class QuestionsApiTests :
IClassFixture<TeamPulseWebApplicationFactory>
{
    private readonly HttpClient _client;

    public QuestionsApiTests(
        TeamPulseWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetQuestions_WithoutToken_ReturnsUnauthorized()
    {
        var response =
            await _client.GetAsync("/api/questions");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task GetQuestions_WithValidToken_ReturnsOk()
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
            await _client.GetAsync("/api/questions");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task UnknownEndpoint_ReturnsStandardizedNotFound()
    {
        var response =
            await _client.GetAsync("/api/does-not-exist");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);

        var body =
            await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            404,
            body.GetProperty("status").GetInt32());

        Assert.Equal(
            "Requested resource not found.",
            body.GetProperty("message").GetString());
    }
}