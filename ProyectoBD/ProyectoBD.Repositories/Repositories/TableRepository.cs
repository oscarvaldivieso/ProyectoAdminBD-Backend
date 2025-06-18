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
using ProyectoBD.Entities.Entities;

namespace ProyectoBD.Repositories.Repositories
{
    

    public class TableRepository
    {
        private readonly string _sqlServerConnectionString;
        private readonly string _mySqlConnectionString;

        public TableRepository(IConfiguration config)
        {
            _sqlServerConnectionString = config.GetConnectionString("SqlServerConnection");
            _mySqlConnectionString = config.GetConnectionString("MySqlConnection");
        }

        public string servidor = "OSCARVALDIVIESO";
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

        public async Task EjecutarInsertAsync(string databaseName, string sql, MotorBaseDatos motor)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("La consulta SQL no puede estar vacía.");

            var trimmedSql = sql.TrimStart().ToUpperInvariant();

            if (!trimmedSql.StartsWith("INSERT INTO"))
                throw new ArgumentException("Solo se permiten sentencias que comienzan con 'INSERT INTO'.");

            if (!trimmedSql.Contains("VALUES"))
                throw new ArgumentException("La sentencia INSERT debe contener la cláusula 'VALUES'.");

            if (sql.Contains(";"))
                throw new ArgumentException("No se permiten múltiples sentencias (no uses ';').");

            // Validaciones por motor
            if (motor == MotorBaseDatos.MySql && sql.Contains("IDENTITY", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("'IDENTITY' es una palabra reservada de SQL Server, no válida en MySQL.");

            if (motor == MotorBaseDatos.SqlServer && sql.Contains("AUTO_INCREMENT", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("'AUTO_INCREMENT' es una palabra reservada de MySQL, no válida en SQL Server.");

            // Ejecución segura
            using var connection = await AbrirConexionAsync(databaseName, motor);
            using var command = CrearComando(connection, sql);
            await command.ExecuteNonQueryAsync();
        }


        public async Task<List<Dictionary<string, object>>> ObtenerRegistrosAsync(string databaseName, string tableName, MotorBaseDatos motor)
        {
            if (string.IsNullOrWhiteSpace(tableName) || !Regex.IsMatch(tableName, @"^[a-zA-Z0-9_]+$"))
                throw new ArgumentException("Nombre de tabla inválido.");

            using var connection = await AbrirConexionAsync(databaseName, motor);
            string sql = $"SELECT * FROM {(motor == MotorBaseDatos.MySql ? $"`{tableName}`" : $"[{tableName}]")}";

            using var command = CrearComando(connection, sql);
            using var reader = await command.ExecuteReaderAsync();

            var resultados = new List<Dictionary<string, object>>();

            while (await reader.ReadAsync())
            {
                var fila = new Dictionary<string, object>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var nombreColumna = reader.GetName(i);
                    var valor = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    fila[nombreColumna] = valor;
                }

                resultados.Add(fila);
            }

            return resultados;
        }


        public async Task CrearRelacionAsync(RelacionRequest request)
        {
            using var connection = await AbrirConexionAsync(request.DatabaseName, request.Motor);

            string wrap(string nombre) => request.Motor == MotorBaseDatos.MySql ? $"`{nombre}`" : $"[{nombre}]";

            string sql = $"ALTER TABLE {wrap(request.TablaOrigen)} " +
                         $"ADD CONSTRAINT {wrap(request.NombreRelacion)} " +
                         $"FOREIGN KEY ({wrap(request.ColumnaOrigen)}) " +
                         $"REFERENCES {wrap(request.TablaReferencia)} ({wrap(request.ColumnaReferencia)});";

            using var command = CrearComando(connection, sql);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<string>> ObtenerColumnasAsync(string databaseName, string tableName, MotorBaseDatos motor)
        {
            if (string.IsNullOrWhiteSpace(tableName) || !Regex.IsMatch(tableName, @"^[a-zA-Z0-9_]+$"))
                throw new ArgumentException("Nombre de tabla inválido.");

            using var connection = await AbrirConexionAsync(databaseName, motor);

            string sql = motor switch
            {
                MotorBaseDatos.SqlServer =>
                    $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'",

                MotorBaseDatos.MySql =>
                    $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '{databaseName}' AND TABLE_NAME = '{tableName}'",

                _ => throw new Exception("Motor no soportado")
            };

            using var command = CrearComando(connection, sql);
            using var reader = await command.ExecuteReaderAsync();

            var columnas = new List<string>();

            while (await reader.ReadAsync())
            {
                columnas.Add(reader.GetString(0));
            }

            return columnas;
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
