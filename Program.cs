// Este using permite usar RepositorioPropietario sin escribir el namespace completo.
using InmobiliariaUlP_2025.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Registramos el soporte para controladores + vistas (MVC).
builder.Services.AddControllersWithViews();

// Registramos RepositorioPropietario como servicio de "Scoped".
// Scoped = se crea una instancia por cada request HTTP.
// Así luego lo podemos pedir en los controladores por constructor.
builder.Services.AddScoped<RepositorioPropietario>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
