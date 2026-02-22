using Microsoft.AspNetCore.Mvc;              // Permite usar Controller, IActionResult, View, etc.
using Microsoft.AspNetCore.Authorization;    // Permite usar [Authorize] y control por roles.
using System.Security.Claims;                // Permite acceder a datos del usuario logueado (claims).
using InmobiliariaUlP_2025.Models;           // Modelos del sistema (Usuario).
using InmobiliariaUlP_2025.Repositories.Interfaces; // Interfaces para acceso a base de datos.

namespace InmobiliariaUlP_2025.Controllers
{
    // Obliga a que el usuario esté autenticado para usar este controlador.
    [Authorize]
    public class UsuarioController : Controller
    {
        // Repositorio que permite acceder a la tabla Usuarios.
        private readonly IRepositorioUsuario repo;

        // Constructor: ASP.NET inyecta el repositorio automáticamente.
        public UsuarioController(IRepositorioUsuario repo)
        {
            this.repo = repo;
        }

        // =========================
        // LISTADO (Solo Administrador)
        // =========================

        // Solo usuarios con rol Administrador pueden ver la lista.
        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            // Obtiene todos los usuarios y los envía a la vista.
            return View(repo.ObtenerTodos());
        }

        // =========================
        // CREAR USUARIO (Solo Admin)
        // =========================

        [Authorize(Roles = "Administrador")]
        public IActionResult Crear()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult Crear(Usuario usuario)
        {
            // Verifica validaciones del modelo.
            if (!ModelState.IsValid)
                return View(usuario);

            // Inserta en la base.
            repo.Alta(usuario);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDITAR (Solo Admin)
        // =========================

        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(int id)
        {
            var usuario = repo.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult Editar(Usuario usuario)
        {
            if (!ModelState.IsValid)
                return View(usuario);

            repo.Modificacion(usuario);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // ELIMINAR (Solo Admin)
        // =========================

        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var usuario = repo.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            repo.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // PERFIL (Usuario Logueado)
        // =========================

        // Permite que cualquier usuario logueado edite SU propio perfil.
        public IActionResult Perfil()
        {
            // Obtiene el email guardado en el login (claim Name).
            var email = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            // Busca el usuario en la base.
            var usuario = repo.ObtenerPorEmail(email);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost]
        public IActionResult Perfil(Usuario usuario, IFormFile? fotoArchivo)
        {
            // Busca el usuario actual en base.
            var actual = repo.ObtenerPorId(usuario.Id);
            if (actual == null) return NotFound();

            // Si escribió nueva contraseña, la actualiza.
            if (!string.IsNullOrEmpty(usuario.Password))
                actual.Password = usuario.Password;

            // Si subió una foto nueva.
            if (fotoArchivo != null && fotoArchivo.Length > 0)
            {
                // Guarda el archivo en wwwroot/img/usuarios
                var ruta = Path.Combine("wwwroot/img/usuarios", fotoArchivo.FileName);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    fotoArchivo.CopyTo(stream);
                }

                // Guarda la ruta en la base.
                actual.Foto = "/img/usuarios/" + fotoArchivo.FileName;
            }

            // Actualiza en la base.
            repo.Modificacion(actual);

            TempData["Mensaje"] = "Perfil actualizado correctamente";

            return RedirectToAction(nameof(Index), "Propietarios");
        }
    }
}