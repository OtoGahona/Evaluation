using Entity.Dto.ClienteDTO;
using Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IClienteData
    {
        Task<List<Cliente>> GetAllAsync();
        Task<Cliente> GetByIdAsync(int id);
        Task<Cliente> CreateAsync(Cliente cliente);
        Task<Cliente> UpdateAsync(Cliente cliente);
        Task<bool> DeleteAsync(int id);

        // Métodos específicos para Cliente
        Task<Cliente> GetByEmailAsync(string email);
        Task<List<Cliente>> GetByNombreAsync(string nombre);
        Task<bool> ExistsEmailAsync(string email);
        Task<List<Cliente>> GetClientesRecentesAsync(int dias = 30);
        Task<bool> UpdateClienteDto(UpdateClienteDto dto);
        Task GetByNameAsync(string searchTerm);
        Task<bool> ActiveAsync(int id, bool status);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId);
    }
}