using ProyectoBD.Entities.Entities;
using ProyectoBD.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProyectoBD.Repositories.Repositories
{
    public class TableRepository
    {
        public string servidor = "DESKTOP-FONUA2P\\SQLEXPRESS";
        public async Task CrearTablaAsync(string databaseName, CreateTable table)
        {
            var connectionString = $"Server={servidor};Database={databaseName};Trusted_Connection=True;";

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var columnsSql = table.Columns.Select(c =>
            {
                var nullable = c.IsNullable ? "NULL" : "NOT NULL";
                return $"[{c.Name}] {c.DataType} {nullable}".Trim();
            });

            var primaryKeys = table.Columns
                .Where(c => c.IsPrimaryKey)
                .Select(c => $"[{c.Name}]");

            var pkSql = primaryKeys.Any() ? $", PRIMARY KEY ({string.Join(", ", primaryKeys)})" : "";

            var createTableSql = $"CREATE TABLE [{table.TableName}] ({string.Join(", ", columnsSql)}{pkSql});";

            using var command = connection.CreateCommand();
            command.CommandText = createTableSql;
            await command.ExecuteNonQueryAsync();
        }

        public async Task AlterTableAsync(string databaseName, AlterTable alterTable)
        {
            var server = "DESKTOP-FONUA2P\\SQLEXPRESS";
            var connectionString = $"Server={servidor};Database={databaseName};Trusted_Connection=True;";
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            foreach (var alteration in alterTable.Alterations)
            {
                string sql = alteration.Operation switch
                {
                    "ADD" => $"ALTER TABLE [{alterTable.TableName}] ADD [{alteration.ColumnName}] {alteration.DataType} {(alteration.IsNullable.HasValue && alteration.IsNullable.Value ? "NULL" : "NOT NULL")};",
                    "DROP" => $"ALTER TABLE [{alterTable.TableName}] DROP COLUMN [{alteration.ColumnName}];",
                    "ALTER" => $"ALTER TABLE [{alterTable.TableName}] ALTER COLUMN [{alteration.ColumnName}] {alteration.DataType} {(alteration.IsNullable.HasValue && alteration.IsNullable.Value ? "NULL" : "NOT NULL")};",
                    _ => throw new InvalidOperationException("Operación no soportada")
                };
                using var command = connection.CreateCommand();
                command.CommandText = sql;
                await command.ExecuteNonQueryAsync();
            }
        }


        public async Task EliminarTablaAsync(string databaseName, string nombreTabla)
        {
            var server = "DESKTOP-FONUA2P\\SQLEXPRESS";
            var connectionString = $"Server={server};Database={databaseName};Trusted_Connection=True;";

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Validación opcional para evitar inyección
            if (!Regex.IsMatch(nombreTabla, @"^[a-zA-Z0-9_]+$"))
                throw new ArgumentException("Nombre de tabla inválido.");

            var dropSql = $"DROP TABLE [{nombreTabla}]";

            using var command = new SqlCommand(dropSql, connection);
            await command.ExecuteNonQueryAsync();
        }


        public async Task<List<string>> ListarTablasAsync(string databaseName)
        {
            var server = "DESKTOP-FONUA2P\\SQLEXPRESS";
            var connectionString = $"Server={servidor};Database={databaseName};Trusted_Connection=True;";

            var tablas = new List<string>();

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var sql = @"SELECT TABLE_NAME 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_TYPE = 'BASE TABLE'";

            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                tablas.Add(reader.GetString(0));
            }

            return tablas;
        }


    }
}
