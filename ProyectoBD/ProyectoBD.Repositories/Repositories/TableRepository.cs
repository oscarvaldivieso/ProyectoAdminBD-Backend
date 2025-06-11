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
            var server = "DESKTOP-LQVPKMF\\SQLEXPRESS";
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

    }
}
