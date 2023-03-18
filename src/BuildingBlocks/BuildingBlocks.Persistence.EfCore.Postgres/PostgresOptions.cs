using BuildingBlocks.Core.Messaging.MessagePersistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence.EfCore.Postgres;

public class PostgresOptions
{
    public DatabaseType DatabaseType { get; set; } = default!;
    public string ConnectionString { get; set; } = default!;
    public bool UseInMemory { get; set; }
    public string? MigrationAssembly { get; set; } = null!;
}
