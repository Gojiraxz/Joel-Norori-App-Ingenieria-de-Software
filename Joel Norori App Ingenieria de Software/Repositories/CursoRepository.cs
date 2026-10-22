using Joel_Norori_App_Ingenieria_de_Software.Data;
using Joel_Norori_App_Ingenieria_de_Software.DTOS;
using Joel_Norori_App_Ingenieria_de_Software.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Repositories
{
    public class CursoRepository(AppDBContext context) : ICursoRepository
    {
        public async Task<Curso> CreateAsync(Curso curso)
        {
            var entry = await context.AddAsync(curso);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var curso= await context.cursos.FindAsync(id);
            if(curso == null) return false;

            context.cursos.Remove(curso);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Curso>> GetAllAsync()
        {
            return await context.cursos.OrderBy(c => c.Nombre).ToListAsync();
        }

        public async Task<DataTableResponseDto<CursoTableDto>> GetPagedAsync(DatoaTablesRequestDto request)
        {
            var query = context.cursos.AsQueryable();
            var recordsTotal = await query.CountAsync();
            var search = request.Search?.Value?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.Nombre.Contains(search)
                || (c.descripcion != null && c.Descripcion.Contains(search)
                ));

            }
            var recordsFiltered = await query.CountAsync();
            var orderColumn = request.Order.FirstOrDefault()?.Column ?? 0;
            var orderDir = request.Order.FirstOrDefault()?.Dir ?? "asc";
            var columnName = request.Columns.ElementAtOrDefault(orderColumn)?.Data ?? "nombre";

            query = columnName.ToLowerInvariant() switch
            {
                "descripcion" => orderDir == "desc" ? query.OrderByDescending(c => c.Descripcion) : query.OrderBy(c => c.Descripcion),
                "totalestudiantes" => orderDir ? == "desc" ?
                query.OrderByDescending(c => c.Descripcion) : query.OrderBy(c => c.Estudiantes.Count),
                "id" => orderDir == "desc" ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                _ => orderDir == "desc" ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre)



            };

            var data = await query
                .Skip(request.Start)
                .Take(request.Length > 0 ? request.Length : 10)
                .Select(c => new CursoTableDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion
                    TotalEstudiantes = c.Estudiantes.Count
                }).ToListAsync();
            return new DataTableResponseDto<CursoTableDto>
            {
                Draw = request.Draw,
                RecordsTotal = recordsTotal,
                RecordsTotalFiltered = recordsFiltered
                Data = data
            };
        }

        public async Task<Curso?> GetByIdAsync(int id)
        {
            return await context.cursos.FindAsync(id);
        }

        public async Task<bool> HasAnyEstudianteAsync(int Id)
        {
            return await context.cursos.AnyAsync
                (c => c.Id ==Id);
        }

        public async Task<bool> NombreExisteAsync(string nombre, int? excluedeId)
        {
            return await context.cursos.AnyAsync
                (c=>c.Nombre == nombre && (!excluedeId.HasValue|| c.Id != excluedeId.Value));   
        }

        public async Task<bool> UpdateAsync(Curso curso)
        {
            context.cursos.Update(curso);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
