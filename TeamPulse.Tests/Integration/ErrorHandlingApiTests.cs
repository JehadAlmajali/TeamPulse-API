using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace TeamPulse.Tests.Integration;

public class ErrorHandlingApiTests :
    IClassFixture<TeamPulseWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ErrorHandlingApiTests(
        TeamPulseWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UnknownEndpoint_ReturnsNotFound()
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

    [Fact]
    public async Task SubmitAnswer_WithMalformedJson_ReturnsStandardBadRequest()
    {
        var tokenResponse =
            await _client.GetAsync("/api/dev/token");

        tokenResponse.EnsureSuccessStatusCode();

        var tokenJson =
            await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();

        var token =
            tokenJson.GetProperty("accessToken").GetString();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                token);

        var content =
            new StringContent(
                "{",
                Encoding.UTF8,
                "application/json");

        var response =
            await _client.PostAsync(
                "/api/pulse/answer",
                content);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        var body =
            await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            400,
            body.GetProperty("status").GetInt32());

        Assert.Equal(
            "Invalid request.",
            body.GetProperty("message").GetString());
    }
}