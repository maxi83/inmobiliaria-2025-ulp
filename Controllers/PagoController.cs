using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
    [Authorize] // 🔒 requiere login
    public class PagoController : Controller
    {
        private readonly IRepositorioPago repositorioPago;
        private readonly IRepositorioContrato repositorioContrato;

        public PagoController(
            IRepositorioPago repositorioPago,
            IRepositorioContrato repositorioContrato)
        {
            this.repositorioPago = repositorioPago;
            this.repositorioContrato = repositorioContrato;
        }

        // =========================
        // LISTAR PAGOS DE CONTRATO
        // =========================
        public IActionResult Index(int contratoId)
        {
            ViewBag.Contrato = repositorioContrato.ObtenerPorId(contratoId);
            var pagos = repositorioPago.ObtenerPorContrato(contratoId);
            return View(pagos);
        }

        // =========================
        // CREAR PAGO
        // =========================
        public IActionResult Crear(int contratoId)
        {
            var pago = new Pago
            {
                ContratoId = contratoId
            };
            return View(pago);
        }

        [HttpPost]
        public IActionResult Crear(Pago pago)
        {
            if (!ModelState.IsValid)
                return View(pago);

            repositorioPago.Alta(pago);
            TempData["Mensaje"] = "Pago registrado correctamente.";
            return RedirectToAction(nameof(Index), new { contratoId = pago.ContratoId });
        }

        // =========================
        // ELIMINAR (SOLO ADMIN)
        // =========================
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var pago = repositorioPago.ObtenerPorId(id);
            if (pago == null) return NotFound();

            return View(pago);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            var pago = repositorioPago.ObtenerPorId(id);
            if (pago == null) return NotFound();

            repositorioPago.Baja(id);
            TempData["Mensaje"] = "Pago eliminado correctamente.";

            return RedirectToAction(nameof(Index), new { contratoId = pago.ContratoId });
        }
    }
}
