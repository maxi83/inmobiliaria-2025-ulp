using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
    [Authorize]
    public class InmueblesController : Controller
    {
        private readonly IRepositorioInmueble repoInmueble;
        private readonly IRepositorioPropietario repoPropietario;

        public InmueblesController(
            IRepositorioInmueble repoInmueble,
            IRepositorioPropietario repoPropietario)
        {
            this.repoInmueble = repoInmueble;
            this.repoPropietario = repoPropietario;
        }

        // =========================
        // LISTADO
        // =========================

        public IActionResult Index(
            Disponibilidad? dispo,
            int? propietarioId,
            DateTime? inicio,
            DateTime? fin)
        {
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();

            var inmuebles = repoInmueble.ObtenerTodos();

            // Filtro por disponibilidad entre fechas
            if (inicio.HasValue && fin.HasValue)
            {
                if (fin < inicio)
                {
                    TempData["Error"] = "La fecha Hasta no puede ser menor que Desde.";
                    return RedirectToAction(nameof(Index));
                }

                var inicioDateOnly = DateOnly.FromDateTime(inicio.Value);
                var finDateOnly = DateOnly.FromDateTime(fin.Value);

                inmuebles = repoInmueble
                    .ObtenerDisponiblesEntreFechas(inicioDateOnly, finDateOnly)
                    .ToList();
            }

            if (dispo.HasValue)
                inmuebles = inmuebles
                    .Where(i => i.Disponibilidad == dispo.Value)
                    .ToList();

            if (propietarioId.HasValue)
                inmuebles = inmuebles
                    .Where(i => i.PropietarioId == propietarioId.Value)
                    .ToList();

            return View(inmuebles);
        }

        public IActionResult Detalles(int id)
        {
            var inmueble = repoInmueble.Buscar(id);
            if (inmueble == null) return NotFound();

            return View(inmueble);
        }

        // =========================
        // ALTA
        // =========================

        public IActionResult Crear()
        {
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Inmueble inmueble)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                return View(inmueble);
            }

            var resultado = repoInmueble.Alta(inmueble);

            // Regla de negocio: evitar direcciones duplicadas
            if (resultado == -1)
            {
                ViewBag.Error = "La dirección ya existe.";
                ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                return View(inmueble);
            }

            TempData["Mensaje"] = "Inmueble creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // MODIFICACIÓN
        // =========================

        public IActionResult Editar(int id)
        {
            var inmueble = repoInmueble.Buscar(id);
            if (inmueble == null) return NotFound();

            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            return View(inmueble);
        }

        [HttpPost]
        public IActionResult Editar(Inmueble inmueble)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                return View(inmueble);
            }

            repoInmueble.Modificacion(inmueble);

            TempData["Mensaje"] = "Inmueble editado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // BAJA (Solo Administrador)
        // =========================

        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var inmueble = repoInmueble.Buscar(id);
            if (inmueble == null) return NotFound();

            return View(inmueble);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult ConfirmarEliminar(int id)
        {
            var resultado = repoInmueble.Baja(id);

            // Regla de negocio: no permitir eliminar si tiene contratos
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