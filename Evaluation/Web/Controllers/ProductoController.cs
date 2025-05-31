using Business.Implements;
using Business.Interfaces;
using Entity.Dto.ProductoDTO;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de productos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IBaseBusiness<Producto, ProductoDTO> _productoBusiness;

        public ProductosController(IBaseBusiness<Producto, ProductoDTO> productoBusiness)
        {
            _productoBusiness = productoBusiness;
        }

        /// <summary>
        /// Obtiene todos los productos
        /// </summary>
        /// <returns>Lista de productos</returns>
        [HttpGet]
        public async Task<ActionResult<List<ProductoDTO>>> GetProductos()
        {
            try
            {
                var productos = await _productoBusiness.GetAllAsync();
                return Ok(productos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Producto encontrado</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDTO>> GetProducto(int id)
        {
            try
            {
                var producto = await _productoBusiness.GetByIdAsync(id);

                if (producto == null)
                    return NotFound(new { mensaje = $"Producto con ID {id} no encontrado" });

                return Ok(producto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Busca productos por nombre
        /// </summary>
        /// <param name="nombre">Nombre del producto a buscar</param>
        /// <returns>Lista de productos que coinciden</returns>
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> BuscarProductosPorNombre([FromQuery] string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return BadRequest(new { mensaje = "El parámetro 'nombre' es requerido" });

                var productos = await ((ProductoBusiness)_productoBusiness).GetProductosByNombreAsync(nombre);
                return Ok(productos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene productos con stock disponible
        /// </summary>
        /// <param name="stockMinimo">Stock mínimo requerido (por defecto 1)</param>
        /// <returns>Lista de productos con stock</returns>
        [HttpGet("stock")]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> GetProductosConStock([FromQuery] int stockMinimo = 1)
        {
            try
            {
                var productos = await ((ProductoBusiness)_productoBusiness).GetProductosWithStockAsync(stockMinimo);
                return Ok(productos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        /// <param name="productoDto">Datos del nuevo producto</param>
        /// <returns>Producto creado</returns>
        [HttpPost]
        public async Task<ActionResult<ProductoDTO>> CreateProducto([FromBody] ProductoDTO productoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var producto = await _productoBusiness.CreateAsync(productoDto);
                return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="productoDto">Datos actualizados</param>
        /// <returns>Producto actualizado</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductoDTO>> UpdateProducto(int id, [FromBody] ProductoDTO productoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != productoDto.Id)
                {
                    return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID del producto" });
                }

                var producto = await _productoBusiness.UpdateAsync(productoDto);
                return Ok(producto);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un producto
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="updateDto">Campos a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdatePartialProducto(int id, [FromBody] UpdateProductoDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != updateDto.Id)
                {
                    return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID del producto" });
                }

                var resultado = await ((ProductoBusiness)_productoBusiness).UpdatePartialProductoAsync(updateDto);

                if (resultado)
                    return Ok(new { mensaje = "Producto actualizado parcialmente con éxito" });
                else
                    return BadRequest(new { mensaje = "No se pudo actualizar el producto" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Elimina permanentemente un producto
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProducto(int id)
        {
            try
            {
                var resultado = await _productoBusiness.DeleteAsync(id);

                if (resultado)
                    return Ok(new { mensaje = "Producto eliminado correctamente", eliminado = true });
                else
                    return BadRequest(new { mensaje = "No se pudo eliminar el producto", eliminado = false });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Realiza eliminación lógica de un producto (activar/desactivar)
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="deleteLogicDto">Datos para la eliminación lógica</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> DeleteLogicProducto(int id, [FromBody] DeleteLogicProductoDto deleteLogicDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != deleteLogicDto.Id)
                {
                    return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID del producto" });
                }

                var resultado = await ((ProductoBusiness)_productoBusiness).DeleteLogicProductoAsync(deleteLogicDto);

                if (resultado)
                {
                    string accion = deleteLogicDto.Status ? "activado" : "desactivado";
                    return Ok(new { mensaje = $"Producto {accion} correctamente", resultado = true });
                }
                else
                {
                    return BadRequest(new { mensaje = "No se pudo cambiar el estado del producto", resultado = false });
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza el stock de un producto
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="nuevoStock">Nuevo valor de stock</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPatch("{id}/stock")]
        public async Task<ActionResult> UpdateStock(int id, [FromBody] int nuevoStock)
        {
            try
            {
                var resultado = await ((ProductoBusiness)_productoBusiness).UpdateStockAsync(id, nuevoStock);

                if (resultado)
                    return Ok(new { mensaje = "Stock actualizado correctamente", nuevoStock = nuevoStock });
                else
                    return BadRequest(new { mensaje = "No se pudo actualizar el stock" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Valida si un nombre de producto es único
        /// </summary>
        /// <param name="nombre">Nombre a validar</param>
        /// <param name="excludeId">ID a excluir de la validación (opcional)</param>
        /// <returns>True si es único, false si ya existe</returns>
        [HttpGet("validate-nombre")]
        public async Task<ActionResult<bool>> ValidateUniqueNombre([FromQuery] string nombre, [FromQuery] int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return BadRequest(new { mensaje = "El parámetro 'nombre' es requerido" });

                var esUnico = await ((ProductoBusiness)_productoBusiness).ValidateUniqueNombreAsync(nombre, excludeId);
                return Ok(new { esUnico = esUnico, mensaje = esUnico ? "Nombre disponible" : "Nombre ya existe" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }
    }
}