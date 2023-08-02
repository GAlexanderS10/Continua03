﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BackEnd.Dtos;
using BackEnd.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly DBVETPETContext _context;

        public AutenticacionController(IConfiguration config, DBVETPETContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost]
        [Route("Validar")]
        public IActionResult Validar([FromBody] LoginModel request)
        {
            try
            {
                // Buscar al usuario por su nombre de usuario en la base de datos, incluyendo los roles
                var usuario = _context.Usuarios.Include(u => u.Rols).FirstOrDefault(u => u.UserName == request.UserName);

                // Verificar si el usuario existe y si la contraseña es válida
                if (usuario != null && ValidarPassword(usuario.Password, request.Password))
                {
                    // Generar el token JWT incluyendo el rol del usuario como claim
                    var token = GenerarToken(usuario);

                    // Devolver el token y el DNI del usuario como resultado exitoso
                    return StatusCode(StatusCodes.Status200OK, new { token, dni = usuario.Dni });
                }

                // Si las credenciales no son válidas, devolver un resultado de error
                return StatusCode(StatusCodes.Status401Unauthorized, new { token = "", dni = "" });
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir durante el proceso de autenticación
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error de autenticación", error = ex.Message });
            }
        }

        private string GenerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _config.GetSection("settings").GetSection("secretKey").Value;
            var keyBytes = Encoding.ASCII.GetBytes(secretKey);

            // Crear una lista de claims, incluyendo el rol del usuario
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, usuario.UserName)
    };

            // Agregar cada rol del usuario como una claim
            foreach (var rol in usuario.Rols)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol.Tipo));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool ValidarPassword(string passwordHash, string password)
        {
            // Utilizamos BCrypt para verificar si la contraseña ingresada coincide con la contraseña almacenada en la base de datos
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}