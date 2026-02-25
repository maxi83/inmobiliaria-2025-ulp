using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly IRepositorioPago repoPago;
        private readonly IRepositorioContrato repoContrato;

        public PagoController(IRepositorioPago repoPago, IRepositorioContrato repoContrato)
        {
            this.repoPago = repoPago;
            this.repoContrato = repoContrato;
        }

        // =========================
        // LISTADO
        // =========================

        public IActionResult PorContrato(int id, int? inmuebleId, bool desdeInmueble = false)
        {
            ViewBag.Contrato = repoContrato.ObtenerPorId(id);
            ViewBag.InmuebleId = inmuebleId;
            ViewBag.DesdeInmueble = desdeInmueble;

            return View(repoPago.ObtenerPorContrato(id));
        }

        // =========================
        // ALTA
        // =========================

        public IActionResult Crear(int contratoId, int? inmuebleId, bool desdeInmueble = false)
        {
            var contrato = repoContrato.ObtenerPorId(contratoId);
            if (contrato == null) return NotFound();

            var pago = new Pago
            {
                ContratoId = contratoId,
                Importe = contrato.MontoMensual
            };

            ViewBag.Contrato = contrato;
            ViewBag.InmuebleId = inmuebleId;
            ViewBag.DesdeInmueble = desdeInmueble;

            return View(pago);
        }

        [HttpPost]
        public IActionResult Crear(Pago pago)
        {
            var contrato = repoContrato.ObtenerPorId(pago.ContratoId);
            if (contrato == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Contrato = contrato;
                return View(pago);
            }

            // Regla de negocio: el pago debe estar dentro del período del contrato
            if (pago.FechaPago < contrato.FechaInicio ||
                pago.FechaPago > contrato.FechaFin)
            {
                ModelState.AddModelError("",
                    "La fecha del pago debe estar dentro del período del contrato.");
                ViewBag.Contrato = contrato;
                return View(pago);
            }

            repoPago.Alta(pago);

            return RedirectToAction(nameof(PorContrato),
                new { id = pago.ContratoId });
        }

        // =========================
        // MODIFICACIÓN
        // =========================

        public IActionResult Editar(int id)
        {
            var pago = repoPago.ObtenerPorId(id);
            if (pago == null) return NotFound();

            return View(pago);
        }

        [HttpPost]
        public IActionResult Editar(Pago pago)
        {
            var contrato = repoContrato.ObtenerPorId(pago.ContratoId);
            if (contrato == null) return NotFound();

            if (!ModelState.IsValid)
                return View(pago);

            //  Validar que la fecha esté dentro del período del contrato
            if (pago.FechaPago < contrato.FechaInicio ||
                pago.FechaPago > contrato.FechaFin)
            {
                ModelState.AddModelError("",
                    "La fecha del pago debe estar dentro del período del contrato.");
                return View(pago);
            }

            repoPago.Modificacion(pago);

            return RedirectToAction(nameof(PorContrato),
                new { id = pago.ContratoId });
        }

        // =========================
        // BAJA (Solo Administrador)
        // =========================

        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var pago = repoPago.ObtenerPorId(id);
            if (pago == null) return NotFound();

            return View(pago);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            var pago = repoPago.ObtenerPorId(id);
            if (pago == null) return NotFound();

            repoPago.Baja(id);

            return RedirectToAction(nameof(PorContrato),
                new { id = pago.ContratoId });
        }
    }
}