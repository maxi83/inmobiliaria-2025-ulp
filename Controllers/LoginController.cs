using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using InmobiliariaUlP_2025.Repositories.Interfaces;
using InmobiliariaUlP_2025.Models;

namespace InmobiliariaUlP_2025.Controllers
{
    // Controlador encargado de autenticar usuarios
    public class LoginController : Controller
    {
        // Repositorio para consultar usuarios en la base
        private readonly IRepositorioUsuario repoUsuario;

        // Inyección de dependencia
        public LoginController(IRepositorioUsuario repoUsuario)
        {
            this.repoUsuario = repoUsuario;
        }

        // Muestra la vista de login
        public IActionResult Index()
        {
            return View();
        }

        // Procesa el login
        [HttpPost]
        public IActionResult Index(string usuario, string clave)
        {
            // Busca el usuario por email
            var user = repoUsuario.ObtenerPorEmail(usuario);

            // Verifica que exista y que la contraseña coincida
            if (user != null && user.Password == clave)
            {
                // Se crean los claims (datos del usuario logueado)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Rol),
                    new Claim("Id", user.Id.ToString()), // importante para perfil
                    new Claim("Foto", user.Foto ?? "")
                };

                // Se crea la identidad del usuario
                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                var principal = new ClaimsPrincipal(identity);

                // Se inicia sesión
                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal
                ).Wait();

                return RedirectToAction("Index", "Propietarios");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View();
        }

        // Cierra sesión
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            ).Wait();

            return RedirectToAction("Index");
        }

        // Vista de acceso denegado
        public IActionResult Denegado()
        {
            return View();
        }
    }
}