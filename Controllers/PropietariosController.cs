// -----------------------------------------------------------------------------
// using = importamos namespaces para poder usar sus clases sin escribir el nombre completo.
// -----------------------------------------------------------------------------

// Microsoft.AspNetCore.Mvc contiene la clase base "Controller" y tipos como IActionResult.
using Microsoft.AspNetCore.Mvc;

// Importamos el namespace donde está nuestro RepositorioPropietario y el modelo Propietario.
using InmobiliariaUlP_2025.Models;

// -----------------------------------------------------------------------------
// namespace = "apellido lógico" del controlador.
// Normalmente es NombreProyecto.Controllers
// En tu caso: InmobiliariaUlP_2025.Controllers
// -----------------------------------------------------------------------------
namespace InmobiliariaUlP_2025.Controllers
{
    // -------------------------------------------------------------------------
    // CLASE PropietariosController
    //
    // Hereda de "Controller", que es la clase base para controladores MVC.
    // Un controlador responde a URLs como:
    //   /Propietarios/Index
    //   /Propietarios/Crear
    //   etc. (más adelante).
    // -------------------------------------------------------------------------
    public class PropietariosController : Controller
    {
        // Campo privado para guardar una referencia al repositorio.
        // Así podemos llamar a repo.ObtenerTodos(), repo.Alta(), etc.
        private readonly RepositorioPropietario repositorioPropietario;

        // ---------------------------------------------------------------------
        // CONSTRUCTOR
        //
        // El framework (ASP.NET Core) va a crear este controlador e INYECTAR
        // una instancia de RepositorioPropietario automáticamente, gracias a:
        //   builder.Services.AddScoped<RepositorioPropietario>();
        // que agregamos en Program.cs.
        // ---------------------------------------------------------------------
        public PropietariosController(RepositorioPropietario repositorioPropietario)
        {
            // "this.repositorioPropietario" = campo de la clase.
            // "repositorioPropietario" (parámetro) = lo que llega desde afuera.
            // Guardamos el parámetro en el campo para usarlo en los métodos.
            this.repositorioPropietario = repositorioPropietario;
        }

        // ---------------------------------------------------------------------
        // ACCIÓN: Index
        // URL: /Propietarios/Index  (o simplemente /Propietarios)
        //
        // IActionResult = tipo de retorno general para acciones de MVC.
        // Devuelve una "acción" que el framework convierte en HTML, redirección,
        // JSON, etc. En este caso, va a devolver una Vista.
        // ---------------------------------------------------------------------
        public IActionResult Index()
        {
            // Llamamos al repositorio para obtener la lista de todos los propietarios
            // desde la base de datos MySQL.
            var lista = repositorioPropietario.ObtenerTodos();

            // Devolvemos la Vista "Index" (por convención, Views/Propietarios/Index.cshtml)
            // y le pasamos "lista" como modelo (model) para que la vista pueda mostrarla.
            return View(lista);
        }
    }
}
