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
    public class StoredProcedureRepository
    {
        public string servidor = "DESKTOP-BL38JV6";
        public async Task CrearSPAsync(string databaseName, StoredProcedures sp)
        {
            var connectionString = $"Server={servidor};Database={databaseName};Trusted_Connection=True;";

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var parametros = string.Join(", ", sp.parameters.Select(p=>$"@{p.Name} {p.datatype}"));

            var procedimiento = $@"create procedure[{sp.Name}]
            {parametros}
            as
            begin
            {sp.body}
            end";

            using var command = connection.CreateCommand();
            command.CommandText = procedimiento;
            await command.ExecuteNonQueryAsync();
        }
        public async Task AlterSPAsync(string databaseName, StoredProcedures sp)
        {
            var connectionString = $"Server={servidor};Database={databaseName};Trusted_Connection=True;";

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var parametros = string.Join(", ", sp.parameters.Select(p => $"@{p.Name} {p.datatype}"));

            var procedimiento = $@"alter procedure[{sp.Name}]
            {parametros}
            as
            begin
            {sp.body}
            end";

            using var command = connection.CreateCommand();
            command.CommandText = procedimiento;
            await command.ExecuteNonQueryAsync();
        }
        public async Task EliminarSPAsync(string databaseName, string nombreSP)
        {
            var connectionString = $"Server={servidor};Database={databaseName};Trusted_Connection=True;";

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Validación opcional para evitar inyección
            if (!Regex.IsMatch(nombreSP, @"^[a-zA-Z0-9_]+$"))
                throw new ArgumentException("Nombre de tabla inválido.");

            var dropSql = $"DROP PROCEDURE [{nombreSP}]";

            using var command = new SqlCommand(dropSql, connection);
            await command.ExecuteNonQueryAsync();
        }
        public async Task<List<string>> ListarSPAsync(string databasename)
        {
            var connectionString = $"Server={servidor};Database={databasename};Trusted_Connection=True;";

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            var procedimientos = new List<string>();
            var sql = "SELECT name FROM sys.procedures";

            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                procedimientos.Add(reader.GetString(0));
            }

            return procedimientos;
        }
    }
}
