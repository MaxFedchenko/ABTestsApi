using Microsoft.Data.SqlClient;
using System.Data;

namespace ABTestsApi.DataAccess
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<T?> ExecuteScalar<T>(string storedProcedureName, SqlParameter[]? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(storedProcedureName, connection);

            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            object? result = await command.ExecuteScalarAsync();

            if (result != null && result != DBNull.Value)
            {
                return (T)result;
            }

            return default;
        }
        public async IAsyncEnumerable<SqlDataReader> ExecuteReader(string storedProcedureName, SqlParameter[]? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(storedProcedureName, connection);

            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) 
            {
                yield return reader;
            }
        }
        public async Task ExecuteNonQuery(string storedProcedureName, SqlParameter[]? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(storedProcedureName, connection);

            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            await command.ExecuteNonQueryAsync();
        }
    }
}
