using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
    public class ContratoController : Controller
    {
        private readonly IRepositorioContrato repoContrato;
        private readonly IRepositorioInmueble repoInmueble;
        private readonly IRepositorioInquilino repoInquilino;
        private readonly IRepositorioPago repoPago;

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
        // LISTADO
        // =========================
        public IActionResult Index()
        {
            var contratos = repoContrato.ObtenerTodos();
            return View(contratos);
        }

        public IActionResult Detalle(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        // =========================
        // CREAR
        // =========================
        public IActionResult Crear()
        {
            CargarCombos();
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(contrato);
            }

            bool ocupado = repoContrato.EstaOcupado(
                contrato.InmuebleId,
                contrato.FechaInicio,
                contrato.FechaFin
            );

            if (ocupado)
            {
                ModelState.AddModelError("", "❌ El inmueble ya está ocupado en ese rango de fechas.");
                CargarCombos();
                return View(contrato);
            }

            repoContrato.Alta(contrato);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDITAR
        // =========================
        public IActionResult Editar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

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

            bool ocupado = repoContrato.EstaOcupado(
                contrato.InmuebleId,
                contrato.FechaInicio,
                contrato.FechaFin,
                contrato.Id
            );

            if (ocupado)
            {
                ModelState.AddModelError("", "❌ El inmueble ya está ocupado en esas fechas.");
                CargarCombos();
                return View(contrato);
            }

            repoContrato.Modificacion(contrato);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // ELIMINAR
        // =========================
        public IActionResult Eliminar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            var resultado = repoContrato.Baja(id);

            if (resultado == -1)
            {
                TempData["Error"] = "❌ No se puede eliminar el contrato porque tiene pagos asociados.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Mensaje"] = "✔ Contrato eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // TERMINAR ANTICIPADO
        // =========================
        public IActionResult Terminar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        [HttpPost, ActionName("Terminar")]
        public IActionResult TerminarConfirmado(int id, DateOnly nuevaFechaFin)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            DateTime inicio = contrato.FechaInicio.ToDateTime(TimeOnly.MinValue);

            // ✅ FIX: si FechaFinOriginal es null, usamos FechaFin
            var fechaFinOriginal = contrato.FechaFinOriginal ?? contrato.FechaFin;
            DateTime finOriginal = fechaFinOriginal.ToDateTime(TimeOnly.MinValue);

            DateTime finNueva = nuevaFechaFin.ToDateTime(TimeOnly.MinValue);

            int mesesTotales = (int)((finOriginal - inicio).TotalDays / 30);
            int mesesCumplidos = (int)((finNueva - inicio).TotalDays / 30);

            decimal multa = mesesCumplidos < mesesTotales / 2
                ? contrato.MontoMensual * 2
                : contrato.MontoMensual;

            repoContrato.TerminarContratoAnticipadamente(id, nuevaFechaFin);

            TempData["Mensaje"] = $"Contrato rescindido correctamente. Multa a pagar: ${multa:N2}";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // RENOVAR
        // =========================
        public IActionResult Renovar(int id)
        {
            var anterior = repoContrato.ObtenerPorId(id);
            if (anterior == null) return NotFound();

            var nuevo = new Contrato
            {
                InmuebleId = anterior.InmuebleId,
                InquilinoId = anterior.InquilinoId,
                FechaInicio = DateOnly.FromDateTime(DateTime.Now),
                FechaFin = anterior.FechaFin.AddMonths(12),
                MontoMensual = anterior.MontoMensual
            };

            CargarCombos();
            return View(nuevo);
        }

        [HttpPost]
        public IActionResult RenovarConfirmado(Contrato nuevo, int contratoAnteriorId)
        {
            repoContrato.RenovarContrato(nuevo, contratoAnteriorId);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // COMBOS
        // =========================
        private void CargarCombos()
        {
            ViewBag.Inmuebles = repoInmueble.ObtenerTodos()
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

        // =========================
        // CONTRATOS VIGENTES
        // =========================
        public IActionResult Vigentes()
        {
            var contratos = repoContrato.ObtenerVigentes();
            return View("Index", contratos);
        }

        // =========================
        // CONTRATOS POR INMUEBLE
        // =========================
        public IActionResult PorInmueble(int id)
        {
            ViewBag.EsPorInmueble = true;
            var contratos = repoContrato.ObtenerPorInmueble(id);
            return View("Index", contratos);
        }

    }
}
