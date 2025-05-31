using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dto.ClienteDTO;
using Entity.Dtos.Base;
using Entity.Model;
using Entity.Model.Base;
using Microsoft.Extensions.Logging;

namespace Business.Implements
{
    /// <summary>
    /// Contiene la lógica de negocio específica para la entidad Cliente.
    /// Implementa la interfaz base IBaseBusiness.
    /// </summary>
    public class ClienteBusiness : IBaseBusiness<Cliente, ClienteDTO>
    {
        private readonly IClienteData _clienteData;
        private readonly IMapper _mapper;
        private readonly ILogger<ClienteBusiness> _logger;

        public ClienteBusiness(IClienteData clienteData, IMapper mapper, ILogger<ClienteBusiness> logger)
        {
            _clienteData = clienteData;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las entidades desde la base de datos.
        /// </summary>
        /// <returns>Una colección de objetos de tipo ClienteDTO.</returns>
        public async Task<List<ClienteDTO>> GetAllAsync()
        {
            try
            {
                var clientes = await _clienteData.GetAllAsync();
                return _mapper.Map<List<ClienteDTO>>(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los clientes");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un DTO específico por su ID.
        /// </summary>
        /// <param name="id">Identificador único del cliente.</param>
        /// <returns>Un objeto ClienteDTO si se encuentra; de lo contrario, null.</returns>
        public async Task<ClienteDTO> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("ID inválido.", nameof(id));

                var cliente = await _clienteData.GetByIdAsync(id);
                return _mapper.Map<ClienteDTO>(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cliente con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo cliente a partir de un DTO.
        /// </summary>
        /// <param name="dto">Objeto de transferencia con los datos del cliente.</param>
        /// <returns>El DTO del cliente creado.</returns>
        public async Task<ClienteDTO> CreateAsync(ClienteDTO dto)
        {
            try
            {
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto));

                // Validar email único
                if (!await ValidateUniqueEmailAsync(dto.Email))
                    throw new InvalidOperationException("El email ya existe en el sistema.");

                var cliente = _mapper.Map<Cliente>(dto);
                cliente.FechaCreacion = DateTime.UtcNow;
                cliente.FechaModificacion = DateTime.UtcNow;

                var clienteCreado = await _clienteData.CreateAsync(cliente);
                return _mapper.Map<ClienteDTO>(clienteCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cliente");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un registro existente a partir de un DTO.
        /// </summary>
        /// <param name="dto">Objeto de transferencia con los datos actualizados.</param>
        /// <returns>El DTO actualizado o una excepción si falla.</returns>
        public async Task<ClienteDTO> UpdateAsync(ClienteDTO dto)
        {
            try
            {
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto));

                if (dto.Id <= 0)
                    throw new ArgumentException("ID inválido.", nameof(dto.Id));

                // Verificar que el cliente existe
                var clienteExistente = await _clienteData.GetByIdAsync(dto.Id);
                if (clienteExistente == null)
                    throw new InvalidOperationException($"Cliente con ID {dto.Id} no encontrado.");

                // Validar email único (excluyendo el cliente actual)
                if (!await ValidateUniqueEmailAsync(dto.Email, dto.Id))
                    throw new InvalidOperationException("El email ya existe en el sistema.");

                var cliente = _mapper.Map<Cliente>(dto);
                cliente.FechaModificacion = DateTime.UtcNow;
                cliente.FechaCreacion = clienteExistente.FechaCreacion; // Preservar fecha de creación

                var clienteActualizado = await _clienteData.UpdateAsync(cliente);
                return _mapper.Map<ClienteDTO>(clienteActualizado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cliente con ID: {Id}", dto?.Id ?? 0);
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

                // Verificar que el cliente existe
                var cliente = await _clienteData.GetByIdAsync(id);
                if (cliente == null)
                    throw new InvalidOperationException($"Cliente con ID {id} no encontrado.");

                return await _clienteData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cliente con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Actualiza parcialmente un cliente.
        /// </summary>
        public async Task<bool> UpdatePartialClienteAsync(UpdateClienteDto dto)
        {
            try
            {
                if (dto.Id <= 0)
                    throw new ArgumentException("ID inválido.");

                // Verificar que el cliente existe
                var clienteExistente = await _clienteData.GetByIdAsync(dto.Id);
                if (clienteExistente == null)
                    throw new InvalidOperationException($"Cliente con ID {dto.Id} no encontrado.");

                return await _clienteData.UpdateClienteDto(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente cliente con ID: {Id}", dto?.Id ?? 0);
                throw;
            }
        }

        /// <summary>
        /// Activa o desactiva un cliente (eliminación lógica).
        /// </summary>
        public async Task<bool> DeleteLogicClienteAsync(DeleteLogicClienteDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0)
                    throw new ArgumentException("El ID del cliente es inválido.");

                var exists = await _clienteData.GetByIdAsync(dto.Id);
                if (exists == null)
                    throw new InvalidOperationException($"Cliente con ID {dto.Id} no encontrado.");

                return await _clienteData.ActiveAsync(dto.Id, dto.Status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar eliminación lógica del cliente con ID: {Id}", dto?.Id ?? 0);
                throw;
            }
        }

        /// <summary>
        /// Busca clientes por email.
        /// </summary>
        public async Task<IEnumerable<ClienteDTO>> GetClientesByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("El email no puede estar vacío.");

                var clientes = await _clienteData.GetByEmailAsync(email);
                return _mapper.Map<IEnumerable<ClienteDTO>>(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar clientes por email: {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Busca clientes por nombre o apellido.
        /// </summary>
        public async Task<IEnumerable<ClienteDTO>> GetClientesByNameAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    throw new ArgumentException("El término de búsqueda no puede estar vacío.");

                var clientes = await _clienteData.GetByNombreAsync(searchTerm); // Correct method call
                return _mapper.Map<IEnumerable<ClienteDTO>>(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar clientes por nombre: {SearchTerm}", searchTerm);
                throw;
            }
        }

        /// <summary>
        /// Valida que el email del cliente sea único.
        /// </summary>
        public async Task<bool> ValidateUniqueEmailAsync(string email, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                return await _clienteData.IsEmailUniqueAsync(email, excludeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar email único: {Email}", email);
                throw;
            }
        }
    }
}