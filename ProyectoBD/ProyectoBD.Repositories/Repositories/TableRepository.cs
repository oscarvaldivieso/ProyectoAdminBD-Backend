using ProyectoBD.Entities.Entities;
using ProyectoBD.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;


namespace ProyectoBD.Repositories.Repositories
{
    public class TableRepository
    {
        public enum MotorBaseDatos
        {
            SqlServer,
            MySql
        }

        private readonly string _sqlServerConnectionString;
        private readonly string _mySqlConnectionString;

        public TableRepository(IConfiguration config)
        {
            _sqlServerConnectionString = config.GetConnectionString("SqlServerConnection");
            _mySqlConnectionString = config.GetConnectionString("MySqlConnection");
        }

        public string servidor = "DESKTOP-LQVPKMF\\SQLEXPRESS";
        public async Task CrearTablaAsync(string databaseName, CreateTable table, MotorBaseDatos motor)
        {
            using var connection = await AbrirConexionAsync(databaseName, motor);

            string Wrap(string name) => motor == MotorBaseDatos.MySql ? $"`{name}`" : $"[{name}]";

            var columnDefinitions = table.Columns.Select(c =>
            {
                if (string.IsNullOrWhiteSpace(c.DataType))
                    throw new ArgumentException($"La columna '{c.Name}' no tiene un tipo de dato definido.");

                var nullable = c.IsNullable ? "NULL" : "NOT NULL";
                return $"{Wrap(c.Name)} {c.DataType} {nullable}";
            }).ToList();

            var primaryKeys = table.Columns
                .Where(c => c.IsPrimaryKey)
                .Select(c => Wrap(c.Name))
                .ToList();

            if (primaryKeys.Any())
            {
                columnDefinitions.Add($"PRIMARY KEY ({string.Join(", ", primaryKeys)})");
            }

            var createTableSql = $"CREATE TABLE {Wrap(table.TableName)} ({string.Join(", ", columnDefinitions)});";

            Console.WriteLine(createTableSql); // Para depuración

            using var command = CrearComando(connection, createTableSql);
            await command.ExecuteNonQueryAsync();
        }

        public async Task AlterTableAsync(string databaseName, AlterTable alterTable, MotorBaseDatos motor)
        {
            using var connection = await AbrirConexionAsync(databaseName, motor);

            foreach (var alteration in alterTable.Alterations)
            {
                string sql = alteration.Operation switch
                {
                    "ADD" => $"ALTER TABLE `{alterTable.TableName}` ADD `{alteration.ColumnName}` {alteration.DataType} {(alteration.IsNullable.GetValueOrDefault() ? "NULL" : "NOT NULL")};",
                    "DROP" => $"ALTER TABLE `{alterTable.TableName}` DROP COLUMN `{alteration.ColumnName}`;",
                    "ALTER" => $"ALTER TABLE `{alterTable.TableName}` MODIFY `{alteration.ColumnName}` {alteration.DataType} {(alteration.IsNullable.GetValueOrDefault() ? "NULL" : "NOT NULL")};",
                    _ => throw new InvalidOperationException("Operación no soportada")
                };

                using var command = CrearComando(connection, sql);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarTablaAsync(string databaseName, string nombreTabla, MotorBaseDatos motor)
        {
            if (!Regex.IsMatch(nombreTabla, @"^[a-zA-Z0-9_]+$"))
                throw new ArgumentException("Nombre de tabla inválido.");

            using var connection = await AbrirConexionAsync(databaseName, motor);
            var dropSql = $"DROP TABLE `{nombreTabla}`";

            using var command = CrearComando(connection, dropSql);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<string>> ListarTablasAsync(string databaseName, MotorBaseDatos motor)
        {
            using var connection = await AbrirConexionAsync(databaseName, motor);
            var tablas = new List<string>();

            string sql = motor switch
            {
                MotorBaseDatos.SqlServer => "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'",
                MotorBaseDatos.MySql => "SHOW TABLES",
                _ => throw new Exception("Motor no soportado")
            };

            using var command = CrearComando(connection, sql);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                tablas.Add(reader.GetString(0));
            }

            return tablas;
        }


        //DML Operations

        public async Task InsertAsync(InsertRequest request)
        {
            var connectionString = $"Server= {servidor} ;Database={request.DatabaseName};Trusted_Connection=True;";

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var columns = string.Join(", ", request.Data.Keys.Select(k => $"[{k}]"));
            var parameters = string.Join(", ", request.Data.Keys.Select(k => $"@{k}"));

            var sql = $"INSERT INTO [{request.TableName}] ({columns}) VALUES ({parameters})";

            using var command = new SqlCommand(sql, connection);

            foreach (var pair in request.Data)
            {
                var paramValue = ConvertJsonElement(pair.Value);

                command.Parameters.AddWithValue($"@{pair.Key}", paramValue ?? DBNull.Value);
            }

            await command.ExecuteNonQueryAsync();
        }





        // Helpers

        private async Task<IDbConnection> AbrirConexionAsync(string databaseName, MotorBaseDatos motor)
        {
            IDbConnection conn = motor switch
            {
                MotorBaseDatos.SqlServer => new SqlConnection($"{_sqlServerConnectionString};Database={databaseName}"),
                MotorBaseDatos.MySql => new MySqlConnection($"{_mySqlConnectionString};Database={databaseName}"),
                _ => throw new Exception("Motor no soportado")
            };

            await ((dynamic)conn).OpenAsync(); // cast dinámico para usar OpenAsync
            return conn;
        }

        private dynamic CrearComando(IDbConnection connection, string sql)
        {
            return connection switch
            {
                SqlConnection sqlConn => new SqlCommand(sql, sqlConn),
                MySqlConnection mySqlConn => new MySqlCommand(sql, mySqlConn),
                _ => throw new Exception("Conexión no soportada")
            };
        }

        private object ConvertJsonElement(object value)
        {
            if (value is JsonElement jsonElement)
            {
                return jsonElement.ValueKind switch
                {
                    JsonValueKind.String => jsonElement.GetString(),
                    JsonValueKind.Number => jsonElement.TryGetInt32(out var i) ? i :
                                            jsonElement.TryGetDouble(out var d) ? d : jsonElement.ToString(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => DBNull.Value,
                    _ => jsonElement.ToString()
                };
            }

            return value;
        }


    }
}
