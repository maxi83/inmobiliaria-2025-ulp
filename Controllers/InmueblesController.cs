using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
    [Authorize] // 🔒 requiere login
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

        public IActionResult Index(Disponibilidad? dispo)
        {
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();

            if (dispo.HasValue)
            {
                ViewBag.FiltroActual = dispo.Value;
                return View(repositorioInmueble.ObtenerPorDisponibilidad(dispo.Value));
            }

            ViewBag.FiltroActual = null;
            return View(repositorioInmueble.ObtenerTodos());
        }

        public IActionResult PorPropietario(int id)
        {
            ViewBag.Propietario = repositorioPropietario.ObtenerPorId(id);
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            return View("Index", repositorioInmueble.ObtenerPorPropietario(id));
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

            var resultado = repositorioInmueble.Alta(inmueble);

            if (resultado == -1)
            {
                ViewBag.Error = "La dirección ya existe.";
                ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                return View(inmueble);
            }

            TempData["Mensaje"] = "Inmueble creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Editar(int id)
        {
            var inmueble = repositorioInmueble.Buscar(id);
            if (inmueble == null) return NotFound();

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
            TempData["Mensaje"] = "Inmueble modificado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // 🔒 SOLO ADMIN PUEDE ELIMINAR
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var inmueble = repositorioInmueble.Buscar(id);
            if (inmueble == null) return NotFound();

            return View(inmueble);
        }

        // 🔒 SOLO ADMIN PUEDE CONFIRMAR
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult ConfirmarEliminar(int id)
        {
            repositorioInmueble.Baja(id);
            TempData["Mensaje"] = "Inmueble eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
