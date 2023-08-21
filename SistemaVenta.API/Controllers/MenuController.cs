using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.API.Utilidad;
using SistemaVenta.BLL.Servicios;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DTO;

namespace SistemaVenta.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador,Supervisor,Empleado")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuServicio;

        public MenuController(IMenuService menuServicio)
        {
            _menuServicio = menuServicio;
        }

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista(int idUsuario)
        {
            var rsp = new Response<List<MenuDTO>>();
            try
            {
                rsp.Status = true;
                rsp.Value = await _menuServicio.Lista(idUsuario);
            }
            catch (Exception ex)
            {
                rsp.Status = false;
                rsp.Message = ex.Message;
            }
            return Ok(rsp);
        }
    }
}
