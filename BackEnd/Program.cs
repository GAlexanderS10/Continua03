using Microsoft.EntityFrameworkCore;
using BackEnd.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;
using AutoMapper;
using BackEnd.Mappers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("appsettings.json");
var secretKey = builder.Configuration.GetSection("settings").GetSection("secretKey").ToString();// "=Codig0Estudiant3=";
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(config => {

    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config => {
    config.RequireHttpsMetadata = false;
    config.SaveToken = false;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});



builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
// Configuración del contexto de la base de datos
builder.Services.AddDbContext<DBVETPETContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VetPetCn")));

// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(Program)); // Aquí pasamos la clase Program o cualquier otra clase que contenga tus perfiles de mapeo
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles)); // Agregamos el perfil de mapeo creado


var app = builder.Build();

// Configurar la canalización de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// Configurar el middleware de enrutamiento
app.UseRouting();

// Configurar la política CORS
app.UseCors(policy =>
{
    policy.WithOrigins("http://localhost:3000") // React
          .AllowAnyHeader()
          .AllowAnyMethod();
});

// Crear el directorio "Uploads" si no existe
var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "Uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

// Configurar el middleware de archivos estáticos para servir las imágenes desde la carpeta "Uploads"
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/Uploads"
});

// Configurar los endpoints
app.MapControllers();

app.Run();
