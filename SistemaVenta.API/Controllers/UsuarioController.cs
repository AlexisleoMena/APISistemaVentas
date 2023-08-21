using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

using SistemaVenta.DTO;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.API.Utilidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioServicio;
        private readonly IConfiguration _configuration;

        public UsuarioController(IUsuarioService usuarioServicio, IConfiguration configuration)
        {
            _usuarioServicio = usuarioServicio;
            _configuration = configuration;
        }

        [Authorize("Admin")]
        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var rsp = new Response<List<UsuarioDTO>>();
            try
            {
                rsp.Status = true;
                rsp.Value = await _usuarioServicio.Lista();
            }
            catch (Exception ex)
            {
                rsp.Status = false;
                rsp.Message = ex.Message;
            }
            return Ok(rsp);
        }

        [HttpPost]
        [Route("iniciarSesion")]
        public async Task<IActionResult> IniciarSesion([FromBody] LoginDTO login )
        {
            var rsp = new Response<SesionDTO>();

            try
            {
                rsp.Status = true;
                SesionDTO user = await _usuarioServicio.ValidarCredenciales(login.Correo, login.Clave);
                user.Token = GenerateToken(user);
                rsp.Value = user;
            }
            catch (Exception ex)
            {
                rsp.Status = false;
                rsp.Message = ex.Message;
            }
            return Ok(rsp);
        }

        [HttpPost]
        [Route("Guardar")]
        public async Task<IActionResult> Guardar([FromBody] UsuarioDTO usuario)
        {
            var rsp = new Response<UsuarioDTO>();

            try
            {
                rsp.Status = true;
                rsp.Value = await _usuarioServicio.Crear(usuario);
            }
            catch (Exception ex)
            {
                rsp.Status = false;
                rsp.Message = ex.Message;
            }
            return Ok(rsp);
        }

        [Authorize("Admin")]
        [HttpPut]
        [Route("Editar")]
        public async Task<IActionResult> Editar([FromBody] UsuarioDTO usuario)
        {
            var rsp = new Response<bool>();

            try
            {
                rsp.Status = true;
                rsp.Value = await _usuarioServicio.Editar(usuario);
            }
            catch (Exception ex)
            {
                rsp.Status = false;
                rsp.Message = ex.Message;
            }
            return Ok(rsp);
        }


        [Authorize("Admin")]
        [HttpDelete]
        [Route("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var rsp = new Response<bool>();

            try
            {
                rsp.Status = true;
                rsp.Value = await _usuarioServicio.Eliminar(id);
            }
            catch (Exception ex)
            {
                rsp.Status = false;
                rsp.Message = ex.Message;
            }
            return Ok(rsp);
        }

        [Authorize]
        [HttpPost]
        [Route("VerificarToken")]
        public IActionResult VerificarToken(SesionDTO user)
        {
            var rsp = new Response<SesionDTO>();

            try
            {
                rsp.Status = true;
                string token = GenerateToken(user);
                user.Token = token;
                rsp.Value = user;
            }
            catch (Exception ex)
            {
                rsp.Status = false;
                rsp.Message = ex.Message;
            }
            return Ok(rsp);
        }

        private string GenerateToken(SesionDTO user)
        {

            var claims = new[] {
                new Claim(ClaimTypes.Email, user.Correo),
                new Claim(ClaimTypes.Role, user.RolDescripcion),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Key").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(10),
                signingCredentials: creds
            );
            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }

    }
}
