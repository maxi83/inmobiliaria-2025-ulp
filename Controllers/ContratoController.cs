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

            var contratos = repoContrato.ObtenerPorInmueble(id);
            return View("Index", contratos);
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
        // ELIMINAR
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
