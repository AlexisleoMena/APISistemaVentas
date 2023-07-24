using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.Model;

namespace SistemaVenta.DAL.Repositorios
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        private readonly DbventaContext _dBContext;

        public VentaRepository(DbventaContext dbContext)
            : base(dbContext)
        {
            _dBContext = dbContext;
        }

        public async Task<Venta> Registrar(Venta modelo)
        {
            Venta ventaGenerada = new Venta();
            using (var transaction = _dBContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (DetalleVenta dv in modelo.DetalleVenta)
                    {
                        Producto productoEncontrado = _dBContext.Productos
                            .Where(p => p.IdProducto == dv.IdProducto)
                            .First();
                        productoEncontrado.Stock = productoEncontrado?.Stock - dv.Cantidad;
                        _dBContext.Productos.Update(productoEncontrado);
                    }
                    await _dBContext.SaveChangesAsync();

                    NumeroDocumento correlativo = _dBContext.NumeroDocumentos.First();

                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaRegistro = DateTime.Now;

                    _dBContext.NumeroDocumentos.Update(correlativo);
                    await _dBContext.SaveChangesAsync();

                    int cantidadDigitos = 4;
                    string ceros = string.Concat(Enumerable.Repeat("0", cantidadDigitos));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();

                    numeroVenta = numeroVenta.Substring(
                        numeroVenta.Length - cantidadDigitos,
                        cantidadDigitos
                    );

                    modelo.NumeroDocumento = numeroVenta;

                    await _dBContext.Venta.AddAsync(modelo);
                    await _dBContext.SaveChangesAsync();
                     
                    ventaGenerada = modelo;

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
                return ventaGenerada;
            }
        }
    }
}
