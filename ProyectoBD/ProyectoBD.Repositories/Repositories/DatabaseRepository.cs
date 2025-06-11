using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace ProyectoBD.Repositories.Repositories
{
    public class DatabaseRepository 
    {
        private readonly string _connectionString;

        public DatabaseRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("MasterConnection");
        }

        public async Task<bool> TestConnectionAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            try
            {
                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task CrearBaseDeDatosAsync(string nombre)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE [{nombre}]";
            await command.ExecuteNonQueryAsync();
        }
    }
}
