using Microsoft.AspNetCore.Mvc;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
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

        // LISTADO GENERAL
        public IActionResult Index()
        {
            var contratos = repoContrato.ObtenerTodos();
            return View(contratos);
        }

        // DETALLE
        public IActionResult Detalle(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        // GET - CREAR
        public IActionResult Crear()
        {
            ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
            ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
            return View();
        }

        // POST - CREAR
        [HttpPost]
        public IActionResult Crear(Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                return View(contrato);
            }

            var ocupado = repoContrato.EstaOcupado(
                contrato.InmuebleId,
                contrato.FechaInicio,
                contrato.FechaFin
            );

            if (ocupado)
            {
                ModelState.AddModelError("", "El inmueble está ocupado en esas fechas.");
                ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                return View(contrato);
            }

            repoContrato.Alta(contrato);
            return RedirectToAction("Index");
        }

        // GET - EDITAR
        public IActionResult Editar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
            ViewBag.Inquilinos = repoInquilino.ObtenerTodos();

            return View(contrato);
        }

        // POST - EDITAR
        [HttpPost]
        public IActionResult Editar(Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                return View(contrato);
            }

            var ocupado = repoContrato.EstaOcupado(
                contrato.InmuebleId,
                contrato.FechaInicio,
                contrato.FechaFin,
                contrato.Id
            );

            if (ocupado)
            {
                ModelState.AddModelError("", "El inmueble está ocupado en esas fechas.");
                ViewBag.Inmuebles = repoInmueble.ObtenerTodos();
                ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
                return View(contrato);
            }

            repoContrato.Modificacion(contrato);
            return RedirectToAction("Index");
        }

        // GET - ELIMINAR
        public IActionResult Eliminar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        // POST - ELIMINAR
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            repoContrato.Baja(id);
            return RedirectToAction("Index");
        }
    }
}
