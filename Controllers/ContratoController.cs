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

            repoContrato.Alta(contrato);

            var inmueble = repoInmueble.Buscar(contrato.InmuebleId);
            if (inmueble != null)
            {
                inmueble.Disponibilidad = Disponibilidad.OCUPADO;
                repoInmueble.Modificacion(inmueble);
            }

            TempData["Mensaje"] = "Contrato creado correctamente.";
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
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            var resultado = repoContrato.Baja(id);

            if (resultado == -1)
            {
                TempData["Error"] = "No se puede eliminar el contrato porque tiene pagos asociados.";
                return RedirectToAction(nameof(Index));
            }

            // 🔥 Verificar si el inmueble quedó sin contratos
            var tieneContratos = repoContrato.TieneContratosVigentes(contrato.InmuebleId);

            if (!tieneContratos)
            {
                var inmueble = repoInmueble.Buscar(contrato.InmuebleId);
                if (inmueble != null)
                {
                    inmueble.Disponibilidad = Disponibilidad.DESOCUPADO;
                    repoInmueble.Modificacion(inmueble);
                }
            }

            TempData["Mensaje"] = "Contrato eliminado correctamente.";
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

            // 🔥 VALIDACIÓN DE FECHA
            if (nuevaFechaFin < contrato.FechaInicio ||
                nuevaFechaFin > contrato.FechaFin)
            {
                ModelState.AddModelError("",
                    "La fecha de rescisión debe estar dentro del período del contrato.");

                return View(contrato);
            }

            var inicio = contrato.FechaInicio.ToDateTime(TimeOnly.MinValue);
            var finOriginal = contrato.FechaFin.ToDateTime(TimeOnly.MinValue);
            var finNuevo = nuevaFechaFin.ToDateTime(TimeOnly.MinValue);

            int mesesTotales = (int)((finOriginal - inicio).TotalDays / 30);
            int mesesCumplidos = (int)((finNuevo - inicio).TotalDays / 30);

            decimal multa = mesesCumplidos < mesesTotales / 2
                ? contrato.MontoMensual * 2
                : contrato.MontoMensual;

            repoContrato.TerminarContratoAnticipadamente(id, nuevaFechaFin);

            var inmueble = repoInmueble.Buscar(contrato.InmuebleId);
            if (inmueble != null)
            {
                inmueble.Disponibilidad = Disponibilidad.DESOCUPADO;
                repoInmueble.Modificacion(inmueble);
            }

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
                .Where(i => i.Disponibilidad != Disponibilidad.SUSPENDIDO)
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

        public IActionResult Renovar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            var nuevoContrato = new Contrato
            {
                InmuebleId = contrato.InmuebleId,
                InquilinoId = contrato.InquilinoId,
                FechaInicio = contrato.FechaFin.AddDays(1),
                FechaFin = contrato.FechaFin.AddYears(1),
                MontoMensual = contrato.MontoMensual
            };

            return View(nuevoContrato);
        }

        [HttpPost]
        public IActionResult Renovar(Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                return View(contrato);
            }

            // 🔥 VALIDAR SUPERPOSICIÓN
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

            var inmueble = repoInmueble.Buscar(contrato.InmuebleId);
            if (inmueble != null)
            {
                inmueble.Disponibilidad = Disponibilidad.OCUPADO;
                repoInmueble.Modificacion(inmueble);
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
