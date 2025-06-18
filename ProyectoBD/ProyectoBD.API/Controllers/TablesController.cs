using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Entities.Entities;
using ProyectoBD.Repositories.Entities;
using ProyectoBD.Repositories.Repositories;
using static ProyectoBD.Repositories.Repositories.TableRepository;

namespace ProyectoBD.API.Controllers
{
    public class TablesController : Controller
    {
        private readonly TableRepository _repository;

        public TablesController(TableRepository repository)
        {
            _repository = repository;
        }

        //Helper
        private MotorBaseDatos ObtenerMotor(string motor)
        {
            return motor?.ToLower() switch
            {
                "sqlserver" => MotorBaseDatos.SqlServer,
                "mysql" => MotorBaseDatos.MySql,
                _ => throw new ArgumentException("Motor de base de datos no soportado. Usa 'sqlserver' o 'mysql'.")
            };
        }




        [HttpPost("create-table/{databaseName}")]
        public async Task<IActionResult> CrearTabla(string databaseName, [FromQuery] string motor, [FromBody] CreateTable table)
        {
            try
            {
                var motorEnum = ObtenerMotor(motor);
                await _repository.CrearTablaAsync(databaseName, table, motorEnum);
                return Ok($"Tabla '{table.TableName}' creada en la base de datos '{databaseName}'");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("alter-table/{databaseName}")]
        public async Task<IActionResult> AlterTable(string databaseName, [FromQuery] string motor, [FromBody] AlterTable alterTable)
        {
            try
            {
                var motorEnum = ObtenerMotor(motor);
                await _repository.AlterTableAsync(databaseName, alterTable, motorEnum);
                return Ok($"Tabla '{alterTable.TableName}' alterada en la base de datos '{databaseName}'");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete-table")]
        public async Task<IActionResult> EliminarTabla(string databaseName, string nombreTabla, [FromQuery] string motor)
        {
            try
            {
                var motorEnum = ObtenerMotor(motor);
                await _repository.EliminarTablaAsync(databaseName, nombreTabla, motorEnum);
                return Ok($"Tabla '{nombreTabla}' eliminada correctamente de la base '{databaseName}'.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("list-tables")]
        public async Task<IActionResult> ListarTablas(string databaseName, [FromQuery] string motor)
        {
            try
            {
                var motorEnum = ObtenerMotor(motor);
                var tablas = await _repository.ListarTablasAsync(databaseName, motorEnum);
                return Ok(tablas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        public IActionResult Index()
        {
            return View();
        }


        //DML Operations

        [HttpPost("insert")]
        public async Task<IActionResult> Insert([FromBody] InsertRequest request)
        {
            try
            {
                await _repository.InsertAsync(request);
                return Ok("Insert realizado con éxito.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al insertar: {ex.Message}");
            }
        }

    }
}
