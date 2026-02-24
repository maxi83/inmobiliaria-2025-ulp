// ===============================
// IMPORTACIONES (USING)
// ===============================

// Importa las clases base del framework MVC:
// Controller, IActionResult, View(), RedirectToAction(), ModelState, etc.
using Microsoft.AspNetCore.Mvc;

// Permite usar atributos de seguridad como [Authorize]
// y control de acceso por roles.
using Microsoft.AspNetCore.Authorization;

// Permite usar SelectListItem para generar opciones
// en listas desplegables (<select>) dentro de las vistas.
using Microsoft.AspNetCore.Mvc.Rendering;

// Importa las clases del modelo del dominio:
// Contrato, Inmueble, Inquilino, Disponibilidad, etc.
using InmobiliariaUlP_2025.Models;

// Importa las interfaces de los repositorios.
// Estas interfaces permiten acceder a la base de datos
// sin acoplar el controlador a una implementación concreta.
using InmobiliariaUlP_2025.Repositories.Interfaces;


// ===============================
// DEFINICIÓN DEL CONTROLADOR
// ===============================

namespace InmobiliariaUlP_2025.Controllers
{
    // El atributo Authorize indica que todas las acciones
    // requieren que el usuario esté autenticado (logueado).
    [Authorize]
    public class ContratoController : Controller
    {
        // =====================================================
        // CAMPOS PRIVADOS (DEPENDENCIAS)
        // =====================================================

        // readonly indica que la variable solo puede asignarse
        // dentro del constructor y luego no puede modificarse.
        private readonly IRepositorioContrato repoContrato;

        // Repositorio encargado de acceder a la tabla Pagos.
        private readonly IRepositorioPago repoPago;

        // Repositorio para consultar y modificar inmuebles.
        private readonly IRepositorioInmueble repoInmueble;

        // Repositorio para consultar inquilinos.
        private readonly IRepositorioInquilino repoInquilino;


        // =====================================================
        // CONSTRUCTOR (INYECCIÓN DE DEPENDENCIAS)
        // =====================================================

        // El framework ASP.NET Core inyecta automáticamente
        // las implementaciones configuradas en Program.cs.
        public ContratoController(
            IRepositorioContrato repoContrato,
            IRepositorioInmueble repoInmueble,
            IRepositorioInquilino repoInquilino,
            IRepositorioPago repoPago)
        {
            // Se asignan las dependencias recibidas
            // a los campos privados del controlador.
            this.repoContrato = repoContrato;
            this.repoInmueble = repoInmueble;
            this.repoInquilino = repoInquilino;
            this.repoPago = repoPago;
        }


        // =====================================================
        // INDEX - LISTADO GENERAL DE CONTRATOS
        // =====================================================

        // Acción HTTP GET que muestra todos los contratos.
        public IActionResult Index()
        {
            // ViewBag es un objeto dinámico que permite
            // enviar información adicional a la vista.
            ViewBag.DesdeInmueble = false;

            // Se obtiene la lista completa desde el repositorio
            // y se envía como modelo a la vista.
            var contratos = repoContrato.ObtenerTodos();
            return View(contratos);
        }


        // =====================================================
        // LISTADO FILTRADO POR INMUEBLE
        // =====================================================

        // Muestra los contratos asociados a un inmueble específico.
        public IActionResult PorInmueble(int id)
        {
            // Indica que el listado proviene desde un inmueble.
            ViewBag.DesdeInmueble = true;

            // Busca el inmueble para mostrar sus datos en la vista.
            ViewBag.Inmueble = repoInmueble.Buscar(id);

            // Devuelve la vista Index reutilizándola
            // pero con contratos filtrados.
            var contratos = repoContrato.ObtenerPorInmueble(id);
            return View("Index", contratos);
        }


        // =====================================================
        // CREAR (GET)
        // =====================================================

        // Muestra el formulario para crear un contrato.
        public IActionResult Crear()
        {
            // Carga listas desplegables (combos).
            CargarCombos();

            // Envía un objeto vacío para que la vista lo complete.
            return View(new Contrato());
        }


        // =====================================================
        // CREAR (POST)
        // =====================================================

        // Indica que responde a una petición HTTP POST.
        [HttpPost]
        public IActionResult Crear(Contrato contrato)
        {
            // ModelState contiene el resultado de validaciones
            // definidas mediante DataAnnotations en el modelo.
            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(contrato);
            }

            // Verifica que no exista superposición de fechas
            // para el mismo inmueble.
            if (repoContrato.EstaOcupado(
                    contrato.InmuebleId,
                    contrato.FechaInicio,
                    contrato.FechaFin))
            {
                // Agrega un error general al modelo.
                ModelState.AddModelError("",
                    "El inmueble ya está ocupado en esas fechas.");

                CargarCombos();
                return View(contrato);
            }

            // Inserta el nuevo contrato en la base de datos.
            repoContrato.Alta(contrato);

            // Cambia el estado del inmueble a OCUPADO.
            var inmueble = repoInmueble.Buscar(contrato.InmuebleId);
            if (inmueble != null)
            {
                inmueble.Disponibilidad = Disponibilidad.OCUPADO;
                repoInmueble.Modificacion(inmueble);
            }

            // TempData permite mantener datos entre requests
            // luego de un Redirect.
            TempData["Mensaje"] = "Contrato creado correctamente.";

            // Patrón PRG (Post-Redirect-Get).
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Vigentes()
        {
            ViewBag.DesdeInmueble = false;
            return View("Index", repoContrato.ObtenerVigentes());
        }

        // =====================================================
        // TERMINAR CONTRATO ANTICIPADAMENTE
        // =====================================================

        public IActionResult Terminar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);

            if (contrato == null)
                return NotFound();

            return View(contrato);
        }

        [HttpPost]
        public IActionResult Terminar(int id, DateOnly nuevaFechaFin)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null)
                return NotFound();

            // Valida que la fecha esté dentro del rango original.
            if (nuevaFechaFin < contrato.FechaInicio ||
                nuevaFechaFin > contrato.FechaFin)
            {
                ModelState.AddModelError("",
                    "La fecha debe estar dentro del período del contrato.");

                return View(contrato);
            }

            // Conversión a DateTime para poder calcular diferencias.
            var inicio = contrato.FechaInicio.ToDateTime(TimeOnly.MinValue);
            var finOriginal = contrato.FechaFin.ToDateTime(TimeOnly.MinValue);
            var finNuevo = nuevaFechaFin.ToDateTime(TimeOnly.MinValue);

            // Cálculo aproximado de meses.
            int mesesTotales = (int)((finOriginal - inicio).TotalDays / 30);
            int mesesCumplidos = (int)((finNuevo - inicio).TotalDays / 30);

            // Regla de multa:
            // Si cumplió menos de la mitad → paga 2 meses.
            decimal multa = mesesCumplidos < mesesTotales / 2
                ? contrato.MontoMensual * 2
                : contrato.MontoMensual;

            // Obtiene pagos realizados.
            var pagos = repoPago.ObtenerPorContrato(id);

            int mesesAdeudados = mesesCumplidos - pagos.Count();
            if (mesesAdeudados < 0)
                mesesAdeudados = 0;

            // Actualiza fecha de finalización.
            repoContrato.TerminarContratoAnticipadamente(id, nuevaFechaFin);

            // Libera el inmueble.
            var inmueble = repoInmueble.Buscar(contrato.InmuebleId);
            if (inmueble != null)
            {
                inmueble.Disponibilidad = Disponibilidad.DESOCUPADO;
                repoInmueble.Modificacion(inmueble);
            }

            TempData["Multa"] = multa.ToString("N2");

            if (mesesAdeudados > 0)
            {
                TempData["Deuda"] =
                    $"El inquilino adeuda {mesesAdeudados} mes(es).";
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Renovar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);

            if (contrato == null)
                return NotFound();

            return View(contrato);
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


        // =====================================================
        // MÉTODO AUXILIAR PARA CARGAR COMBOS
        // =====================================================

        // Método privado: solo se usa dentro de esta clase.
        private void CargarCombos()
        {
            // Genera lista para el combo de inmuebles.
            ViewBag.Inmuebles = repoInmueble.ObtenerTodos()
                .Where(i => i.Disponibilidad != Disponibilidad.SUSPENDIDO)
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.Direccion
                })
                .ToList();

            // Genera lista para el combo de inquilinos.
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