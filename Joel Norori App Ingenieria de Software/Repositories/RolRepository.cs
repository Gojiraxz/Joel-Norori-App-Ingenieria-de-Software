using Joel_Norori_App_Ingenieria_de_Software.Data;
using Joel_Norori_App_Ingenieria_de_Software.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Repositories
{
    public class RolRepository(AppDBContext context) : IRoleRepository
    {
        public async Task<bool> AssingPermissionToRoleAsync(int roleId, int permissionId)
        {
            var exists = await context.rolePermisos
                .AnyAsync(rp => rp.rolId == roleId && rp.permisoId == permissionId);

            if (exists) return false;
            context.rolePermisos.Add(new RolePermiso { rolId = roleId, permisoId = permissionId });
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Rol> CreateAsync(string name)
        {
            var role = new Rol { Name = name.Trim() };
            var entry = await context.roles.AddAsync(role);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<IReadOnlyList<Rol>> GetAllWithPermissionAsync()
        {
            return await context.roles.Include(r => r.rolePermisos)
                .ThenInclude(rp => rp.Permiso)
                .ToListAsync();
                
        }

        public async Task<Rol?> GetByIdWithPermissionAsync(int id)
        {
            return await context.roles
                .Include(r => r.rolePermisos)
                .ThenInclude(rp => rp.Permiso)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public Task<Rol?> GetByNameAsync(string name)
        {
            return context.roles.FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
          var rolePermission = context.rolePermisos
          .FirstOrDefault(rp => rp.rolId == roleId 
          && rp.permisoId == permissionId);

          if (rolePermission == null) return false;
          context.rolePermisos.Remove(rolePermission);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
