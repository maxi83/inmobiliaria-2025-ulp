// Importa clases base de MVC (Controller, IActionResult, View, Redirect, etc.)
using Microsoft.AspNetCore.Mvc;

// Permite usar atributos como [Authorize] y control por roles
using Microsoft.AspNetCore.Authorization;

// Permite usar las clases del modelo (Inmueble, Disponibilidad, etc.)
using InmobiliariaUlP_2025.Models;

// Permite usar las interfaces de los repositorios
using InmobiliariaUlP_2025.Repositories.Interfaces;

// Define el espacio de nombres del controlador
namespace InmobiliariaUlP_2025.Controllers
{
    // Indica que todas las acciones requieren usuario logueado
    [Authorize]
    public class InmueblesController : Controller
    {
        // Repositorio para manejar los inmuebles (acceso a BD)
        private readonly IRepositorioInmueble repositorioInmueble;

        // Repositorio para manejar propietarios (se usa para combos y relaciones)
        private readonly IRepositorioPropietario repositorioPropietario;

        // Constructor con inyección de dependencias
        public InmueblesController(
            IRepositorioInmueble repositorioInmueble,
            IRepositorioPropietario repositorioPropietario)
        {
            // Se asignan los repositorios recibidos
            this.repositorioInmueble = repositorioInmueble;
            this.repositorioPropietario = repositorioPropietario;
        }

        // Acción principal que lista inmuebles
        // Recibe filtros opcionales:
        // dispo = estado del inmueble (nullable)
        // propietarioId = filtro por propietario (nullable)
        // inicio y fin = rango de fechas (nullable)
        public IActionResult Index(
            Disponibilidad? dispo,
            int? propietarioId,
            DateTime? inicio,
            DateTime? fin)
        {
            // Carga todos los propietarios para el combo de filtro
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();

            // Obtiene todos los inmuebles inicialmente
            var inmuebles = repositorioInmueble.ObtenerTodos();

            // Si se ingresaron fechas, filtra por disponibilidad entre fechas
            if (inicio.HasValue && fin.HasValue)
            {
                // Convierte DateTime a DateOnly (solo fecha sin hora)
                var inicioDateOnly = DateOnly.FromDateTime(inicio.Value);
                var finDateOnly = DateOnly.FromDateTime(fin.Value);

                // Llama al método que trae solo los disponibles en ese rango
                inmuebles = repositorioInmueble
                    .ObtenerDisponiblesEntreFechas(inicioDateOnly, finDateOnly)
                    .ToList();
            }

            // Si se seleccionó un estado (Disponible, Ocupado, Suspendido)
            if (dispo.HasValue)
            {
                // Filtra en memoria por estado
                inmuebles = inmuebles
                    .Where(i => i.Disponibilidad == dispo.Value)
                    .ToList();
            }

            // Si se seleccionó un propietario específico
            if (propietarioId.HasValue)
            {
                // Filtra en memoria por propietario
                inmuebles = inmuebles
                    .Where(i => i.PropietarioId == propietarioId.Value)
                    .ToList();
            }

            // Devuelve la vista con la lista final filtrada
            return View(inmuebles);
        }

        // Acción GET que muestra el formulario de creación
        public IActionResult Crear()
        {
            // Carga propietarios para el combo del formulario
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();

            return View();
        }

        // Acción POST para crear un inmueble
        [HttpPost]
        public IActionResult Crear(Inmueble inmueble)
        {
            // Si el modelo no pasa validaciones
            if (!ModelState.IsValid)
            {
                // Se vuelven a cargar propietarios porque se pierde el ViewBag
                ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();

                return View(inmueble);
            }

            // Llama al repositorio para insertar
            var resultado = repositorioInmueble.Alta(inmueble);

            // Si devuelve -1 significa error (por ejemplo dirección duplicada)
            if (resultado == -1)
            {
                ViewBag.Error = "La dirección ya existe.";

                // Se vuelve a cargar combo
                ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();

                return View(inmueble);
            }
            TempData["Mensaje"] = "Inmueble creado correctamente.";
            // Si todo salió bien, redirige al listado
            return RedirectToAction(nameof(Index));
        }

        // Acción GET para editar
        public IActionResult Editar(int id)
        {
            // Busca inmueble por id
            var inmueble = repositorioInmueble.Buscar(id);

            if (inmueble == null) return NotFound();

            // Carga propietarios para combo
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();

            return View(inmueble);
        }

        // Acción POST para guardar edición
        [HttpPost]
        public IActionResult Editar(Inmueble inmueble)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                return View(inmueble);
            }

            // Llama al repositorio para actualizar
            repositorioInmueble.Modificacion(inmueble);
            TempData["Mensaje"] = "Inmueble editado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Solo administrador puede eliminar
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var inmueble = repositorioInmueble.Buscar(id);

            if (inmueble == null) return NotFound();

            return View(inmueble);
        }

        // Acción POST que confirma eliminación
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult ConfirmarEliminar(int id)
        {
            var resultado = repositorioInmueble.Baja(id);

            // Si tiene contratos asociados
            if (resultado == -1)
            {
                TempData["Error"] = "No se puede eliminar el inmueble porque tiene contratos asociados.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Mensaje"] = "Inmueble eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Acción GET para ver detalles
        public IActionResult Detalles(int id)
        {
            var inmueble = repositorioInmueble.Buscar(id);

            if (inmueble == null) return NotFound();

            return View(inmueble);
        }
    }
}
