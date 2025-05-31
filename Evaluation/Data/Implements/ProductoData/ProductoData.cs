using Data.Implements.BaseData;
using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Implements.ProductoData
{
    public class ProductoData : BaseModelData<Producto>, IProductoData
    {
        public ProductoData(ApplicationDbContext context) : base(context)
        {
        }

        // Método específico: Buscar productos por nombre
        public async Task<List<Producto>> GetByNombreAsync(string nombre)
        {
            return await _dbSet
                .Where(p => p.Nombre.ToLower().Contains(nombre.ToLower()) ||
                           !string.IsNullOrEmpty(p.Descripcion) && p.Descripcion.ToLower().Contains(nombre.ToLower()))
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        // Método específico: Obtener productos con stock disponible
        public async Task<List<Producto>> GetProductosEnStockAsync(int stockMinimo)
        {
            return await _dbSet
                .Where(p => p.Stock > 0)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        // Método específico: Obtener productos sin stock
        public async Task<List<Producto>> GetProductosSinStockAsync()
        {
            return await _dbSet
                .Where(p => p.Stock <= 0)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        // Método específico: Buscar productos por rango de precio
        public async Task<List<Producto>> GetByRangoPrecioAsync(decimal precioMinimo, decimal precioMaximo)
        {
            return await _dbSet
                .Where(p => p.Precio >= precioMinimo && p.Precio <= precioMaximo)
                .OrderBy(p => p.Precio)
                .ToListAsync();
        }

        // Método específico: Actualizar stock de un producto
        public async Task<bool> ActualizarStockAsync(int productoId, int nuevoStock)
        {
            var producto = await _dbSet.FindAsync(productoId);
            if (producto == null) return false;

            producto.Stock = nuevoStock;
            producto.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        // Método específico: Obtener productos recientes
        public async Task<List<Producto>> GetProductosRecientesAsync(int dias = 30)
        {
            var fechaLimite = DateTime.Now.AddDays(-dias);
            return await _dbSet
                .Where(p => p.FechaCreacion >= fechaLimite)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }

        // Override para validaciones específicas en Create
        public override async Task<Producto> CreateAsync(Producto producto)
        {
            // Validaciones específicas para productos
            if (producto.Precio < 0)
            {
                throw new ArgumentException("El precio no puede ser negativo");
            }

            if (producto.Stock < 0)
            {
                throw new ArgumentException("El stock no puede ser negativo");
            }

            return await base.CreateAsync(producto);
        }

        // Override para validaciones específicas en Update
        public override async Task<Producto> UpdateAsync(Producto producto)
        {
            // Validaciones específicas para productos
            if (producto.Precio < 0)
            {
                throw new ArgumentException("El precio no puede ser negativo");
            }

            if (producto.Stock < 0)
            {
                throw new ArgumentException("El stock no puede ser negativo");
            }

            return await base.UpdateAsync(producto);
        }
    }
}