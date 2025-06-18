using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace ProyectoBD.Repositories.Repositories
{
    public class DatabaseRepository 
    {
        public enum MotorBaseDatos
        {
            SqlServer,
            MySql
        }

        private readonly string _sqlServerConnectionString;
        private readonly string _mySqlConnectionString;

        public string servidor = "DESKTOP-LQVPKMF\\SQLEXPRESS";
        public DatabaseRepository(IConfiguration config)
        {
            _sqlServerConnectionString = config.GetConnectionString("SqlServerConnection");
            _mySqlConnectionString = config.GetConnectionString("MySqlConnection");
        }

        private async Task<SqlConnection> ObtenerConexionSqlServer(string databaseName)
        {
            var conn = new SqlConnection($"Server={servidor};Database={databaseName};Trusted_Connection=True;");
            await conn.OpenAsync();
            return conn;
        }

        private async Task<MySqlConnection> ObtenerConexionMySql(string databaseName)
        {
            var conn = new MySqlConnection($"{_mySqlConnectionString};Database={databaseName}");
            await conn.OpenAsync();
            return conn;
        }

        public async Task<bool> TestConnectionAsync(MotorBaseDatos motor)
        {
            try
            {
                switch (motor)
                {
                    case MotorBaseDatos.SqlServer:
                        using (var connection = new SqlConnection(_sqlServerConnectionString))
                        {
                            await connection.OpenAsync();
                            return true;
                        }

                    case MotorBaseDatos.MySql:
                        using (var connection = new MySqlConnection(_mySqlConnectionString))
                        {
                            await connection.OpenAsync();
                            return true;
                        }

                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task CrearBaseDeDatosAsync(string nombre, MotorBaseDatos motor)
        {
            switch (motor)
            {
                case MotorBaseDatos.SqlServer:
                    using (var connection = new SqlConnection(_sqlServerConnectionString))
                    {
                        await connection.OpenAsync();
                        var command = connection.CreateCommand();
                        command.CommandText = $"CREATE DATABASE [{nombre}]";
                        await command.ExecuteNonQueryAsync();
                    }
                    break;

                case MotorBaseDatos.MySql:
                    using (var connection = new MySqlConnection(_mySqlConnectionString))
                    {
                        await connection.OpenAsync();
                        var command = connection.CreateCommand();
                        command.CommandText = $"CREATE DATABASE `{nombre}`";
                        await command.ExecuteNonQueryAsync();
                    }
                    break;

                default:
                    throw new Exception("Motor de base de datos no soportado.");
            }
        }

        public async Task<List<string>> ListarBasesDeDatosAsync(MotorBaseDatos motor)
        {
            var basesDeDatos = new List<string>();

            switch (motor)
            {
                case MotorBaseDatos.SqlServer:
                    using (var connection = new SqlConnection(_sqlServerConnectionString))
                    {
                        await connection.OpenAsync();

                        var sql = "SELECT name FROM sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb');";
                        using var command = new SqlCommand(sql, connection);
                        using var reader = await command.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            basesDeDatos.Add(reader.GetString(0));
                        }
                    }
                    break;

                case MotorBaseDatos.MySql:
                    using (var connection = new MySqlConnection(_mySqlConnectionString))
                    {
                        await connection.OpenAsync();

                        var sql = "SHOW DATABASES;";
                        using var command = new MySqlCommand(sql, connection);
                        using var reader = await command.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            var dbName = reader.GetString(0);
                            if (!new[] { "information_schema", "mysql", "performance_schema", "sys" }.Contains(dbName))
                            {
                                basesDeDatos.Add(dbName);
                            }
                        }
                    }
                    break;

                default:
                    throw new Exception("Motor de base de datos no soportado.");
            }

            return basesDeDatos;
        }

    }
}
