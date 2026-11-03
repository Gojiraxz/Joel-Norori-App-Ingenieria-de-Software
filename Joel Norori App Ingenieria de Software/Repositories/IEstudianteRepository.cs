
using Joel_Norori_App_Ingenieria_de_Software.DTOS;
using Joel_Norori_App_Ingenieria_de_Software.Models;

namespace Joel_Norori_App_Ingenieria_de_Software.Repositories
{
    public interface IEstudianteRepository
    {
        Task<DataTableResponseDto<EstudianteTableDto>> GetPagedAsync(
            DatoaTablesRequestDto request);

        Task<List<EstudianteTableDto>> GetAllAsync();

        Task<Estudiante?> GetByIdAsync(int id);

        Task<bool> EmailExistsAsync(string email, int? excludeId = null);

        Task<Estudiante> CreateAsync(Estudiante estudiante);

        Task<bool> UpdateAsync(Estudiante estudiante);

        Task<bool> DeleteEstudiante(int id);
    }
}
