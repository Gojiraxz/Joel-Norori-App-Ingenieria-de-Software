using Joel_Norori_App_Ingenieria_de_Software.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Services
{
    public interface IRolePermissionService
    {
        Task<IReadOnlyList<PermissionDto>> GetAllPermissionsAsync();
        Task<IReadOnlyList<RoleWithPermissionDto>> GetAllRolesAsync();
        Task<RoleWithPermissionDto> GetRoleByIdAsync(int roleId);
        Task<(bool Success, string? ErrorMessage)> AssignPermissionToRoleAsync(int roleId, int PermissionId);
        Task<(bool Success, string? ErrorMessage)> RemovePermissionFromRoleAsync(int roleId, int PermissionId);
        Task<UserPermissionDto> GetUserPermissionAsync(int userId);
        Task<(bool Success, string? ErrorMessage)> AssignPermissionToUserAsync(int userId, AssingUserPermissionDto dto);
        Task<(bool Success, string? ErrorMessage)> RemovePermissionFromUserAsync(int userId, int permissionId);
        Task<(bool Success, string? ErrorMessage)> UpdateUserRoleAsync(int userId, int roleId);
        Task<DataTableResponseDto<PermissionDto>> GetPermissionPagedAsync(DatoaTablesRequestDto request);
        Task<PermissionDto?> GetPermissionByIdAsync(int id);
        Task<(bool Success, PermissionDto? Result, string? ErrorMessage)> CreatePermissionAsync(SavePermissionDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdatePermissionAsync(SavePermissionDto dto);
        Task<(bool Success, string? ErrorMessage)> DeletePermissionAsync(int id);
        Task<(bool Succes, RoleWithPermissionDto? Result, string? ErrorMessage)> CreateRoleAsync(CreateRolDto dto);
        Task<IReadOnlyList<UserListItemDto>> GetAllUserAsync();


    }
}
