using Joel_Norori_App_Ingenieria_de_Software.DTOS;
using Joel_Norori_App_Ingenieria_de_Software.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Repositories
{
    public interface IPermissionRepository
    {
        Task<DataTableResponseDto<PermissionDto>> GetPagedAsync(DatoaTablesRequestDto request);
        Task<IReadOnlyList<Permiso>> GetAllAsync();
        Task<Permiso?> GetByIdAsync(int id);
        Task<Permiso?> GetByNameAsync(string name);
        Task<Permiso> CreateAsync(string name, string? DescriptionS);
        Task<bool> UpdateAsync(Permiso permiso);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsAssingnedToUsuarioAsync(int Id);

    }
}
