using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SistemaVenta.DTO;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.API.Utilidad;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador,Empleado,Supervisor")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaServico;

        public CategoriaController(ICategoriaService categoriaServico)
        {
            _categoriaServico = categoriaServico;
        }


        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var rsp = new Response<List<CategoriaDTO>>();
            try
            {
                rsp.Status = true;
                rsp.Value = await _categoriaServico.Lista();
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
