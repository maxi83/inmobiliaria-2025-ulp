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
    }
}
