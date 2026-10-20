using Joel_Norori_App_Ingenieria_de_Software.Models;

namespace Joel_Norori_App_Ingenieria_de_Software.Repositories
{
    public interface ICursoRepository
    {
        Task<DataTableResponseDto<CursoTableDto>> Get ICursoRepository(CursoTableRequestDto request);
        Task<IReadOnlyList<Curso>> GetAllAsync();
        Task<Curso?> GetByIdAsync(int id);
        Task<bool> NombreExisteAsync(string nombre);
        Task<Curso> CreateAsync(Curso curso);
        Task<bool> UpdateAsync(Curso curso);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasAnyEstudianteAsync(int Id);
    }
}
