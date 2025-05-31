using Entity.Dto.ProductoDTO;
using Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IProductoData
    {
        Task<List<Producto>> GetAllAsync();
        Task<Producto> GetByIdAsync(int id);
        Task<Producto> CreateAsync(Producto producto);
        Task<Producto> UpdateAsync(Producto producto);
        Task<bool> DeleteAsync(int id);

        // Métodos específicos para Producto
        Task<List<Producto>> GetByNombreAsync(string nombre);
        Task<List<Producto>> GetProductosEnStockAsync(int stockMinimo);
        Task<List<Producto>> GetProductosSinStockAsync();
        Task<List<Producto>> GetByRangoPrecioAsync(decimal precioMinimo, decimal precioMaximo);
        Task<bool> ActualizarStockAsync(int productoId, int nuevoStock);
        Task<List<Producto>> GetProductosRecientesAsync(int dias = 30);
        Task<bool> ActiveAsync(int id, bool status);
        Task<bool> UpdateProductoDto(UpdateProductoDto dto);
        Task GetByDescripcionAsync(string descripcion);
        Task GetByPriceRangeAsync(decimal precioMinimo, decimal precioMaximo);
        Task GetWithStockAsync(int stockMinimo);
        Task GetWithLowStockAsync(int stockLimite);
        Task<bool> UpdateStockAsync(int productoId, int nuevoStock);
        Task<bool> IsNombreUniqueAsync(string nombre, int? excludeId);
    }
}