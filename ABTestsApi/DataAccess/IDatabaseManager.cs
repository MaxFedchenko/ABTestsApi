using Microsoft.Data.SqlClient;

namespace ABTestsApi.DataAccess
{
    public interface IDatabaseManager
    {
        Task<T?> ExecuteScalar<T>(string storedProcedureName, SqlParameter[]? parameters = null);
        IAsyncEnumerable<SqlDataReader> ExecuteReader(string storedProcedureName, SqlParameter[]? parameters = null);
        Task ExecuteNonQuery(string storedProcedureName, SqlParameter[]? parameters = null);
    }
}