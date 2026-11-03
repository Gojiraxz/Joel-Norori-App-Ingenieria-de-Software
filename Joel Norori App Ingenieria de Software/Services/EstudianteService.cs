
using Joel_Norori_App_Ingenieria_de_Software.DTOS;
using Joel_Norori_App_Ingenieria_de_Software.Models;
using Joel_Norori_App_Ingenieria_de_Software.Repositories;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Joel_Norori_App_Ingenieria_de_Software.Services
{
    public class EstudianteService(IEstudianteRepository repo, ICursoRepository repoCurso) : IEstudianteService
    {
        public async Task<(bool Succes, string? ErrorMessage)> CreateAsync(SaveEstudianteDto dto)
        {
            if (await repo.EmailExistsAsync(dto.Email))
                return (false, "El correo electrónico ya está en uso.");

            if (await repoCurso.GetByIdAsync(dto.CursoId) == null)
                return (false, "El curso especificado no existe.");

            var estudiante = new Estudiante
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                CursoId = dto.CursoId
            };

            await repo.CreateAsync(estudiante);

            return (true, null);
        }

        public async Task<(bool Succes, string? ErrorMessage)> DeleteAsync(int id)
        {
            var deleted = await repo.DeleteEstudiante(id);
            return deleted ? (true, null) : (false, "Estudiante no enontrado");
        }

        public async Task<IReadOnlyList<EstudianteTableDto>> GetAllAsync()
        {
            return await repo.GetAllAsync();
        }

        public async Task<SaveEstudianteDto?> GetByIdAsync(int id)
        {
            var estudiante = await repo.GetByIdAsync(id);

            if (estudiante == null)
                return null;

            return new SaveEstudianteDto
            {
                Id = estudiante.Id,
                Nombre = estudiante.Nombre,
                Apellido = estudiante.Apellido,
                Email = estudiante.Email,
                CursoId = estudiante.CursoId
            };
        }

        public Task<DataTableResponseDto<EstudianteTableDto>> GetPagedAsync(DatoaTablesRequestDto request)
        => repo.GetPagedAsync(request);

        public async Task<(bool Succes, string? ErrorMessage)> UpdateAsync(SaveEstudianteDto dto)
        {
            if(!dto.Id.HasValue)
                return (false, "El Id del estudiante es inválido.");

           var estudiante = await repo.GetByIdAsync(dto.Id.Value);
            if (estudiante == null)
                return (false, "El estudiante no existe.");
            if (await repo.EmailExistsAsync(dto.Email, dto.Id))
                return (false, "El correo electrónico ya está en uso.");
            if (await repoCurso.GetByIdAsync(dto.CursoId) == null)
                return (false, "El curso especificado no existe.");

            estudiante.Nombre = dto.Nombre;
            estudiante.Apellido = dto.Apellido;
            estudiante.Email = dto.Email;
            estudiante.CursoId = dto.CursoId;
            await repo.UpdateAsync(estudiante);
            return (true, null);
        }
    }
}



