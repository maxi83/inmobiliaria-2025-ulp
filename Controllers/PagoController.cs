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

        public IActionResult PorContrato(int id, int? inmuebleId, bool desdeInmueble = false)
        {
            ViewBag.Contrato = repoContrato.ObtenerPorId(id);
            ViewBag.InmuebleId = inmuebleId;
            ViewBag.DesdeInmueble = desdeInmueble;

            var pagos = repoPago.ObtenerPorContrato(id);
            return View(pagos);
        }

        public IActionResult Crear(int contratoId)
        {
            var contrato = repoContrato.ObtenerPorId(contratoId);
            if (contrato == null) return NotFound();

            var pago = new Pago
            {
                ContratoId = contratoId
            };

            ViewBag.Contrato = contrato;

            return View(pago);
        }

        [HttpPost]
        public IActionResult Crear(Pago pago)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Contrato = repoContrato.ObtenerPorId(pago.ContratoId);
                return View(pago);
            }

            repoPago.Alta(pago);

            return RedirectToAction(nameof(PorContrato), 
                new { id = pago.ContratoId });
        }

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
