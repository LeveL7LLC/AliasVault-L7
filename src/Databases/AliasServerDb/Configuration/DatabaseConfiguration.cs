//-----------------------------------------------------------------------
// <copyright file="DatabaseConfiguration.cs" company="lanedirt">
// Copyright (c) lanedirt. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace AliasServerDb.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Database configuration class.
/// </summary>
public static class DatabaseConfiguration
{
    /// <summary>
    /// Configures SQLite for use with Entity Framework Core.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the DbContext to.</param>
    /// <param name="configuration">The IConfiguration to use for the connection string.</param>
    /// <returns>The IServiceCollection for method chaining.</returns>
    public static IServiceCollection AddAliasVaultDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Check for environment variable first, then fall back to configuration
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__AliasServerDbContext");
        var dbProvider = Environment.GetEnvironmentVariable("DatabaseProvider")?.ToLower()
            ?? configuration.GetValue<string>("DatabaseProvider")?.ToLower()
            ?? "postgresql";

        // Create a new configuration if we have an environment-provided connection string
        if (!string.IsNullOrEmpty(connectionString))
        {
            var configDictionary = new Dictionary<string, string?>
            {
                ["ConnectionStrings:AliasServerDbContext"] = connectionString,
            };

            var configurationBuilder = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddInMemoryCollection(configDictionary);

            configuration = configurationBuilder.Build();
        }

        // Add custom DbContextFactory registration which supports multiple database providers
        switch (dbProvider)
        {
            case "postgresql":
                services.AddSingleton<IAliasServerDbContextFactory, PostgresqlDbContextFactory>();
                break;
            case "sqlite":
            default:
                services.AddSingleton<IAliasServerDbContextFactory, SqliteDbContextFactory>();
                break;
        }

        // Updated DbContextFactory registration
        services.AddDbContextFactory<AliasServerDbContext>((sp, options) =>
        {
            var factory = sp.GetRequiredService<IAliasServerDbContextFactory>();
            factory.ConfigureDbContextOptions(options);
        });

        // Add scoped DbContext registration based on the factory
        services.AddScoped<AliasServerDbContext>(sp =>
        {
            var factory = sp.GetRequiredService<IAliasServerDbContextFactory>();
            return factory.CreateDbContext();
        });

        return services;
    }
}
