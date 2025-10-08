using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace SGMCJ.Persistence.Common
{
    public class StoredProcedureExecutor
    {
        private readonly string _connString;

        public StoredProcedureExecutor(IConfiguration cfg)
        {
            _connString = cfg.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Falta ConnectionString 'DefaultConnection'.");
        }

        public async Task<SqlDataReader> ExecuteReaderAsync(string spName, params (string Name, object? Value)[] parameters)
        {
            var conn = new SqlConnection(_connString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure };
            foreach (var (name, value) in parameters)
                cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);

            return await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);
        }

        public async Task<int> ExecuteNonQueryAsync(string spName, params (string Name, object? Value)[] parameters)
        {
            using var conn = new SqlConnection(_connString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure };
            foreach (var (name, value) in parameters)
                cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<T?> ExecuteScalarAsync<T>(string spName, params (string Name, object? Value)[] parameters)
        {
            using var conn = new SqlConnection(_connString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure };
            foreach (var (name, value) in parameters)
                cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();
            return (result == null || result == DBNull.Value) ? default : (T)result;
        }
    }
}