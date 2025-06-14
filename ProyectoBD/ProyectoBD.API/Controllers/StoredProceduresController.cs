using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Repositories.Repositories;

namespace ProyectoBD.API.Controllers
{
    public class StoredProceduresController : Controller
    {
        private readonly FunctionRepository _repository;

        public StoredProceduresController(FunctionRepository repository)
        {
            _repository = repository;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
