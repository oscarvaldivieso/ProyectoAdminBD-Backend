using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Repositories.Repositories;
using static ProyectoBD.Repositories.Repositories.DatabaseRepository;
using ProyectoBD.Entities.Entities;

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
        public async Task<IActionResult> TestConnection([FromQuery] MotorBaseDatos motor)
        {
            var success = await _repository.TestConnectionAsync(motor);
            return Ok(success ? "Conexión exitosa" : "Fallo en la conexión");
        }

        // POST: api/Database/crear?nombre=MiBD&motor=SqlServer
        [HttpPost("create")]
        public async Task<IActionResult> Crear([FromQuery] string nombre, [FromQuery] MotorBaseDatos motor)
        {
            await _repository.CrearBaseDeDatosAsync(nombre, motor);
            return Ok($"Base de datos '{nombre}' creada en {motor}.");
        }

        // GET: api/Database/databases-list?motor=SqlServer
        [HttpGet("databases-list")]
        public async Task<IActionResult> Listar([FromQuery] MotorBaseDatos motor)
        {
            var lista = await _repository.ListarBasesDeDatosAsync(motor);
            return Ok(lista);
        }

    }
}
