using Joel_Norori_App_Ingenieria_de_Software.Models;

namespace Joel_Norori_App_Ingenieria_de_Software.Repositories
{
    public interface IRoleRepository
    {
        Task<IReadOnlyList<Rol>> GetAllWithPermissionAsync();
        Task<Rol?> GetByIdWithPermissionAsync(int id);
        Task<Rol?> GetByNameAsync(string name);
        Task<Rol> CreateAsync(string name);
        Task<bool> AssingPermissionToRoleAsync(int roleId, int permissionId);
        Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId);
    }
}
