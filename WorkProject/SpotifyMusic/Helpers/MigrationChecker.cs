using Microsoft.EntityFrameworkCore;

namespace WorkProject.Helpers;

public static class MigrationChecker
{
    public static void CheckAndApplyMigrations<TContext>(
        IServiceProvider serviceProvider,
        ILogger logger) where TContext : DbContext //обмежує, щоб це завжди був нащадок DbContext
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

        var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
        if (pendingMigrations.Any())
        {
            logger.LogWarning("There is {Count} unfulfilled migrations:", pendingMigrations.Count);

            foreach (var migration in pendingMigrations)
            {
                var datePart = migration.Length >= 8 ? migration.Substring(0, 8) : "unknown";
                logger.LogWarning(" - {Migration} (Date: {Date})", migration, datePart);
            }

            dbContext.Database.Migrate();
            logger.LogInformation("Migrations were successfully applied.");
        }
        else
            logger.LogInformation("All migrations have already been applied.");
    }
}