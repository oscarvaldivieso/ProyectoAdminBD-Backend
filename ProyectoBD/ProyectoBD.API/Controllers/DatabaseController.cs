using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Repositories.Repositories;

namespace ProyectoBD.API.Controllers
{
    public class DatabaseController : Controller
    {
        private readonly DatabaseRepository _repository;

        public DatabaseController(DatabaseRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("TestConnection")]
        public async Task<IActionResult> TestConnection()
        {
            var success = await _repository.TestConnectionAsync();
            return Ok(success ? "Conexión exitosa" : "Fallo en la conexión");
        }


        [HttpPost("{nombre}")]
        public async Task<IActionResult> Crear(string nombre)
        {
            await _repository.CrearBaseDeDatosAsync(nombre);
            return Ok($"Base de datos '{nombre}' creada.");
        }


        [HttpGet("databases-list")]
        public async Task<IActionResult> Listar()
        {
            var lista = await _repository.ListarBasesDeDatosAsync();
            return Ok(lista);
        }

    }
}
