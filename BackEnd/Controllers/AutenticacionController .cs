using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BackEnd.Dtos;
using BackEnd.Models;
using System.Security.Claims;
using System;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
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
                var usuario = _context.Usuarios.Include(u => u.Rols).FirstOrDefault(u => u.UserName == request.UserName);

                if (usuario != null && ValidarPassword(usuario.Password, request.Password))
                {
                    var token = GenerarToken(usuario);

                    var roles = usuario.Rols.Select(r => r.Tipo).ToList();

                    return Ok(new { token, dni = usuario.Dni, roles });
                }

                return Unauthorized(new { token = "", dni = "" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error de autenticación", error = ex.Message });
            }
        }

        private string GenerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = _config.GetSection("settings");
            var secretKey = jwtSettings.GetValue<string>("secretkey");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UserName)
            };

            foreach (var rol in usuario.Rols)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol.Tipo));
            }

            var keyBytes = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool ValidarPassword(string passwordHash, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
