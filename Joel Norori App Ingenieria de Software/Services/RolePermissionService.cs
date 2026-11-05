
using Joel_Norori_App_Ingenieria_de_Software.DTOS;
using Joel_Norori_App_Ingenieria_de_Software.Helpers;
using Joel_Norori_App_Ingenieria_de_Software.Models;
using Joel_Norori_App_Ingenieria_de_Software.Repositories;

namespace Joel_Norori_App_Ingenieria_de_Software.Services
{
    public class RolePermissionService( IRoleRepository roleRepo,IPermissionRepository permissionRepository, IUsuarioRespository userRepo) : IRolePermissionService
    {
        public async Task<(bool Success, string? ErrorMessage)>
            AssignPermissionToRoleAsync(int roleId, int PermissionId)
        {
            var role = await roleRepo.GetByIdWithPermissionAsync(roleId);

            if (role == null)
                return (false, "Rol no encontrado");

            var permission = await permissionRepository.GetByIdAsync(PermissionId);

            if (permission == null)
                return (false, "Permiso no encontrado");

            var assigned = await roleRepo.AssingPermissionToRoleAsync(
                roleId, PermissionId);

            if (!assigned)
                return (false, "No se pudo asignar el permiso al rol");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)>
            AssignPermissionToUserAsync(int userId, AssingUserPermissionDto dto)
        {
            var user = await userRepo.GetUsuarioByIdWithPermissionAsync(userId);

            if (user == null)
                return (false, "Usuario no encontrado");

            var permission = await permissionRepository.GetByIdAsync(dto.PermissionId);

            if (permission == null)
                return (false, "Permiso no encontrado");

            var assigned = await userRepo.AssingPermissionToUsuarioAsync(
                userId, dto.PermissionId, dto.IsGranted);

            if (!assigned)
                return (false, "No se pudo asignar el permiso al usuario");

            return (true, null);
        }

        public async Task<(bool Success, PermissionDto? Result, string? ErrorMessage)>
            CreatePermissionAsync(SavePermissionDto dto)
        {
            var name = dto.Name.Trim().ToLowerInvariant();

            var existing = await permissionRepository.GetByNameAsync(name);

            if (existing != null)
                return (false, null, "Ya existe un permiso con ese nombre");

            var permission = await permissionRepository.CreateAsync(
                name, dto.Descripcion);

            return (true, new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.DescriptionS
            }, null);
        }

        public async Task<(bool Success, RoleWithPermissionDto? Result, string? ErrorMessage)> CreateRoleAsync(CreateRolDto dto)
        {
            var existing = await roleRepo.GetByNameAsync(dto.Name.Trim());

            if (existing != null)
                return (false, null, "Ya existe un rol con ese nombre");

            var role = await roleRepo.CreateAsync(dto.Name);

            return (true, new RoleWithPermissionDto
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = []
            }, null);
        }

        public async Task<(bool Success, string? ErrorMessage)>
            DeletePermissionAsync(int id)
        {
            var permission = await permissionRepository.GetByIdAsync(id);

            if (permission == null)
                return (false, "Permiso no encontrado");

            var assigned = await permissionRepository.IsAssingnedToUsuarioAsync(id);

            if (assigned)
                return (false, "El permiso está asignado a uno o más usuarios");

            var deleted = await permissionRepository.DeleteAsync(id);

            if (!deleted)
                return (false, "No se pudo eliminar el permiso");

            return (true, null);
        }

        public async Task<IReadOnlyList<PermissionDto>>
            GetAllPermissionsAsync()
        {
            var permissions = await permissionRepository.GetAllAsync();

            return permissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.DescriptionS
            }).ToList();
        }

        public async Task<IReadOnlyList<RoleWithPermissionDto>>
            GetAllRolesAsync()
        {
            var roles = await roleRepo.GetAllWithPermissionAsync();

            return roles.Select(MapRole).ToList();
        }

        public async Task<IReadOnlyList<UserListItemDto>>
            GetAllUserAsync()
        {
            var users = await userRepo.GetAllUsuarioAsync();

            return users.Select(u => new UserListItemDto
            {
                Id = u.Id,
                Username = u.NombreUsuario,
                Email = u.Email,
                RoleName = u.Role?.Name ?? string.Empty
            }).ToList();
        }

        public async Task<PermissionDto?> GetPermissionByIdAsync(int id)
        {
            var permission = await permissionRepository.GetByIdAsync(id);

            if (permission == null)
                return null;

            return new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.DescriptionS
            };
        }

        public async Task<DataTableResponseDto<PermissionDto>>
            GetPermissionPagedAsync(DatoaTablesRequestDto request)
        {
            return await permissionRepository.GetPagedAsync(request);
        }

        public async Task<RoleWithPermissionDto?> GetRoleByIdAsync(int roleId)
        {
            var role = await roleRepo.GetByIdWithPermissionAsync(roleId);

            if (role == null)
                return null;

            return MapRole(role);
        }

        public async Task<UserPermissionDto?> GetUserPermissionAsync(int userId)
        {
            var user = await userRepo.GetUsuarioByIdWithPermissionAsync(userId);

            if (user == null)
                return null;

            return new UserPermissionDto
            {
                UserId = user.Id,
                Username = user.NombreUsuario,
                Email = user.Email,
                RolId = user.RolId,
                RoleName = user.Role?.Name ?? string.Empty,

                EfectivePermission = EffectivePermission.FromUser(user).ToList(),

                DirectPermission = user.usuarioPermisos.Select(up =>
                    new UserPermissionOverrideDto
                    {
                        PermissionId = up.PermisoId,
                        PermissionName = up.Permiso.Name,
                        IsGranted = up.IsGranted
                    }).ToList()
            };
        }

        public async Task<(bool Success, string? ErrorMessage)>
            RemovePermissionFromRoleAsync(int roleId, int PermissionId)
        {
            var removed = await roleRepo.RemovePermissionFromRoleAsync(
                roleId, PermissionId);

            if (!removed)
                return (false, "El rol no tiene asignado este permiso");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)>
            RemovePermissionFromUserAsync(int userId, int permissionId)
        {
            var removed = await userRepo.RemovePermissionFromUsuarioAsync(
                userId, permissionId);

            if (!removed)
                return (false, "El usuario no tiene asignado este permiso");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)>
            UpdatePermissionAsync(SavePermissionDto dto)
        {
            if (dto.Id == null)
                return (false, "El ID del permiso es requerido");

            var permission = await permissionRepository.GetByIdAsync(dto.Id.Value);

            if (permission == null)
                return (false, "Permiso no encontrado");

            var name = dto.Name.Trim().ToLowerInvariant();

            var duplicate = await permissionRepository.GetByNameAsync(name);

            if (duplicate != null && duplicate.Id != dto.Id.Value)
                return (false, "Ya existe un permiso con ese nombre");

            permission.Name = name;
            permission.DescriptionS = dto.Descripcion;

            var updated = await permissionRepository.UpdateAsync(permission);

            if (!updated)
                return (false, "No se pudo actualizar el permiso");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)>
            UpdateUserRoleAsync(int userId, int roleId)
        {
            var updated = await userRepo.UpdateUsuarioRolAsync(
                userId, roleId);

            if (!updated)
                return (false, "No se pudo actualizar el rol del usuario");

            return (true, null);
        }

        private static RoleWithPermissionDto MapRole(Rol role)
        {
            return new RoleWithPermissionDto
            {
                Id = role.Id,
                Name = role.Name,

                Permissions = role.rolePermisos.Select(p =>
                    new PermissionDto
                    {
                        Id = p.Permiso.Id,
                        Name = p.Permiso.Name,
                        Description = p.Permiso.DescriptionS
                    })
                    .OrderBy(p => p.Name)
                    .ToList()
            };
        }
    }
}
