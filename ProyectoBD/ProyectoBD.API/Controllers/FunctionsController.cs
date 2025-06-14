using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Repositories.Repositories;

namespace ProyectoBD.API.Controllers
{
    public class FunctionsController : Controller
    {
        private readonly FunctionRepository _repository;

        public FunctionsController(FunctionRepository repository)
        {
            _repository = repository;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
