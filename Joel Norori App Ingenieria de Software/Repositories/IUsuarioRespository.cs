using Joel_Norori_App_Ingenieria_de_Software.Models;

namespace Joel_Norori_App_Ingenieria_de_Software.Repositories
{
    public interface IUsuarioRespository
    {
        Task<Usuario?> GetUsuarioByUserName(string userName);
        Task<Usuario?> GetUsuarioByEmail(string email);
        Task<Usuario?> GetUsuarioByIdWithPermissionAsync(int id);
        Task<Usuario?> CreateAsync(Usuario usario);
        bool ValidatePassword(Usuario usuario, string password);
        Task SaveAsync();
        Task<Usuario?> GetUsuarioByRefreshTokenAsync(string refreshToken);
        Task<bool> AssingPermissionToUsuarioAsync(int usuarioId, int permissionId, bool isGranted);
        Task<bool> RemovePermissionFromUsuarioAsync(int usuarioId, int permissionId);
        Task<bool> UpdateUsuarioRolAsync(int usuarioId , int roleId);
        Task<IReadOnlyList<Usuario>> GetAllUsuarioAsync();
    }
}
