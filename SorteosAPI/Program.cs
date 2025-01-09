using dotenv.net;
using Microsoft.EntityFrameworkCore;
using SorteosAPI.Models;
using SorteosAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Cargar el archivo .env
DotEnv.Load();

// Obtener la cadena de conexión de la variable de entorno
var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("La variable de entorno 'DefaultConnection' no está configurada.");
}

// Agregar la cadena de conexión a la configuración
builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

// Agregar servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<SorteosDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IAssignedNumberService, AssignedNumberService>();
builder.Services.AddScoped<IListNumberService, ListNumberService>();

var app = builder.Build();
app.UseCors();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();