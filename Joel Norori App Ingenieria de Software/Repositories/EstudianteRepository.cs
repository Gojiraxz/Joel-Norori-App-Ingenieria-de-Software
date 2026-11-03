
using Joel_Norori_App_Ingenieria_de_Software.Data;
using Joel_Norori_App_Ingenieria_de_Software.DTOS;
using Joel_Norori_App_Ingenieria_de_Software.Models;
using Microsoft.EntityFrameworkCore;

namespace Joel_Norori_App_Ingenieria_de_Software.Repositories
{
    public class EstudianteRepository(AppDBContext context) : IEstudianteRepository
    {
        public async Task<Estudiante> CreateAsync(Estudiante estudiante)
        {
            var entry = await context.estudiantes.AddAsync(estudiante);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteEstudiante(int id)
        {
            var estudiante = await context.estudiantes.FindAsync(id);

            if (estudiante == null)
                return false;

            context.estudiantes.Remove(estudiante);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            return await context.estudiantes.AnyAsync(e =>
                e.Email == email &&
                (!excludeId.HasValue || e.Id != excludeId.Value));
        }

        public async Task<List<EstudianteTableDto>> GetAllAsync()
        {
            return await context.estudiantes
                .Include(e => e.Curso)
                .Select(e => new EstudianteTableDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    Email = e.Email,
                    CursoId = e.CursoId,
                    CursoNombre = e.Curso.Nombre
                })
                .ToListAsync();
        }

        public async Task<Estudiante?> GetByIdAsync(int id)
        {
            return await context.estudiantes
                .Include(e => e.Curso)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<DataTableResponseDto<EstudianteTableDto>> GetPagedAsync(
            DatoaTablesRequestDto request)
        {
            var query = context.estudiantes
                .Include(e => e.Curso)
                .AsQueryable();

            var recordsTotal = await query.CountAsync();

            var search = request.Search?.Value?.Trim();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e =>
                    e.Nombre.Contains(search) ||
                    e.Apellido.Contains(search) ||
                    e.Email.Contains(search) ||
                    e.Curso.Nombre.Contains(search));
            }

            var recordsFiltered = await query.CountAsync();

            var orderColumn = request.Order.FirstOrDefault()?.Column ?? 0;
            var orderDir = request.Order.FirstOrDefault()?.Dir ?? "asc";

            var columnName = request.Columns
                .ElementAtOrDefault(orderColumn)?.Data ?? "nombre";

            query = columnName.ToLowerInvariant() switch
            {
                "apellido" => orderDir == "desc"
                    ? query.OrderByDescending(e => e.Apellido)
                    : query.OrderBy(e => e.Apellido),

                "email" => orderDir == "desc"
                    ? query.OrderByDescending(e => e.Email)
                    : query.OrderBy(e => e.Email),

                "curso" or "curs nombre" or "curs onombre" or "curs nombre" =>
                    orderDir == "desc"
                        ? query.OrderByDescending(e => e.Curso.Nombre)
                        : query.OrderBy(e => e.Curso.Nombre),

                "cursoNombre" => orderDir == "desc"
                    ? query.OrderByDescending(e => e.Curso.Nombre)
                    : query.OrderBy(e => e.Curso.Nombre),

                "id" => orderDir == "desc"
                    ? query.OrderByDescending(e => e.Id)
                    : query.OrderBy(e => e.Id),

                _ => orderDir == "desc"
                    ? query.OrderByDescending(e => e.Nombre)
                    : query.OrderBy(e => e.Nombre)
            };

            var data = await query
                .Skip(request.Start)
                .Take(request.Length > 0 ? request.Length : 10)
                .Select(e => new EstudianteTableDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    Email = e.Email,
                    CursoId = e.CursoId,
                    CursoNombre = e.Curso.Nombre
                })
                .ToListAsync();

            return new DataTableResponseDto<EstudianteTableDto>
            {
                Draw = request.Draw,
                RecordsTotal = recordsTotal,
                RecordsTotalFiltered = recordsFiltered,
                Data = data
            };
        }

        public async Task<bool> UpdateAsync(Estudiante estudiante)
        {
            context.estudiantes.Update(estudiante);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
