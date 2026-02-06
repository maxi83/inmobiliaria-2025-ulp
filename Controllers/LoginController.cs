using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace InmobiliariaUlP_2025.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string usuario, string clave)
        {
            // Usuarios del enunciado (simple y válido)
            // admin / 123 → Administrador
            // empleado / 123 → Empleado

            if (
                (usuario == "admin" && clave == "123") ||
                (usuario == "empleado" && clave == "123")
            )
            {
                var rol = usuario == "admin"
                    ? "Administrador"
                    : "Empleado";

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario),
                    new Claim(ClaimTypes.Role, rol)
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

                return RedirectToAction("Index", "Propietarios");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            ).Wait();

            return RedirectToAction("Index");
        }

        public IActionResult Denegado()
        {
            return View();
        }
    }
}
