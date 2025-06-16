using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Entities.Entities;
using ProyectoBD.Repositories.Entities;
using ProyectoBD.Repositories.Repositories;

namespace ProyectoBD.API.Controllers
{
    public class TablesController : Controller
    {
        private readonly TableRepository _repository;

        public TablesController(TableRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("create-table/{databaseName}")]
        public async Task<IActionResult> CrearTabla(string databaseName, [FromBody] CreateTable table)
        {
            try
            {
                await _repository.CrearTablaAsync(databaseName, table);
                return Ok($"Tabla '{table.TableName}' creada en la base de datos '{databaseName}'");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("alter-table/{databaseName}")]
        public async Task<IActionResult> AlterTable(string databaseName, [FromBody] AlterTable alterTable)
        {
            try
            {
                await _repository.AlterTableAsync(databaseName, alterTable);
                return Ok($"Tabla '{alterTable.TableName}' alterada en la base de datos '{databaseName}'");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("delete-table")]
        public async Task<IActionResult> EliminarTabla(string databaseName, string nombreTabla)
        {
            try
            {
                await _repository.EliminarTablaAsync(databaseName, nombreTabla);
                return Ok($"Tabla '{nombreTabla}' eliminada correctamente de la base '{databaseName}'.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("list-tables")]
        public async Task<IActionResult> ListarTablas(string databaseName)
        {
            try
            {
                var tablas = await _repository.ListarTablasAsync(databaseName);
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
