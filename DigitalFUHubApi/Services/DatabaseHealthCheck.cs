using BusinessObject;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DigitalFUHubApi.Services
{
	// Used by GET /health (deploy pipeline + uptime checks): healthy only when the database answers
	public class DatabaseHealthCheck : IHealthCheck
	{
		private readonly DatabaseContext context;

		public DatabaseHealthCheck(DatabaseContext context)
		{
			this.context = context;
		}

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext healthContext, CancellationToken cancellationToken = default)
		{
			return await context.Database.CanConnectAsync(cancellationToken)
				? HealthCheckResult.Healthy()
				: HealthCheckResult.Unhealthy("Cannot connect to the database");
		}
	}
}
