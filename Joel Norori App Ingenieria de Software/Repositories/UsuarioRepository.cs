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
    public class UsuarioRepository(AppDBContext context) : IUsuarioRespository
    {
        private IQueryable<Usuario> UserWithPermission => 
            context.usuarios.Include(u => u.Role)
            .ThenInclude(r => r.rolePermisos)
            .ThenInclude(rp => rp.Permiso)
            .Include(p => p.usuarioPermisos)
            .ThenInclude(up => up.Permiso);

        public async Task<bool> AssingPermissionToUsuarioAsync(int usuarioId, int permissionId, bool isGranted)
        {
            var userPermission = await context.usuarioPermisos
                .FirstOrDefaultAsync(up => up.usuarioId == usuarioId && up.PermisoId == permissionId);

            if (userPermission != null)
            {
                userPermission.IsGranted = isGranted;
            }
            else
            {
                context.usuarioPermisos.Add(new UsuarioPermiso
                {
                    usuarioId = usuarioId,
                    PermisoId = permissionId,
                    IsGranted = isGranted
                });


            }
            await context.SaveChangesAsync();
            return true;

        }

        public async Task<Usuario?> CreateAsync(Usuario usario)
        {
            var entry = await context.usuarios.AddAsync(usario);
            return entry.Entity;
        }

        public async Task<IReadOnlyList<Usuario>> GetAllUsuarioAsync()
        {
            return await context.usuarios
                .Include(u => u.Role)
                .OrderBy(u => u.NombreUsuario)
                .ToListAsync();
        }

        public async Task<Usuario?> GetUsuarioByEmail(string email)
        {
            return await context.usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario?> GetUsuarioByIdWithPermissionAsync(int id)
        {
            return await UserWithPermission.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> GetUsuarioByRefreshTokenAsync(string refreshToken)
        {
            return await context.usuarios.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<Usuario?> GetUsuarioByUserName(string userName)
        {
            return await UserWithPermission.FirstOrDefaultAsync(u => u.NombreUsuario == userName);
        }

        public async Task<bool> RemovePermissionFromUsuarioAsync(int usuarioId, int permissionId)
        {
            var userPermission = context.usuarioPermisos
                .FirstOrDefault(up => up.usuarioId == usuarioId && up.PermisoId == permissionId);

            if (userPermission != null)
            {
                context.usuarioPermisos.Remove(userPermission);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<bool> UpdateUsuarioRolAsync(int usuarioId, int roleId)
        {
            var user = await context.usuarios.FindAsync(usuarioId);
            if (user == null) return false;

            var roleExists = await context.roles.AnyAsync(r => r.Id == roleId);
            if (!roleExists) return false;
            user.RolId = roleId;
            await context.SaveChangesAsync();

            return true;
        }

        public bool ValidatePassword(Usuario usuario, string password)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, usuario.Password);
        }
    }
}
