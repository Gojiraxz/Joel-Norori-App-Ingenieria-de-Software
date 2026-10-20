namespace Joel_Norori_App_Ingenieria_de_Software.Repositories
{
    public interface IEstudianteRepository
    {
        Task<List<DataTableReponseDto<EstudianteTableDto>> GetPagedAsync(DataTableRequestDto request);
        Task<Estudiante> GetByIdAsync(int id);
        Task<bool> EmalilExitsAsync(string email,int? excludeId=null);
        Task<bool> CreateAsync(Estudiante estudiante);
        Task<bool> UpdateAsync(Estudiante estudiante);
        Task<bool> DeleteEstudiante(int id);
    }
}
