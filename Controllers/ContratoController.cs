using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
    [Authorize]
    public class ContratoController : Controller
    {
        private readonly IRepositorioContrato repoContrato;
        private readonly IRepositorioPago repoPago;
        private readonly IRepositorioInmueble repoInmueble;
        private readonly IRepositorioInquilino repoInquilino;

        public ContratoController(
            IRepositorioContrato repoContrato,
            IRepositorioInmueble repoInmueble,
            IRepositorioInquilino repoInquilino,
            IRepositorioPago repoPago)
        {
            this.repoContrato = repoContrato;
            this.repoInmueble = repoInmueble;
            this.repoInquilino = repoInquilino;
            this.repoPago = repoPago;
        }

        // =========================
        // LISTADOS
        // =========================

        public IActionResult Index()
        {
            ViewBag.DesdeInmueble = false;
            return View(repoContrato.ObtenerTodos());
        }

        public IActionResult Vigentes()
        {
            ViewBag.DesdeInmueble = false;
            return View("Index", repoContrato.ObtenerVigentes());
        }

        public IActionResult PorInmueble(int id)
        {
            ViewBag.DesdeInmueble = true;
            ViewBag.Inmueble = repoInmueble.Buscar(id);

            return View("Index", repoContrato.ObtenerPorInmueble(id));
        }

        // =========================
        // ALTA
        // =========================

        public IActionResult Crear()
        {
            CargarCombos();
            return View(new Contrato());
        }

        [HttpPost]
        public IActionResult Crear(Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(contrato);
            }

            if (repoContrato.EstaOcupado(
                    contrato.InmuebleId,
                    contrato.FechaInicio,
                    contrato.FechaFin))
            {
                ModelState.AddModelError("",
                    "El inmueble ya está ocupado en esas fechas.");
                CargarCombos();
                return View(contrato);
            }

            repoContrato.Alta(contrato);

            var inmueble = repoInmueble.Buscar(contrato.InmuebleId);
            if (inmueble != null)
            {
                inmueble.Disponibilidad = Disponibilidad.OCUPADO;
                repoInmueble.Modificacion(inmueble);
            }

            TempData["Mensaje"] = "Contrato creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDICIÓN
        // =========================

        public IActionResult Editar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null)
                return NotFound();

            CargarCombos();
            return View(contrato);
        }

        [HttpPost]
        public IActionResult Editar(Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(contrato);
            }

            repoContrato.Modificacion(contrato);

            TempData["Mensaje"] = "Contrato modificado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // BAJA
        // =========================

        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            return View(contrato);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            var pagos = repoPago.ObtenerPorContrato(id);
            if (pagos.Any())
            {
                TempData["Error"] = "No se puede eliminar el contrato porque tiene pagos registrados.";
                return RedirectToAction(nameof(Index));
            }

            repoContrato.Baja(id);

            var inmueble = repoInmueble.Buscar(contrato.InmuebleId);
            if (inmueble != null)
            {
                inmueble.Disponibilidad = Disponibilidad.DESOCUPADO;
                repoInmueble.Modificacion(inmueble);
            }

            TempData["Mensaje"] = "Contrato eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        // =========================
        // TERMINACIÓN ANTICIPADA
        // =========================

        public IActionResult Terminar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            return View(contrato);
        }

        [HttpPost]
        public IActionResult Terminar(int id, DateOnly nuevaFechaFin)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            if (nuevaFechaFin < contrato.FechaInicio ||
                nuevaFechaFin > contrato.FechaFin)
            {
                ModelState.AddModelError("",
                    "La fecha debe estar dentro del período del contrato.");
                return View(contrato);
            }

            var inicio = contrato.FechaInicio.ToDateTime(TimeOnly.MinValue);
            var finOriginal = contrato.FechaFin.ToDateTime(TimeOnly.MinValue);
            var finNuevo = nuevaFechaFin.ToDateTime(TimeOnly.MinValue);

            int mesesTotales = (int)((finOriginal - inicio).TotalDays / 30);
            int mesesCumplidos = (int)((finNuevo - inicio).TotalDays / 30);

            decimal multa = mesesCumplidos < mesesTotales / 2
                ? contrato.MontoMensual * 2
                : contrato.MontoMensual;

            var pagos = repoPago.ObtenerPorContrato(id);
            int mesesAdeudados = Math.Max(mesesCumplidos - pagos.Count(), 0);

            repoContrato.TerminarContratoAnticipadamente(id, nuevaFechaFin);

            var inmueble = repoInmueble.Buscar(contrato.InmuebleId);
            if (inmueble != null)
            {
                inmueble.Disponibilidad = Disponibilidad.DESOCUPADO;
                repoInmueble.Modificacion(inmueble);
            }

            TempData["Multa"] = multa.ToString("N2");

            if (mesesAdeudados > 0)
                TempData["Deuda"] =
                    $"El inquilino adeuda {mesesAdeudados} mes(es).";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // RENOVACIÓN
        // =========================

        public IActionResult Renovar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            var nuevoInicio = contrato.FechaFin.AddDays(1);
            var nuevoFin = nuevoInicio.AddYears(1);

            var renovacion = new Contrato
            {
                InmuebleId = contrato.InmuebleId,
                InquilinoId = contrato.InquilinoId,
                FechaInicio = nuevoInicio,
                FechaFin = nuevoFin,
                MontoMensual = contrato.MontoMensual
            };

            return View(renovacion);
        }
        [HttpPost]
        public IActionResult Renovar(Contrato contrato)
        {
            if (!ModelState.IsValid)
                return View(contrato);

            if (repoContrato.EstaOcupado(
                    contrato.InmuebleId,
                    contrato.FechaInicio,
                    contrato.FechaFin))
            {
                ModelState.AddModelError("",
                    "El inmueble ya está ocupado en esas fechas.");
                return View(contrato);
            }

            repoContrato.Alta(contrato);

            TempData["Mensaje"] = "Contrato renovado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // AUXILIARES
        // =========================

        private void CargarCombos()
        {
            ViewBag.Inmuebles = repoInmueble.ObtenerTodos()
                .Where(i => i.Disponibilidad != Disponibilidad.SUSPENDIDO)
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.Direccion
                })
                .ToList();

            ViewBag.Inquilinos = repoInquilino.ObtenerTodos()
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = $"{i.Apellido}, {i.Nombre}"
                })
                .ToList();
        }
    }
}