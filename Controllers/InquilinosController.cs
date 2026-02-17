// Importa las clases necesarias para trabajar con MVC (Controller, IActionResult, View, Redirect, etc.)
using Microsoft.AspNetCore.Mvc;

// Permite usar atributos de autorización como [Authorize] y control por roles
using Microsoft.AspNetCore.Authorization;

// Permite usar las clases del modelo, por ejemplo la clase Inquilino
using InmobiliariaUlP_2025.Models;

// Permite usar la interfaz del repositorio de inquilinos
using InmobiliariaUlP_2025.Repositories.Interfaces;

// Define el espacio de nombres (organización lógica del proyecto)
namespace InmobiliariaUlP_2025.Controllers
{
    // Indica que todo el controlador requiere que el usuario esté logueado
    [Authorize]

    // Define la clase pública InquilinosController que hereda de Controller
    public class InquilinosController : Controller
    {
        // Campo privado y readonly que guarda la referencia al repositorio
        // readonly significa que solo puede asignarse una vez (normalmente en el constructor)
        private readonly IRepositorioInquilino repositorioInquilino;

        // Constructor del controlador
        // ASP.NET inyecta automáticamente una implementación de IRepositorioInquilino
        public InquilinosController(IRepositorioInquilino repositorioInquilino)
        {
            // Se asigna la instancia recibida al campo privado
            this.repositorioInquilino = repositorioInquilino;
        }

        // Acción que responde a GET /Inquilinos
        // Devuelve un IActionResult (puede ser View, Redirect, NotFound, etc.)
        public IActionResult Index()
        {
            // Llama al repositorio para obtener todos los inquilinos de la base de datos
            var lista = repositorioInquilino.ObtenerTodos();

            // Devuelve la vista Index.cshtml y le pasa la lista como modelo
            return View(lista);
        }

        // Acción GET que muestra el formulario para crear un nuevo inquilino
        public IActionResult Crear()
        {
            // Devuelve la vista vacía
            return View();
        }

        // Indica que este método responde a una petición HTTP POST (envío de formulario)
        [HttpPost]

        // Acción POST que recibe un objeto Inquilino (llenado automáticamente por Model Binding)
        public IActionResult Crear(Inquilino inquilino)
        {
            // Verifica si el modelo cumple con todas las validaciones (DataAnnotations)
            if (!ModelState.IsValid)
                // Si hay errores, vuelve a mostrar la vista con los datos cargados
                return View(inquilino);

            // Llama al repositorio para insertar el nuevo inquilino en la base
            var resultado = repositorioInquilino.Alta(inquilino);

            // Si devuelve -1 significa que hubo un error (por ejemplo DNI o email duplicado)
            if (resultado == -1)
            {
                // Guarda mensaje de error en ViewBag (vive solo en esta petición)
                ViewBag.Error = "Ya existe un inquilino con ese DNI o email.";

                // Devuelve la vista con los datos cargados
                return View(inquilino);
            }

            // Guarda un mensaje temporal que sobrevivirá al Redirect
            TempData["Mensaje"] = "Inquilino creado correctamente.";

            // Redirige al método Index
            return RedirectToAction(nameof(Index));
        }

        // Acción GET para editar un inquilino existente
        // Recibe el id del inquilino a editar
        public IActionResult Editar(int id)
        {
            // Busca el inquilino en la base por su id
            var inquilino = repositorioInquilino.ObtenerPorId(id);

            // Si no existe, devuelve error 404
            if (inquilino == null)
                return NotFound();

            // Devuelve la vista Editar con el inquilino como modelo
            return View(inquilino);
        }

        // Acción POST que procesa la edición del inquilino
        [HttpPost]
        public IActionResult Editar(Inquilino inquilino)
        {
            // Verifica validaciones del modelo
            if (!ModelState.IsValid)
                // Si falla la validación, vuelve a la vista con los datos cargados
                return View(inquilino);

            // Llama al repositorio para actualizar el inquilino en la base
            repositorioInquilino.Modificacion(inquilino);

            // Guarda mensaje temporal de éxito
            TempData["Mensaje"] = "Inquilino modificado correctamente.";

            // Redirige al Index
            return RedirectToAction(nameof(Index));
        }

        // Solo los usuarios con rol Administrador pueden acceder a esta acción
        [Authorize(Roles = "Administrador")]

        // Acción GET que muestra la pantalla de confirmación de eliminación
        public IActionResult Eliminar(int id)
        {
            // Busca el inquilino por id
            var inquilino = repositorioInquilino.ObtenerPorId(id);

            // Si no existe, devuelve 404
            if (inquilino == null)
                return NotFound();

            // Muestra la vista de confirmación pasando el inquilino como modelo
            return View(inquilino);
        }

        // Solo administradores pueden ejecutar esta acción
        [Authorize(Roles = "Administrador")]

        // Indica que es POST y que su nombre lógico para MVC es "Eliminar"
        // Aunque el método se llame EliminarConfirmado en C#
        [HttpPost, ActionName("Eliminar")]

        // Método que ejecuta realmente la eliminación
        public IActionResult EliminarConfirmado(int id)
        {
            // Llama al repositorio para eliminar el inquilino por id
            var resultado = repositorioInquilino.Baja(id);

            // Si devuelve -1 significa que no se puede eliminar (por ejemplo tiene contratos asociados)
            if (resultado == -1)
            {
                // Guarda mensaje de error temporal
                TempData["Error"] = "No se puede eliminar el inquilino porque tiene contratos asociados.";

                // Redirige al Index
                return RedirectToAction(nameof(Index));
            }

            // Si se eliminó correctamente, guarda mensaje de éxito
            TempData["Mensaje"] = "Inquilino eliminado correctamente.";

            // Redirige al Index
            return RedirectToAction(nameof(Index));
        }
    }
}
