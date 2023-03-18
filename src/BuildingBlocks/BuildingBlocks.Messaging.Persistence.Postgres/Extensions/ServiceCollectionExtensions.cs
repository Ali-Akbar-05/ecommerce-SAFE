using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using BuildingBlocks.Core.Messaging.MessagePersistence;
using BuildingBlocks.Core.Web.Extenions.ServiceCollection;
using BuildingBlocks.Messaging.Persistence.Postgres.MessagePersistence;
using BuildingBlocks.Persistence.EfCore.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Messaging.Persistence.Postgres.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPostgresMessagePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var dbType = configuration.GetValue<DatabaseType>("PostgresOptions:DatabaseType");
        if (dbType == DatabaseType.Postgres)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        services.AddValidatedOptions<MessagePersistenceOptions>(nameof(MessagePersistenceOptions));

        services.AddScoped<IMessagePersistenceConnectionFactory>(sp =>
        {
            var postgresOptions = sp.GetService<MessagePersistenceOptions>();
            Guard.Against.NullOrEmpty(postgresOptions?.ConnectionString);

            return new NpgsqlMessagePersistenceConnectionFactory(postgresOptions.ConnectionString, postgresOptions.DatabaseType);
        });

        services.AddDbContext<MessagePersistenceDbContext>(
            (sp, options) =>
            {
                var postgresOptions = sp.GetRequiredService<MessagePersistenceOptions>();
                switch (postgresOptions.DatabaseType)
                {
                    case DatabaseType.MSSQL:
                        options.UseSqlServer(
                       postgresOptions.ConnectionString,
                       sqlOptions =>
                       {
                           sqlOptions.MigrationsAssembly(
                               postgresOptions.MigrationAssembly
                                   ?? typeof(MessagePersistenceDbContext).Assembly.GetName().Name
                           );
                           sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                       }).UseSnakeCaseNamingConvention();
                        break;

                    case DatabaseType.Postgres:
                        options.UseNpgsql(postgresOptions.ConnectionString,
                           sqlOptions =>
                           {
                               sqlOptions.MigrationsAssembly(
                                   postgresOptions.MigrationAssembly
                                       ?? typeof(MessagePersistenceDbContext).Assembly.GetName().Name
                               );
                               sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                           }).UseSnakeCaseNamingConvention();
                        break;
                }

            }
        );

        services.ReplaceScoped<IMessagePersistenceRepository, PostgresMessagePersistenceRepository>();
    }
}
