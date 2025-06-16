using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Entities.Entities;
using ProyectoBD.Repositories.Entities;
using ProyectoBD.Repositories.Repositories;

namespace ProyectoBD.API.Controllers
{
    public class StoredProceduresController : Controller
    {
        private readonly StoredProcedureRepository _repository;

        public StoredProceduresController(StoredProcedureRepository repository)
        {
            _repository = repository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("create-sp/{databaseName}")]
        public async Task<IActionResult> CrearSP(string databaseName, [FromBody] StoredProcedures sp)
        {
            try
            {
                await _repository.CrearSPAsync(databaseName, sp);
                return Ok($"Procedimiento Almacenado '{sp.Name}' creado en la base de datos '{databaseName}'");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("alter-sp/{databaseName}")]
        public async Task<IActionResult> AlterSP(string databaseName, [FromBody] StoredProcedures sp)
        {
            try
            {
                await _repository.AlterSPAsync(databaseName, sp);
                return Ok($"Procedimiento Almacenado '{sp.Name}' modificado en la base de datos '{databaseName}'");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("drop-sp")]
        public async Task<IActionResult> EliminarSP(string databaseName, string nombreSP)
        {
            try
            {
                await _repository.EliminarSPAsync(databaseName, nombreSP);
                return Ok($"Procedimiento '{nombreSP}' eliminado correctamente de la base '{databaseName}'.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("list-sp")]
        public async Task<IActionResult> ListarSP(string databaseName)
        {
            try
            {
                var tablas = await _repository.ListarSPAsync(databaseName);
                return Ok(tablas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
