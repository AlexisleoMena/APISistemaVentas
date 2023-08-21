using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SistemaVenta.BLL.Servicios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _usuarioRepositorio;
        private readonly IMapper _mapper;

        public UsuarioService(IGenericRepository<Usuario> usuarioRepositorio, IMapper mapper)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _mapper = mapper;
        }

        public async Task<UsuarioDTO> Crear(UsuarioDTO modelo)
        {
            try
            {
                var modeloMapeado = _mapper.Map<Usuario>(modelo);
                modeloMapeado.SetClave(modelo.Clave!);

                var usuarioCreado = await _usuarioRepositorio.Crear(modeloMapeado);

                if (usuarioCreado.IdUsuario == 0)
                    throw new TaskCanceledException("No se pudo crear");

                var query = await _usuarioRepositorio.Consultar(
                    u => u.IdUsuario == usuarioCreado.IdUsuario
                );
                usuarioCreado = query.Include(rol => rol.IdRolNavigation).First();

                return _mapper.Map<UsuarioDTO>(usuarioCreado);
            }
            catch(Exception ex)
            {
                throw new Exception($"Error al crear el usuario: {ex.Message}");
            }
        }

        public async Task<bool> Editar(UsuarioDTO modelo)
        {
            try
            {
                var usuarioModelo = _mapper.Map<Usuario>(modelo);
                var usuarioEncontrado = await _usuarioRepositorio.Obtener(
                    u => u.IdUsuario == usuarioModelo.IdUsuario
                );

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                usuarioEncontrado.NombreCompleto = usuarioModelo.NombreCompleto;
                usuarioEncontrado.Correo = usuarioModelo.Correo;
                usuarioEncontrado.IdRol = usuarioModelo.IdRol;
                usuarioEncontrado.EsActivo = usuarioModelo.EsActivo;
                //usuarioEncontrado.SetClave(usuarioModelo.Clave!);

                bool respuesta = await _usuarioRepositorio.Editar(usuarioEncontrado);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar");

                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var usuarioEncontrado = await _usuarioRepositorio.Obtener(u => u.IdUsuario == id);

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                bool respuesta = await _usuarioRepositorio.Eliminar(usuarioEncontrado);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo eliminar");

                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<UsuarioDTO>> Lista()
        {
            try
            {
                var queryUsuario = await _usuarioRepositorio.Consultar();
                var listaUsuarios = queryUsuario.Include(rol => rol.IdRolNavigation).ToList();

                return _mapper.Map<List<UsuarioDTO>>(listaUsuarios);
            }
            catch
            {
                throw;
            }
        }

        public async Task<SesionDTO> ValidarCredenciales(string correo, string clave)
        {
            try
            {
                var queryUsuarios = await _usuarioRepositorio.Consultar(u => u.Correo == correo);
                var usuarios = await queryUsuarios.Include(u => u.IdRolNavigation).ToListAsync();

                var usuario = usuarios.FirstOrDefault(u => u.VerificarClave(clave));

                if (usuario == null)
                    throw new TaskCanceledException("El usuario no existe o la contraseña es incorrecta");

                return _mapper.Map<SesionDTO>(usuario);
            }
            catch
            {
                throw;
            }
        }
        
    }
}
