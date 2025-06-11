using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBD.Repositories.Repositories
{
    public class DatabaseRepository : IDatabaseRepository
    {

        private readonly string _masterConnection;

        public DatabaseRepository(IConfiguration configuration)
        {
            _masterConnection = configuration.GetConnectionString("MasterConnection");
        }

        public async Task CreateDatabaseAsync(string dbName)
        {
            var sql = $"CREATE DATABASE [{dbName}]";

            using var connection = new SqlConnection(_masterConnection);
            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}
