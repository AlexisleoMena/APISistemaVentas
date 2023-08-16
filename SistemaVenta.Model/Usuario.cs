using System;
using System.Collections.Generic;
using BCryptNet = BCrypt.Net.BCrypt;

namespace SistemaVenta.Model;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string? NombreCompleto { get; set; }

    public string? Correo { get; set; }

    public int? IdRol { get; set; }

    public string? Clave { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual Rol? IdRolNavigation { get; set; }

    public void SetClave(string clave)
    {
        Clave = BCryptNet.HashPassword(clave);
    }

    public bool VerificarClave(string clave)
    {
        return BCryptNet.Verify(clave, Clave);
    }
}
