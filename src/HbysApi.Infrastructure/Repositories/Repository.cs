// Repository.cs
#nullable enable
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HbysApi.Infrastructure.Repositories
{
    public class Repository
    {
        private readonly string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null)
        {
            using var connection = new OracleConnection(_connectionString);
            using var command = new OracleCommand(query, connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.Add(new OracleParameter(param.Key, param.Value));
                }
            }

            var dataTable = new DataTable();
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<List<Dictionary<string, object>>> ExecuteQueryAsListAsync(string query, Dictionary<string, object>? parameters = null)
        {
            using var connection = new OracleConnection(_connectionString);
            using var command = new OracleCommand(query, connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.Add(new OracleParameter(param.Key, param.Value));
                }
            }

            var resultList = new List<Dictionary<string, object>>();
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.GetValue(i);
                }
                resultList.Add(row);
            }

            return resultList;
        }
    }
}
