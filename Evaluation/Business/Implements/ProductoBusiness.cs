using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dto.ProductoDTO;
using Entity.Dtos.Base;
using Entity.Model;
using Entity.Model.Base;
using Microsoft.Extensions.Logging;

namespace Business.Implements
{
    /// <summary>
    /// Contiene la lógica de negocio específica para la entidad Producto.
    /// Implementa la interfaz base IBaseBusiness.
    /// </summary>
    public class ProductoBusiness : IBaseBusiness<Producto, ProductoDTO>
    {
        private readonly IProductoData _productoData;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductoBusiness> _logger;

        public ProductoBusiness(IProductoData productoData, IMapper mapper, ILogger<ProductoBusiness> logger)
        {
            _productoData = productoData;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las entidades desde la base de datos.
        /// </summary>
        /// <returns>Una colección de objetos de tipo ProductoDTO.</returns>
        public async Task<List<ProductoDTO>> GetAllAsync()
        {
            try
            {
                var productos = await _productoData.GetAllAsync();
                return _mapper.Map<List<ProductoDTO>>(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los productos");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un DTO específico por su ID.
        /// </summary>
        /// <param name="id">Identificador único del producto.</param>
        /// <returns>Un objeto ProductoDTO si se encuentra; de lo contrario, null.</returns>
        public async Task<ProductoDTO> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("ID inválido.", nameof(id));

                var producto = await _productoData.GetByIdAsync(id);
                return _mapper.Map<ProductoDTO>(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo producto a partir de un DTO.
        /// </summary>
        /// <param name="dto">Objeto de transferencia con los datos del producto.</param>
        /// <returns>El DTO del producto creado.</returns>
        public async Task<ProductoDTO> CreateAsync(ProductoDTO dto)
        {
            try
            {
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto));

                // Validar nombre único
                if (!await ValidateUniqueNombreAsync(dto.Nombre))
                    throw new InvalidOperationException("El nombre del producto ya existe en el sistema.");

                var producto = _mapper.Map<Producto>(dto);
                producto.CreatedAt = DateTime.UtcNow;
                producto.UpdatedAt = DateTime.UtcNow;

                var productoCreado = await _productoData.CreateAsync(producto);
                return _mapper.Map<ProductoDTO>(productoCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un registro existente a partir de un DTO.
        /// </summary>
        /// <param name="dto">Objeto de transferencia con los datos actualizados.</param>
        /// <returns>El DTO actualizado o una excepción si falla.</returns>
        public async Task<ProductoDTO> UpdateAsync(ProductoDTO dto)
        {
            try
            {
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto));

                if (dto.Id <= 0)
                    throw new ArgumentException("ID inválido.", nameof(dto.Id));

                // Verificar que el producto existe
                var productoExistente = await _productoData.GetByIdAsync(dto.Id);
                if (productoExistente == null)
                    throw new InvalidOperationException($"Producto con ID {dto.Id} no encontrado.");

                // Validar nombre único (excluyendo el producto actual)
                if (!await ValidateUniqueNombreAsync(dto.Nombre, dto.Id))
                    throw new InvalidOperationException("El nombre del producto ya existe en el sistema.");

                var producto = _mapper.Map<Producto>(dto);
                producto.UpdatedAt = DateTime.UtcNow;
                producto.CreatedAt = productoExistente.CreatedAt; // Preservar fecha de creación

                var productoActualizado = await _productoData.UpdateAsync(producto);
                return _mapper.Map<ProductoDTO>(productoActualizado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto con ID: {Id}", dto?.Id ?? 0);
                throw;
            }
        }

        /// <summary>
        /// Elimina permanentemente un registro del sistema.
        /// </summary>
        /// <param name="id">Identificador del registro a eliminar.</param>
        /// <returns>True si la operación fue exitosa; false en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("ID inválido.", nameof(id));

                // Verificar que el producto existe
                var producto = await _productoData.GetByIdAsync(id);
                if (producto == null)
                    throw new InvalidOperationException($"Producto con ID {id} no encontrado.");

                return await _productoData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Actualiza parcialmente un producto.
        /// </summary>
        public async Task<bool> UpdatePartialProductoAsync(UpdateProductoDto dto)
        {
            try
            {
                if (dto.Id <= 0)
                    throw new ArgumentException("ID inválido.");

                // Verificar que el producto existe
                var productoExistente = await _productoData.GetByIdAsync(dto.Id);
                if (productoExistente == null)
                    throw new InvalidOperationException($"Producto con ID {dto.Id} no encontrado.");

                return await _productoData.UpdateProductoDto(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente producto con ID: {Id}", dto?.Id ?? 0);
                throw;
            }
        }

        /// <summary>
        /// Activa o desactiva un producto (eliminación lógica).
        /// </summary>
        public async Task<bool> DeleteLogicProductoAsync(DeleteLogicProductoDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0)
                    throw new ArgumentException("El ID del producto es inválido.");

                var exists = await _productoData.GetByIdAsync(dto.Id);
                if (exists == null)
                    throw new InvalidOperationException($"Producto con ID {dto.Id} no encontrado.");

                return await _productoData.ActiveAsync(dto.Id, dto.Status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar eliminación lógica del producto con ID: {Id}", dto?.Id ?? 0);
                throw;
            }
        }

        /// <summary>
        /// Busca productos por nombre.
        /// </summary>
        public async Task<IEnumerable<ProductoDTO>> GetProductosByNombreAsync(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("El nombre no puede estar vacío.");

                var productos = await _productoData.GetByNombreAsync(nombre);
                return _mapper.Map<IEnumerable<ProductoDTO>>(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos por nombre: {Nombre}", nombre);
                throw;
            }
        }
        


        /// Obtiene productos con stock disponible.
        /// </summary>
        public async Task<IEnumerable<ProductoDTO>> GetProductosWithStockAsync(int stockMinimo = 1)
        {
            try
            {
                if (stockMinimo < 0)
                    throw new ArgumentException("El stock mínimo no puede ser negativo.");

                var productos = await _productoData.GetProductosEnStockAsync(stockMinimo); // Cambiar método para que devuelva una lista de productos
                return _mapper.Map<IEnumerable<ProductoDTO>>(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos con stock mínimo: {StockMinimo}", stockMinimo);
                throw;
            }
        }

        

        /// <summary>
        /// Actualiza el stock de un producto.
        /// </summary>
        public async Task<bool> UpdateStockAsync(int productoId, int nuevoStock)
        {
            try
            {
                if (productoId <= 0)
                    throw new ArgumentException("ID del producto inválido.");

                if (nuevoStock < 0)
                    throw new ArgumentException("El stock no puede ser negativo.");

                // Verificar que el producto existe
                var producto = await _productoData.GetByIdAsync(productoId);
                if (producto == null)
                    throw new InvalidOperationException($"Producto con ID {productoId} no encontrado.");

                return await _productoData.UpdateStockAsync(productoId, nuevoStock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar stock del producto con ID: {ProductoId}", productoId);
                throw;
            }
        }

        /// <summary>
        /// Valida que el nombre del producto sea único.
        /// </summary>
        public async Task<bool> ValidateUniqueNombreAsync(string nombre, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return false;

                return await _productoData.IsNombreUniqueAsync(nombre, excludeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar nombre único: {Nombre}", nombre);
                throw;
            }
        }
    }
}