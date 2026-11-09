using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace DenetciAjan.Api.Health;

public sealed class PostgresHealthCheck(NpgsqlDataSource dataSource) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var command = dataSource.CreateCommand("SELECT version()");
            var version = (string?)await command.ExecuteScalarAsync(cancellationToken);
            return HealthCheckResult.Healthy(version);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL erişilemiyor.", ex);
        }
    }
}
