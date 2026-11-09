using System.Net.Http.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DenetciAjan.Api.Health;

public sealed class OllamaHealthCheck(IHttpClientFactory httpClientFactory) : IHealthCheck
{
    public const string HttpClientName = "ollama";

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var client = httpClientFactory.CreateClient(HttpClientName);
            var response = await client.GetFromJsonAsync<OllamaVersionResponse>("api/version", cancellationToken);
            return HealthCheckResult.Healthy($"Ollama {response?.Version}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Ollama erişilemiyor.", ex);
        }
    }

    private sealed record OllamaVersionResponse(string Version);
}
