using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IRepositorioUsuario repo;

        public UsuarioController(IRepositorioUsuario repo)
        {
            this.repo = repo;
        }

        // =========================
        // LISTADO (Solo Administrador)
        // =========================

        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View(repo.ObtenerTodos());
        }

        // =========================
        // ALTA (Solo Administrador)
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
            if (!ModelState.IsValid)
                return View(usuario);

            repo.Alta(usuario);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // MODIFICACIÓN (Solo Administrador)
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
        // BAJA (Solo Administrador)
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

        public IActionResult Perfil()
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var usuario = repo.ObtenerPorEmail(email);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost]
        public IActionResult Perfil(Usuario usuario, IFormFile? fotoArchivo)
        {
            var actual = repo.ObtenerPorId(usuario.Id);
            if (actual == null) return NotFound();

            // Actualiza contraseña si se ingresa nueva
            if (!string.IsNullOrEmpty(usuario.Password))
                actual.Password = usuario.Password;

            // Actualiza foto si se carga archivo
            if (fotoArchivo != null && fotoArchivo.Length > 0)
            {
                var carpeta = Path.Combine("wwwroot", "img", "usuarios");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(fotoArchivo.FileName);
                var ruta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    fotoArchivo.CopyTo(stream);
                }

                actual.Foto = "/img/usuarios/" + nombreArchivo;
            }

            repo.Modificacion(actual);

            // 🔄 Re-crear los claims para actualizar la foto sin reloguear
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, actual.Email),
                new Claim(ClaimTypes.Role, actual.Rol),
                new Claim("Id", actual.Id.ToString()),
                new Claim("Foto", actual.Foto ?? "")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            ).Wait();

            TempData["Mensaje"] = "Perfil actualizado correctamente";
            return RedirectToAction("Index", "Propietarios");
        }
    }
}