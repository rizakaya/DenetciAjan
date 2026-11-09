using DenetciAjan.Api.Health;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DenetciAjan")
    ?? throw new InvalidOperationException("ConnectionStrings:DenetciAjan yapılandırılmamış.");
var ollamaBaseUrl = builder.Configuration["Ollama:BaseUrl"]
    ?? throw new InvalidOperationException("Ollama:BaseUrl yapılandırılmamış.");

builder.Services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));
builder.Services.AddHttpClient(OllamaHealthCheck.HttpClientName, client =>
{
    client.BaseAddress = new Uri(ollamaBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddHealthChecks()
    .AddCheck<PostgresHealthCheck>("postgres", tags: ["ready"])
    .AddCheck<OllamaHealthCheck>("ollama", tags: ["ready"]);

var app = builder.Build();

// Liveness: süreç ayakta mı? Bağımlılıkları kontrol etmez.
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });

// Readiness: PostgreSQL ve Ollama erişilebilir mi?
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = HealthResponseWriter.WriteJsonAsync
});

app.Run();
