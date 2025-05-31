using Business.Implements;
using Business.Interfaces;
using Entity.Dto.ClienteDTO;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de clientes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IBaseBusiness<Cliente, ClienteDTO> _clienteBusiness;

        public ClientesController(IBaseBusiness<Cliente, ClienteDTO> clienteBusiness)
        {
            _clienteBusiness = clienteBusiness;
        }

        /// <summary>
        /// Obtiene todos los clientes
        /// </summary>
        /// <returns>Lista de clientes</returns>
        [HttpGet]
        public async Task<ActionResult<List<ClienteDTO>>> GetClientes()
        {
            try
            {
                var clientes = await _clienteBusiness.GetAllAsync();
                return Ok(clientes);
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
        /// Obtiene un cliente por su ID
        /// </summary>
        /// <param name="id">ID del cliente</param>
        /// <returns>Cliente encontrado</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDTO>> GetCliente(int id)
        {
            try
            {
                var cliente = await _clienteBusiness.GetByIdAsync(id);

                if (cliente == null)
                    return NotFound(new { mensaje = $"Cliente con ID {id} no encontrado" });

                return Ok(cliente);
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
        /// Busca clientes por email
        /// </summary>
        /// <param name="email">Email del cliente a buscar</param>
        /// <returns>Lista de clientes que coinciden</returns>
        [HttpGet("buscar/email")]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> BuscarClientesPorEmail([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return BadRequest(new { mensaje = "El parámetro 'email' es requerido" });

                var clientes = await ((ClienteBusiness)_clienteBusiness).GetClientesByEmailAsync(email);
                return Ok(clientes);
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
        /// Busca clientes por nombre o apellido
        /// </summary>
        /// <param name="nombre">Nombre del cliente a buscar</param>
        /// <returns>Lista de clientes que coinciden</returns>
        [HttpGet("buscar/nombre")]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> BuscarClientesPorNombre([FromQuery] string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return BadRequest(new { mensaje = "El parámetro 'nombre' es requerido" });

                var clientes = await ((ClienteBusiness)_clienteBusiness).GetClientesByNameAsync(nombre);
                return Ok(clientes);
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
        /// Crea un nuevo cliente
        /// </summary>
        /// <param name="clienteDto">Datos del nuevo cliente</param>
        /// <returns>Cliente creado</returns>
        [HttpPost]
        public async Task<ActionResult<ClienteDTO>> CreateCliente([FromBody] ClienteDTO clienteDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cliente = await _clienteBusiness.CreateAsync(clienteDto);
                return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
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
        /// Actualiza un cliente existente
        /// </summary>
        /// <param name="id">ID del cliente</param>
        /// <param name="clienteDto">Datos actualizados</param>
        /// <returns>Cliente actualizado</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ClienteDTO>> UpdateCliente(int id, [FromBody] ClienteDTO clienteDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != clienteDto.Id)
                {
                    return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID del cliente" });
                }

                var cliente = await _clienteBusiness.UpdateAsync(clienteDto);
                return Ok(cliente);
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
        /// Actualiza parcialmente un cliente
        /// </summary>
        /// <param name="id">ID del cliente</param>
        /// <param name="updateDto">Campos a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdatePartialCliente(int id, [FromBody] UpdateClienteDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != updateDto.Id)
                {
                    return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID del cliente" });
                }

                var resultado = await ((ClienteBusiness)_clienteBusiness).UpdatePartialClienteAsync(updateDto);

                if (resultado)
                    return Ok(new { mensaje = "Cliente actualizado parcialmente con éxito" });
                else
                    return BadRequest(new { mensaje = "No se pudo actualizar el cliente" });
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
        /// Elimina permanentemente un cliente
        /// </summary>
        /// <param name="id">ID del cliente</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCliente(int id)
        {
            try
            {
                var resultado = await _clienteBusiness.DeleteAsync(id);

                if (resultado)
                    return Ok(new { mensaje = "Cliente eliminado correctamente", eliminado = true });
                else
                    return BadRequest(new { mensaje = "No se pudo eliminar el cliente", eliminado = false });
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
        /// Realiza eliminación lógica de un cliente (activar/desactivar)
        /// </summary>
        /// <param name="id">ID del cliente</param>
        /// <param name="deleteLogicDto">Datos para la eliminación lógica</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> DeleteLogicCliente(int id, [FromBody] DeleteLogicClienteDto deleteLogicDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != deleteLogicDto.Id)
                {
                    return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID del cliente" });
                }

                var resultado = await ((ClienteBusiness)_clienteBusiness).DeleteLogicClienteAsync(deleteLogicDto);

                if (resultado)
                {
                    string accion = deleteLogicDto.Status ? "activado" : "desactivado";
                    return Ok(new { mensaje = $"Cliente {accion} correctamente", resultado = true });
                }
                else
                {
                    return BadRequest(new { mensaje = "No se pudo cambiar el estado del cliente", resultado = false });
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
        /// Valida si un email de cliente es único
        /// </summary>
        /// <param name="email">Email a validar</param>
        /// <param name="excludeId">ID a excluir de la validación (opcional)</param>
        /// <returns>True si es único, false si ya existe</returns>
        [HttpGet("validate-email")]
        public async Task<ActionResult<bool>> ValidateUniqueEmail([FromQuery] string email, [FromQuery] int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return BadRequest(new { mensaje = "El parámetro 'email' es requerido" });

                var esUnico = await ((ClienteBusiness)_clienteBusiness).ValidateUniqueEmailAsync(email, excludeId);
                return Ok(new { esUnico = esUnico, mensaje = esUnico ? "Email disponible" : "Email ya existe" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }
    }
}