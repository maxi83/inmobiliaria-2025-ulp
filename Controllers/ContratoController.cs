// Permite usar las clases base de ASP.NET Core MVC (Controller, IActionResult, View, Redirect, etc.)
using Microsoft.AspNetCore.Mvc;

// Permite usar atributos de autorización como [Authorize] y control por roles
using Microsoft.AspNetCore.Authorization;

// Permite usar SelectListItem para construir combos (<select>) en las vistas
using Microsoft.AspNetCore.Mvc.Rendering;

// Permite usar las clases del modelo (Contrato, Inmueble, Inquilino, etc.)
using InmobiliariaUlP_2025.Models;

// Permite usar las interfaces de los repositorios (acceso a datos)
using InmobiliariaUlP_2025.Repositories.Interfaces;

// Espacio de nombres lógico que organiza esta clase dentro del proyecto
namespace InmobiliariaUlP_2025.Controllers
{
    // Indica que todas las acciones de este controlador requieren usuario logueado
    [Authorize]
    public class ContratoController : Controller
    {
        // Campo privado y readonly que guarda el repositorio de contratos
        // readonly significa que solo se puede asignar en el constructor
        private readonly IRepositorioContrato repoContrato;

        // Repositorio para acceder a datos de inmuebles
        private readonly IRepositorioInmueble repoInmueble;

        // Repositorio para acceder a datos de inquilinos
        private readonly IRepositorioInquilino repoInquilino;

        // Constructor: ASP.NET inyecta automáticamente las dependencias
        public ContratoController(
            IRepositorioContrato repoContrato,
            IRepositorioInmueble repoInmueble,
            IRepositorioInquilino repoInquilino)
        {
            // Se asignan las dependencias recibidas a los campos privados
            this.repoContrato = repoContrato;
            this.repoInmueble = repoInmueble;
            this.repoInquilino = repoInquilino;
        }

        // =========================
        // LISTADO GENERAL
        // =========================

        // Acción GET que muestra todos los contratos
        public IActionResult Index()
        {
            // ViewBag.DesdeInmueble es una bandera (true/false)
            // Sirve para que la vista sepa desde dónde fue llamada
            ViewBag.DesdeInmueble = false;

            // Devuelve la vista Index.cshtml con todos los contratos como modelo
            return View(repoContrato.ObtenerTodos());
        }

        // =========================
        // LISTADO DESDE INMUEBLE
        // =========================

        // Acción GET que muestra los contratos de un inmueble específico
        public IActionResult PorInmueble(int id)
        {
            // Indica que la vista viene desde un inmueble
            ViewBag.DesdeInmueble = true;

            // Carga el inmueble para mostrar sus datos en la vista
            ViewBag.Inmueble = repoInmueble.Buscar(id);

            // Devuelve la misma vista Index pero con contratos filtrados
            return View("Index", repoContrato.ObtenerPorInmueble(id));
        }

        // =========================
        // CREAR
        // =========================

        // Acción GET que muestra el formulario de creación
        public IActionResult Crear()
        {
            // Carga los combos (inmuebles e inquilinos)
            CargarCombos();

            // Devuelve la vista Crear con un objeto nuevo vacío
            return View(new Contrato());
        }

        // Acción POST que recibe el contrato enviado desde el formulario
        [HttpPost]
        public IActionResult Crear(Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(contrato);
            }

            // 🔥 VALIDACIÓN OBLIGATORIA SEGÚN ENUNCIADO
            if (repoContrato.EstaOcupado(
                    contrato.InmuebleId,
                    contrato.FechaInicio,
                    contrato.FechaFin))
            {
                ModelState.AddModelError("",
                    "El inmueble ya está ocupado en esas fechas.");

                CargarCombos();
                return View(contrato);
            }

            // Guarda el contrato
            repoContrato.Alta(contrato);

            // Marca inmueble como ocupado
            var inmueble = repoInmueble.Buscar(contrato.InmuebleId);
            if (inmueble != null)
            {
                inmueble.Disponibilidad = Disponibilidad.OCUPADO;
                repoInmueble.Modificacion(inmueble);
            }

            return RedirectToAction(nameof(Index));
        }


        // =========================
        // EDITAR
        // =========================

        // Acción GET para mostrar formulario de edición
        public IActionResult Editar(int id)
        {
            // Busca el contrato por id
            var contrato = repoContrato.ObtenerPorId(id);

            // Si no existe, devuelve error 404
            if (contrato == null) return NotFound();

            // Carga combos nuevamente
            CargarCombos();

            // Devuelve la vista con el contrato como modelo
            return View(contrato);
        }

        // Acción POST que procesa la modificación
        [HttpPost]
        public IActionResult Editar(Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(contrato);
            }

            // Llama al repositorio para actualizar en la base
            repoContrato.Modificacion(contrato);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // ELIMINAR (ADMIN)
        // =========================

        // Solo el rol Administrador puede eliminar
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);

            if (contrato == null) return NotFound();

            return View(contrato);
        }

        // Confirmación de eliminación
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult EliminarConfirmado(int id)
        {
            // Llama al repositorio para borrar el contrato
            repoContrato.Baja(id);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // TERMINAR / RESCINDIR
        // =========================

        // Muestra la pantalla para terminar contrato anticipadamente
        public IActionResult Terminar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);

            if (contrato == null) return NotFound();

            return View(contrato);
        }

        // Acción POST que calcula multa y finaliza el contrato
        [HttpPost]
        public IActionResult Terminar(int id, DateOnly nuevaFechaFin)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            // Convierte DateOnly a DateTime agregando hora mínima
            var inicio = contrato.FechaInicio.ToDateTime(TimeOnly.MinValue);
            var finOriginal = contrato.FechaFin.ToDateTime(TimeOnly.MinValue);
            var finNuevo = nuevaFechaFin.ToDateTime(TimeOnly.MinValue);

            // Calcula meses totales del contrato original
            int mesesTotales = (int)((finOriginal - inicio).TotalDays / 30);

            // Calcula meses cumplidos hasta la nueva fecha
            int mesesCumplidos = (int)((finNuevo - inicio).TotalDays / 30);

            // Operador ternario:
            // Si cumplió menos de la mitad → multa = 2 meses
            // Si no → multa = 1 mes
            decimal multa = mesesCumplidos < mesesTotales / 2
                ? contrato.MontoMensual * 2
                : contrato.MontoMensual;

            // Actualiza fecha fin en base
            repoContrato.TerminarContratoAnticipadamente(id, nuevaFechaFin);
            var inmueble = repoInmueble.Buscar(contrato.InmuebleId);
            if (inmueble != null)
            {
                inmueble.Disponibilidad = Disponibilidad.DESOCUPADO; // tu enum exacto
                repoInmueble.Modificacion(inmueble);
            }
                    // Guarda la multa en TempData como string
            // Se convierte a string para evitar problemas de serialización
            TempData["Multa"] = multa.ToString("N2");

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // COMBOS
        // =========================

        // Método privado auxiliar que carga combos para formularios
       private void CargarCombos()
        {
            ViewBag.Inmuebles = repoInmueble.ObtenerTodos()
                .Where(i => i.Disponibilidad == Disponibilidad.DESOCUPADO)
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
        public IActionResult Vigentes()
        {
            return View("Index", repoContrato.ObtenerVigentes());
        }

    }
}
