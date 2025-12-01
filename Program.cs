using InmobiliariaUlP_2025.Repositories.Implementations;
using InmobiliariaUlP_2025.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Registramos los repositorios correctamente
builder.Services.AddScoped<IPropietarioRepository, PropietarioRepository>();
builder.Services.AddScoped<IRepositorioInquilino, RepositorioInquilino>();




var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Propietarios}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
