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
        private readonly IRepositorioInmueble repoInmueble;
        private readonly IRepositorioInquilino repoInquilino;

        public ContratoController(
            IRepositorioContrato repoContrato,
            IRepositorioInmueble repoInmueble,
            IRepositorioInquilino repoInquilino)
        {
            this.repoContrato = repoContrato;
            this.repoInmueble = repoInmueble;
            this.repoInquilino = repoInquilino;
        }

        // =========================
        // LISTADO GENERAL
        // =========================
        public IActionResult Index()
        {
            ViewBag.DesdeInmueble = false;
            return View(repoContrato.ObtenerTodos());
        }

        // =========================
        // LISTADO DESDE INMUEBLE
        // =========================
        public IActionResult PorInmueble(int id)
        {
            ViewBag.DesdeInmueble = true;
            ViewBag.Inmueble = repoInmueble.Buscar(id);
            return View("Index", repoContrato.ObtenerPorInmueble(id));
        }

        // =========================
        // CREAR
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

            repoContrato.Modificacion(contrato);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // ELIMINAR (ADMIN)
        // =========================
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            return View(contrato);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult EliminarConfirmado(int id)
        {
            repoContrato.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // TERMINAR / RESCINDIR
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

            var inicio = contrato.FechaInicio.ToDateTime(TimeOnly.MinValue);
            var finOriginal = contrato.FechaFin.ToDateTime(TimeOnly.MinValue);
            var finNuevo = nuevaFechaFin.ToDateTime(TimeOnly.MinValue);

            int mesesTotales = (int)((finOriginal - inicio).TotalDays / 30);
            int mesesCumplidos = (int)((finNuevo - inicio).TotalDays / 30);

            decimal multa = mesesCumplidos < mesesTotales / 2
                ? contrato.MontoMensual * 2
                : contrato.MontoMensual;

            repoContrato.TerminarContratoAnticipadamente(id, nuevaFechaFin);

            // 🔑 TempData COMO STRING (evita error)
            TempData["Multa"] = multa.ToString("N2");

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
                });

            ViewBag.Inquilinos = repoInquilino.ObtenerTodos()
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = $"{i.Apellido}, {i.Nombre}"
                });
        }
    }
}
