using Microsoft.AspNetCore.Mvc;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
    public class PagoController : Controller
    {
        private readonly IRepositorioPago repoPago;
        private readonly IRepositorioContrato repoContrato;

        public PagoController(IRepositorioPago repoPago, IRepositorioContrato repoContrato)
        {
            this.repoPago = repoPago;
            this.repoContrato = repoContrato;
        }

        // LISTA DE PAGOS POR CONTRATO
        public IActionResult Index(int contratoId)
        {
            var contrato = repoContrato.ObtenerPorId(contratoId);
            if (contrato == null)
                return NotFound();

            ViewBag.Contrato = contrato;
            var pagos = repoPago.ObtenerPorContrato(contratoId);

            return View(pagos);
        }

        // GET - CREAR
        public IActionResult Crear(int contratoId)
        {
            ViewBag.ContratoId = contratoId;
            return View();
        }

        // POST - CREAR
        [HttpPost]
        public IActionResult Crear(Pago pago)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ContratoId = pago.ContratoId;
                return View(pago);
            }

            // Calcular número de pago
            pago.NoPago = repoPago.ObtenerSiguienteNumeroPago(pago.ContratoId);

            repoPago.Alta(pago);

            return RedirectToAction("Index", new { contratoId = pago.ContratoId });
        }

        // GET - ELIMINAR
        public IActionResult Eliminar(int id)
        {
            var pago = repoPago.ObtenerPorId(id);
            if (pago == null)
                return NotFound();

            return View(pago);
        }

        // POST - ELIMINAR
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            var pago = repoPago.ObtenerPorId(id);
            if (pago == null)
                return NotFound();

            repoPago.Baja(id);

            return RedirectToAction("Index", new { contratoId = pago.ContratoId });
        }
    }
}
