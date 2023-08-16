using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.API.Utilidad;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Empleado,Supervisor")]
    [ApiController]
    public class RolController : ControllerBase
    {
        private readonly IRolService _rolServicio;

        public RolController(IRolService rolServicio)
        {
            _rolServicio = rolServicio;
        }

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var rsp = new Response<List<RolDTO>>();
            try
            {
                rsp.Status = true;
                rsp.Value = await _rolServicio.Lista();
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
