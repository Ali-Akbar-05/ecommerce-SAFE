using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Persistence.EfCore;
using BuildingBlocks.Core.Messaging.MessagePersistence;
using BuildingBlocks.Persistence.EfCore.Postgres;
using Npgsql;

namespace Core.Persistence.Postgres;

public class NpgsqlConnectionFactory : IConnectionFactory
{
    private readonly string _connectionString;
    private readonly DatabaseType _databaseType;
    private DbConnection? _connection;

    public NpgsqlConnectionFactory(string connectionString, DatabaseType databaseType)
    {
        Guard.Against.NullOrEmpty(connectionString);
        _connectionString = connectionString;
        _databaseType = databaseType;
    }

    public async Task<DbConnection> GetOrCreateConnectionAsync()
    {
        if (_connection is null || _connection.State != ConnectionState.Open)
        {
            switch (_databaseType)
            {
                case DatabaseType.MSSQL:
                    _connection = new SqlConnection(_connectionString);
                    break;
                case DatabaseType.Postgres:
                    _connection = new NpgsqlConnection(_connectionString);
                    break;
                default:
                    _connection = new SqlConnection(_connectionString);
                    break;
            }

            await _connection.OpenAsync();
        }

        return _connection;
    }

    public void Dispose()
    {
        if (_connection is { State: ConnectionState.Open })
            _connection.Dispose();
    }
}
