using Microsoft.AspNetCore.Mvc;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
    public class InmueblesController : Controller
    {
        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly IRepositorioPropietario repositorioPropietario;

        public InmueblesController(
            IRepositorioInmueble repositorioInmueble,
            IRepositorioPropietario repositorioPropietario)
        {
            this.repositorioInmueble = repositorioInmueble;
            this.repositorioPropietario = repositorioPropietario;
        }

        public IActionResult Index()
        {
            var lista = repositorioInmueble.ObtenerTodos();
            return View(lista);
        }

        public IActionResult Crear()
        {
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Inmueble inmueble)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                return View(inmueble);
            }

            repositorioInmueble.Alta(inmueble);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Editar(int id)
        {
            var inmueble = repositorioInmueble.Buscar(id);

            if (inmueble == null)
                return NotFound();

            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            return View(inmueble);
        }

        [HttpPost]
        public IActionResult Editar(Inmueble inmueble)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                return View(inmueble);
            }

            repositorioInmueble.Modificacion(inmueble);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Eliminar(int id)
        {
            var inmueble = repositorioInmueble.Buscar(id);

            if (inmueble == null)
                return NotFound();

            return View(inmueble);
        }

        [HttpPost]
        public IActionResult ConfirmarEliminar(int id)
        {
            repositorioInmueble.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
