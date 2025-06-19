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
        public async Task<IActionResult> EliminarTabla([FromQuery] string databaseName, [FromQuery] string nombreTabla, [FromQuery] MotorBaseDatos motor)
        {
            var resultado = await _repository.EliminarTablaAsync(databaseName, nombreTabla, motor);
            return Ok(resultado);
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

        [HttpPost("ejecutar-dml")]
        public async Task<IActionResult> EjecutarDML([FromBody] InsertRequest request)
        {
            try
            {
                await _repository.EjecutarComandoDMLAsync(request.DatabaseName, request.Sql, request.Motor);
                return Ok(new { mensaje = "Sentencia ejecutada correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al ejecutar la sentencia.", error = ex.Message });
            }
        }



        [HttpPost("consultar")]
        public async Task<IActionResult> Consultar([FromBody] ConsultaTablaRequest request)
        {
            try
            {
                var registros = await _repository.ObtenerRegistrosAsync(request.DatabaseName, request.TableName, request.Motor);
                return Ok(registros);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("relation-create")]
        public async Task<IActionResult> CrearRelacion([FromBody] RelacionRequest request)
        {
            try
            {
                await _repository.CrearRelacionAsync(request);
                return Ok(new { mensaje = "Relación creada correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("columns-list")]
        public async Task<IActionResult> ListarColumnas([FromBody] ColumnasRequest request)
        {
            try
            {
                var columnas = await _repository.ObtenerColumnasAsync(request.DatabaseName, request.TableName, request.Motor);
                return Ok(columnas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }


        [HttpPost("script")]
        public async Task<IActionResult> EjecutarScript([FromBody] SqlLibreRequest request)
        {
            try
            {
                await _repository.EjecutarScriptAsync(request);
                return Ok(new { mensaje = "Script ejecutado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message, error = ex.StackTrace });
            }
        }





    }
}
