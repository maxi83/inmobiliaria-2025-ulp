using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
    [Authorize]
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

        // ======================================================
        // INDEX
        // ======================================================

        public IActionResult Index(
            Disponibilidad? dispo,
            int? propietarioId,
            DateTime? inicio,
            DateTime? fin)
        {
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();

            var inmuebles = repositorioInmueble.ObtenerTodos();

            if (inicio.HasValue && fin.HasValue)
            {
                var inicioDateOnly = DateOnly.FromDateTime(inicio.Value);
                var finDateOnly = DateOnly.FromDateTime(fin.Value);

                inmuebles = repositorioInmueble
                    .ObtenerDisponiblesEntreFechas(inicioDateOnly, finDateOnly)
                    .ToList();
            }

            if (dispo.HasValue)
            {
                inmuebles = inmuebles
                    .Where(i => i.Disponibilidad == dispo.Value)
                    .ToList();
            }

            if (propietarioId.HasValue)
            {
                inmuebles = inmuebles
                    .Where(i => i.PropietarioId == propietarioId.Value)
                    .ToList();
            }

            return View(inmuebles);
        }

        // ======================================================
        // DETALLES
        // ======================================================

        public IActionResult Detalles(int id)
        {
            var inmueble = repositorioInmueble.Buscar(id);

            if (inmueble == null)
                return NotFound();

            return View(inmueble);
        }

        // ======================================================
        // CREAR (GET)
        // ======================================================

        public IActionResult Crear()
        {
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            return View();
        }

        // ======================================================
        // CREAR (POST)
        // ======================================================

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

        // ======================================================
        // EDITAR (GET)
        // ======================================================

        public IActionResult Editar(int id)
        {
            var inmueble = repositorioInmueble.Buscar(id);

            if (inmueble == null)
                return NotFound();

            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            return View(inmueble);
        }

        // ======================================================
        // EDITAR (POST)
        // ======================================================

        [HttpPost]
        public IActionResult Editar(Inmueble inmueble)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                return View(inmueble);
            }

            repositorioInmueble.Modificacion(inmueble);

            TempData["Mensaje"] = "Inmueble editado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ======================================================
        // ELIMINAR (Solo Admin)
        // ======================================================

        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var inmueble = repositorioInmueble.Buscar(id);

            if (inmueble == null)
                return NotFound();

            return View(inmueble);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult ConfirmarEliminar(int id)
        {
            var resultado = repositorioInmueble.Baja(id);

            if (resultado == -1)
            {
                TempData["Error"] =
                    "No se puede eliminar el inmueble porque tiene contratos asociados.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Mensaje"] = "Inmueble eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}