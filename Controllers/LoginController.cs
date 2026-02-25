using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
    public class LoginController : Controller
    {
        private readonly IRepositorioUsuario repoUsuario;

        public LoginController(IRepositorioUsuario repoUsuario)
        {
            this.repoUsuario = repoUsuario;
        }

        // =========================
        // LOGIN
        // =========================

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string usuario, string clave)
        {
            var user = repoUsuario.ObtenerPorEmail(usuario);

            if (user != null && user.Password == clave)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Rol),
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Foto", user.Foto ?? "")
                };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal
                );

                return RedirectToAction("Index", "Propietarios");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View();
        }

        // =========================
        // LOGOUT
        // =========================

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // ACCESO DENEGADO
        // =========================

        public IActionResult Denegado()
        {
            return View();
        }
    }
}