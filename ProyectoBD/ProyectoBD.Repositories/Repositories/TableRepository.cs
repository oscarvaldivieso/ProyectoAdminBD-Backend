using ProyectoBD.Entities.Entities;
using ProyectoBD.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBD.Repositories.Repositories
{
    public class TableRepository
    {
        public async Task CrearTablaAsync(string databaseName, CreateTable table)
        {
            var server = "DESKTOP-QAOLMF0\\SQLEXPRESS";
            var connectionString = $"Server={server};Database={databaseName};Trusted_Connection=True;";

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
            var server = "DESKTOP-QAOLMF0\\SQLEXPRESS";
            var connectionString = $"Server={server};Database={databaseName};Trusted_Connection=True;";
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

    }
}
