// Importa las clases necesarias del framework MVC (Controller, IActionResult, View, Redirect, etc.)
using Microsoft.AspNetCore.Mvc;

// Permite usar atributos de autorización como [Authorize] y control por roles
using Microsoft.AspNetCore.Authorization;

// Permite usar las clases del modelo, por ejemplo la clase Propietario
using InmobiliariaUlP_2025.Models;

// Permite usar la interfaz del repositorio de propietarios
using InmobiliariaUlP_2025.Repositories.Interfaces;

// Define el espacio de nombres donde está este controlador
namespace InmobiliariaUlP_2025.Controllers
{
    // Atributo que indica que para acceder a este controlador el usuario debe estar logueado
    [Authorize]

    // Define una clase pública llamada PropietariosController
    // Hereda de la clase base Controller (propia de MVC)
    public class PropietariosController : Controller
    {
        // Campo privado y readonly (solo se asigna una vez)
        // Guarda la referencia al repositorio que maneja los propietarios
        private readonly IRepositorioPropietario repositorioPropietario;

        // Constructor del controlador
        // Recibe por inyección de dependencias una implementación del repositorio
        public PropietariosController(IRepositorioPropietario repositorioPropietario)
        {
            // Asigna el repositorio recibido al campo privado de la clase
            this.repositorioPropietario = repositorioPropietario;
        }

        // Acción que responde a GET /Propietarios
        // Devuelve un IActionResult (puede ser View, Redirect, etc.)
        public IActionResult Index()
        {
            // Llama al repositorio para obtener todos los propietarios de la base de datos
            var lista = repositorioPropietario.ObtenerTodos();

            // Devuelve la vista Index.cshtml y le pasa la lista como modelo
            return View(lista);
        }

        // Acción GET que muestra el formulario para crear un propietario
        public IActionResult Crear()
        {
            // Solo devuelve la vista vacía
            return View();
        }

        // Indica que este método responde a una petición HTTP POST (envío de formulario)
        [HttpPost]

        // Acción POST para crear un nuevo propietario
        // Recibe un objeto Propietario que se llena automáticamente con los datos del formulario (Model Binding)
        public IActionResult Crear(Propietario propietario)
        {
            // Verifica si las validaciones del modelo son correctas
            if (!ModelState.IsValid)
                // Si hay errores de validación, vuelve a mostrar la vista con los datos cargados
                return View(propietario);

            // Llama al método Alta del repositorio para guardar el propietario en la base
            var resultado = repositorioPropietario.Alta(propietario);

            // Si el repositorio devuelve -1 significa que hubo un error (por ejemplo DNI o email duplicado)
            if (resultado == -1)
            {
                // Guarda un mensaje de error en ViewBag (se usa en la misma vista)
                ViewBag.Error = "Ya existe un propietario con ese DNI o email.";

                // Devuelve nuevamente la vista con los datos ingresados
                return View(propietario);
            }

            // Guarda un mensaje temporal que sobrevivirá al Redirect
            TempData["Mensaje"] = "Propietario creado correctamente.";

            // Redirige al método Index
            return RedirectToAction(nameof(Index));
        }

        // Acción GET que muestra el formulario de edición
        // Recibe el id del propietario a editar
        public IActionResult Editar(int id)
        {
            // Busca el propietario en la base de datos por su id
            var propietario = repositorioPropietario.Buscar(id);

            // Si no existe, devuelve error 404
            if (propietario == null)
                return NotFound();

            // Devuelve la vista Editar con el propietario como modelo
            return View(propietario);
        }

        // Acción POST que procesa la edición
        [HttpPost]
        public IActionResult Editar(Propietario propietario)
        {
            // Verifica validaciones del modelo
            if (!ModelState.IsValid)
                // Si falla la validación, vuelve a la vista con los datos cargados
                return View(propietario);

            // Llama al repositorio para actualizar los datos en la base
            repositorioPropietario.Modificacion(propietario);

            // Guarda mensaje temporal de éxito
            TempData["Mensaje"] = "Propietario modificado correctamente.";

            // Redirige al Index
            return RedirectToAction(nameof(Index));
        }

        // Solo los usuarios con rol Administrador pueden acceder
        [Authorize(Roles = "Administrador")]

        // Acción GET que muestra la pantalla de confirmación de eliminación
        public IActionResult Eliminar(int id)
        {
            // Busca el propietario por id
            var propietario = repositorioPropietario.Buscar(id);

            // Si no existe, devuelve 404
            if (propietario == null)
                return NotFound();

            // Muestra la vista de confirmación pasando el propietario
            return View(propietario);
        }

        // Solo administradores pueden ejecutar esta acción
        [Authorize(Roles = "Administrador")]

        // Indica que es un POST y que mantiene el nombre lógico "Eliminar"
        [HttpPost, ActionName("Eliminar")]

        // Método que realmente ejecuta la eliminación
        public IActionResult EliminarConfirmado(int id)
        {
            // Llama al repositorio para eliminar el propietario por id
            var resultado = repositorioPropietario.Baja(id);

            // Si devuelve -1 significa que no se puede eliminar (por ejemplo tiene inmuebles asociados)
            if (resultado == -1)
            {
                // Guarda mensaje de error temporal
                TempData["Error"] = "No se puede eliminar el propietario porque tiene inmuebles asociados.";

                // Redirige al Index
                return RedirectToAction(nameof(Index));
            }

            // Si se eliminó correctamente, guarda mensaje de éxito
            TempData["Mensaje"] = "Propietario eliminado correctamente.";

            // Redirige al Index
            return RedirectToAction(nameof(Index));
        }
    }
}
