using Data.Implements.BaseData;
using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Implements.ClienteData
{
    public class ClienteData : BaseModelData<Cliente>, IClienteData
    {
        public ClienteData(ApplicationDbContext context) : base(context)
        {
        }

        // Método específico: Obtener cliente por email
        public async Task<Cliente> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
        }

        // Método específico: Buscar clientes por nombre
        public async Task<List<Cliente>> GetByNombreAsync(string nombre)
        {
            return await _dbSet
                .Where(c => c.Nombre.ToLower().Contains(nombre.ToLower()) ||
                           c.Apellido.ToLower().Contains(nombre.ToLower()))
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        // Método específico: Verificar si existe un email
        public async Task<bool> ExistsEmailAsync(string email)
        {
            return await _dbSet
                .AnyAsync(c => c.Email.ToLower() == email.ToLower());
        }

        // Método específico: Obtener clientes recientes
        public async Task<List<Cliente>> GetClientesRecentesAsync(int dias = 30)
        {
            var fechaLimite = DateTime.Now.AddDays(-dias);
            return await _dbSet
                .Where(c => c.FechaCreacion >= fechaLimite)
                .OrderByDescending(c => c.FechaCreacion)
                .ToListAsync();
        }

        // Override para validaciones específicas en Create
        public override async Task<Cliente> CreateAsync(Cliente cliente)
        {
            // Validar que el email no exista
            if (await ExistsEmailAsync(cliente.Email))
            {
                throw new InvalidOperationException($"Ya existe un cliente con el email: {cliente.Email}");
            }

            return await base.CreateAsync(cliente);
        }

        // Override para validaciones específicas en Update
        public override async Task<Cliente> UpdateAsync(Cliente cliente)
        {
            // Verificar que no exista otro cliente con el mismo email
            var existingClient = await _dbSet
                .FirstOrDefaultAsync(c => c.Email.ToLower() == cliente.Email.ToLower() && c.Id != cliente.Id);

            if (existingClient != null)
            {
                throw new InvalidOperationException($"Ya existe otro cliente con el email: {cliente.Email}");
            }

            return await base.UpdateAsync(cliente);
        }
    }
}