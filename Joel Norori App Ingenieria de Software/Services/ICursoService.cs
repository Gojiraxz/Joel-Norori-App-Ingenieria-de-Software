using Joel_Norori_App_Ingenieria_de_Software.DTOS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Services
{
    public interface ICursoService
    {
        Task<DataTableResponseDto<CursoTableDto>> GetPagedAsync(DatoaTablesRequestDto request);
        Task<IReadOnlyList<CursoTableDto>> GetAllAsync();
        Task<SaveCursoDto?> GetByIdAsync(int id);
        Task<(bool Succes, string? ErrorMessage)> CreateAsync(SaveCursoDto dto);
        Task<(bool Succes, string? ErrorMessage)> UpdateAync(SaveCursoDto dto);
        Task<(bool Succes, string? ErrorMessage)> DeleteAsync(int id);
    }
}
