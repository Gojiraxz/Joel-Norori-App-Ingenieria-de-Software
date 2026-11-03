using Joel_Norori_App_Ingenieria_de_Software.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Services
{
    public interface IEstudianteService
    {
        Task<DataTableResponseDto<EstudianteTableDto>> GetPagedAsync(DatoaTablesRequestDto request);
        Task<IReadOnlyList<EstudianteTableDto>> GetAllAsync();
        Task<SaveEstudianteDto?> GetByIdAsync(int id);
        Task<(bool Succes, string? ErrorMessage)> CreateAsync(SaveEstudianteDto dto);
        Task<(bool Succes, string? ErrorMessage)> UpdateAsync(SaveEstudianteDto dto);
        Task<(bool Succes, string? ErrorMessage)> DeleteAsync(int id);

    }
}
