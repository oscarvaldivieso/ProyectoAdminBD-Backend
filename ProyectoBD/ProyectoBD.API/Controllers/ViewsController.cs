using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Entities.Entities;
using ProyectoBD.Repositories.Repositories;

namespace ProyectoBD.API.Controllers
{
    public class ViewsController : Controller
    {
        private readonly ViewRepository _repository;

        public ViewsController(ViewRepository repository)
        {
            _repository = repository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("{databaseName}/crear-vista")]
        public async Task<IActionResult> CrearVista(string databaseName, [FromBody] CreateViewRequest request)
        {
            try
            {
                await _repository.CrearVistaAsync(databaseName, request);
                return Ok($"Vista '{request.ViewName}' creada exitosamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear la vista: {ex.Message}");
            }
        }
    }
}
