using dotenv.net;
using Microsoft.EntityFrameworkCore;
using SorteosAPI.Models;
using SorteosAPI.Services;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();

var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("La variable de entorno 'DefaultConnection' no está configurada.");
}

builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

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
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IRaffleService, RaffleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRaffleAssignmentService, RaffleAssignmentService>();

var app = builder.Build();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();