using ProyectoBD.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBD.Repositories.Repositories
{
    public class ViewRepository

    {
        public string servidor = "LUISEDUARDOLOPE\\SQLEXPRESS";
        public string GenerarSqlDeVista(CreateViewRequest request)
        {
            var select = string.Join(", ", request.SelectedColumns.Select(c =>
            {
                var alias = !string.IsNullOrEmpty(c.Alias) ? $" AS [{c.Alias}]" : "";
                return $"[{c.Table}].[{c.Column}]{alias}";
            }));

            var from = $"[{request.SelectedTables.First()}]";

            string joins = "";
            if (request.Joins != null && request.Joins.Any())
            {
                foreach (var join in request.Joins)
                {
                    joins += $" JOIN [{join.RightTable}] ON [{join.LeftTable}].[{join.LeftColumn}] = [{join.RightTable}].[{join.RightColumn}]";
                }
            }

            string where = "";
            if (request.Filters != null && request.Filters.Any())
            {
                var conditions = request.Filters.Select(f =>
                {
                    var value = f.Operator.ToUpper() == "LIKE" ? $"'%{f.Value}%'" : $"'{f.Value}'";
                    return $"[{f.Table}].[{f.Column}] {f.Operator} {value}";
                });
                where = $" WHERE {string.Join(" AND ", conditions)}";
            }

            var sql = $@"
CREATE VIEW [{request.ViewName}] AS
SELECT {select}
FROM {from}
{joins}
{where};
";
            return sql;
        }

    
    public async Task CrearVistaAsync(string databaseName, CreateViewRequest request)
        {
            var sql = GenerarSqlDeVista(request);

            var connectionString = $"Server={servidor};Database={databaseName};Trusted_Connection=True;";
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }


    }
}

