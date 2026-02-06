using InmobiliariaUlP_2025.Repositories.Interfaces;
using InmobiliariaUlP_2025.Repositories.Implementations;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// =========================
// MVC + Localización
// =========================
builder.Services.AddControllersWithViews(options =>
{
    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
        _ => "El valor debe ser un número válido."
    );
})
.AddDataAnnotationsLocalization()
.AddViewLocalization();

// =========================
// AUTH (LOGIN)
// =========================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Login/Denegado";
    });

// =========================
// REPOSITORIOS
// =========================
builder.Services.AddScoped<IRepositorioContrato, RepositorioContrato>();
builder.Services.AddScoped<IRepositorioPago, RepositorioPago>();
builder.Services.AddScoped<IRepositorioInmueble, RepositorioInmueble>();
builder.Services.AddScoped<IRepositorioInquilino, RepositorioInquilino>();
builder.Services.AddScoped<IRepositorioPropietario, RepositorioPropietario>();

// =========================
// CULTURA
// =========================
var culture = new CultureInfo("es-AR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var app = builder.Build();

// =========================
// LOCALIZACIÓN
// =========================
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = new[] { culture },
    SupportedUICultures = new[] { culture }
});

// =========================
// MIDDLEWARE
// =========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// =========================
// RUTAS
// =========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
