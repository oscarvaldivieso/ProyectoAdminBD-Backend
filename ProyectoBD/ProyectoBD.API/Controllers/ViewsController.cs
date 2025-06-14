using Microsoft.AspNetCore.Mvc;
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
    }
}
