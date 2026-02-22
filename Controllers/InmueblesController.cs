// ===============================
// ESPACIOS DE NOMBRES (USING)
// ===============================

// Permite usar las clases base del framework MVC como:
// Controller, IActionResult, View(), RedirectToAction(), NotFound(), etc.
using Microsoft.AspNetCore.Mvc;

// Permite usar autenticación y autorización mediante atributos como:
// [Authorize] y control de roles.
using Microsoft.AspNetCore.Authorization;

// Permite acceder a las clases del modelo del dominio:
// Inmueble, Propietario, Disponibilidad, etc.
using InmobiliariaUlP_2025.Models;

// Permite utilizar las interfaces de los repositorios.
// Esto desacopla la lógica de acceso a datos del controlador.
using InmobiliariaUlP_2025.Repositories.Interfaces;


// ===============================
// DEFINICIÓN DEL CONTROLADOR
// ===============================

namespace InmobiliariaUlP_2025.Controllers
{
    /*
     * Un Controller en ASP.NET MVC es una clase que:
     * 1) Recibe peticiones HTTP del usuario.
     * 2) Ejecuta lógica de negocio.
     * 3) Se comunica con la capa de datos (Repositorio).
     * 4) Devuelve una respuesta (Vista, Redirect, JSON, etc).
     */

    // Indica que todas las acciones requieren un usuario autenticado.
    [Authorize]
    public class InmueblesController : Controller
    {
        // ===============================
        // INYECCIÓN DE DEPENDENCIAS
        // ===============================

        /*
         * readonly significa que la variable solo puede asignarse
         * una vez (en el constructor).
         * 
         * Se usan interfaces para aplicar el principio
         * de Inversión de Dependencias (SOLID).
         */

        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly IRepositorioPropietario repositorioPropietario;

        /*
         * El constructor recibe los repositorios por inyección
         * de dependencias (Dependency Injection).
         * 
         * Esto lo gestiona el contenedor de servicios
         * configurado en Program.cs.
         */
        public InmueblesController(
            IRepositorioInmueble repositorioInmueble,
            IRepositorioPropietario repositorioPropietario)
        {
            this.repositorioInmueble = repositorioInmueble;
            this.repositorioPropietario = repositorioPropietario;
        }

        // ======================================================
        // INDEX - LISTADO PRINCIPAL CON FILTROS
        // ======================================================

        /*
         * IActionResult es un tipo de retorno que representa
         * una respuesta HTTP abstracta.
         * 
         * Puede devolver:
         * - View()
         * - Redirect()
         * - NotFound()
         * - Json()
         * etc.
         * 
         * Disponibilidad? indica que el parámetro es nullable.
         * El signo ? significa que puede ser null.
         */
        public IActionResult Index(
            Disponibilidad? dispo,
            int? propietarioId,
            DateTime? inicio,
            DateTime? fin)
        {
            /*
             * ViewBag es un objeto dinámico que permite enviar datos
             * del Controller a la Vista sin tipado fuerte.
             */
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();

            /*
             * Se obtiene la lista completa desde la base de datos.
             * Esta llamada va:
             * Controller → Repositorio → MySQL
             */
            var inmuebles = repositorioInmueble.ObtenerTodos();

            /*
             * DateTime representa fecha y hora.
             * DateTime? significa que puede ser null.
             * 
             * HasValue verifica que no sea null.
             */
            if (inicio.HasValue && fin.HasValue)
            {
                /*
                 * DateOnly almacena solo la fecha sin hora.
                 * Se convierte desde DateTime.
                 */
                var inicioDateOnly = DateOnly.FromDateTime(inicio.Value);
                var finDateOnly = DateOnly.FromDateTime(fin.Value);

                /*
                 * Se consulta al repositorio los inmuebles
                 * que no tengan contratos superpuestos en ese rango.
                 */
                inmuebles = repositorioInmueble
                    .ObtenerDisponiblesEntreFechas(inicioDateOnly, finDateOnly)
                    .ToList();
            }

            /*
             * Filtrado en memoria usando LINQ.
             * Where aplica una condición.
             */
            if (dispo.HasValue)
            {
                inmuebles = inmuebles
                    .Where(i => i.Disponibilidad == dispo.Value)
                    .ToList();
            }

            if (propietarioId.HasValue)
            {
                inmuebles = inmuebles
                    .Where(i => i.PropietarioId == propietarioId.Value)
                    .ToList();
            }

            /*
             * View(inmuebles) envía el modelo a la Vista.
             * La Vista lo recibe como @model IEnumerable<Inmueble>
             */
            return View(inmuebles);
        }


        // ======================================================
        // CREAR (GET)
        // ======================================================

        /*
         * Método que responde a una petición HTTP GET.
         * Solo muestra el formulario.
         */
        public IActionResult Crear()
        {
            // Carga propietarios para el combo desplegable.
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();

            return View();
        }


        // ======================================================
        // CREAR (POST)
        // ======================================================

        /*
         * [HttpPost] indica que responde a una petición POST.
         * POST se usa cuando se modifican datos.
         */
        [HttpPost]
        public IActionResult Crear(Inmueble inmueble)
        {
            /*
             * ModelState contiene el resultado de las validaciones
             * definidas con DataAnnotations en el modelo.
             */
            if (!ModelState.IsValid)
            {
                ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                return View(inmueble);
            }

            /*
             * Alta ejecuta un INSERT en la base de datos.
             */
            var resultado = repositorioInmueble.Alta(inmueble);

            /*
             * -1 puede representar una regla de negocio incumplida.
             * En este caso dirección duplicada.
             */
            if (resultado == -1)
            {
                ViewBag.Error = "La dirección ya existe.";
                ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                return View(inmueble);
            }

            /*
             * TempData permite enviar mensajes entre requests.
             * Se usa en patrón PRG (Post-Redirect-Get).
             */
            TempData["Mensaje"] = "Inmueble creado correctamente.";

            return RedirectToAction(nameof(Index));
        }


        // ======================================================
        // ELIMINAR
        // ======================================================

        /*
         * Solo usuarios con rol Administrador
         * pueden ejecutar esta acción.
         */
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var inmueble = repositorioInmueble.Buscar(id);

            if (inmueble == null)
                return NotFound();

            return View(inmueble);
        }

        /*
         * Confirmación de eliminación.
         * También restringida por rol.
         */
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult ConfirmarEliminar(int id)
        {
            var resultado = repositorioInmueble.Baja(id);

            /*
             * Regla de integridad referencial:
             * No eliminar si tiene contratos asociados.
             */
            if (resultado == -1)
            {
                TempData["Error"] =
                    "No se puede eliminar el inmueble porque tiene contratos asociados.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Mensaje"] = "Inmueble eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}