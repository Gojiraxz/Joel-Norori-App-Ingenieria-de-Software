
using Joel_Norori_App_Ingenieria_de_Software.Data;
using Joel_Norori_App_Ingenieria_de_Software.DTOS;
using Joel_Norori_App_Ingenieria_de_Software.Models;
using Microsoft.EntityFrameworkCore;

namespace Joel_Norori_App_Ingenieria_de_Software.Repositories
{
    public class PermissionRepository(AppDBContext context) : IPermissionRepository
    {
        public async Task<Permiso> CreateAsync(string name, string? DescriptionS)
        {
            var permission = new Permiso
            {
                Name = name.Trim().ToLowerInvariant(),
                DescriptionS = DescriptionS?.Trim()
            };

            var entry = await context.permisos.AddAsync(permission);
            await context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var permission = await context.permisos.FindAsync(id);

            if (permission == null)
                return false;

            context.permisos.Remove(permission);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<Permiso>> GetAllAsync()
        {
            return await context.permisos
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<Permiso?> GetByIdAsync(int id)
        {
            return await context.permisos.FindAsync(id);
        }

        public async Task<Permiso?> GetByNameAsync(string name)
        {
            var normalizedName = name.Trim().ToLowerInvariant();

            return await context.permisos
                .FirstOrDefaultAsync(x => x.Name == normalizedName);
        }

        public async Task<DataTableResponseDto<PermissionDto>> GetPagedAsync(
            DatoaTablesRequestDto request)
        {
            var query = context.permisos.AsQueryable();

            var recordsTotal = await query.CountAsync();

            var search = request.Search?.Value?.Trim();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    c.Name.Contains(search) ||
                    (c.DescriptionS != null && c.DescriptionS.Contains(search)));
            }

            var recordsFiltered = await query.CountAsync();

            var orderColumn = request.Order.FirstOrDefault()?.Column ?? 0;
            var orderDir = request.Order.FirstOrDefault()?.Dir ?? "asc";

            var columnName = request.Columns
                .ElementAtOrDefault(orderColumn)?.Data ?? "nombre";

            query = columnName.ToLowerInvariant() switch
            {
                "descripcion" => orderDir == "desc"
                    ? query.OrderByDescending(c => c.DescriptionS)
                    : query.OrderBy(c => c.DescriptionS),

                "id" => orderDir == "desc"
                    ? query.OrderByDescending(c => c.Id)
                    : query.OrderBy(c => c.Id),

                _ => orderDir == "desc"
                    ? query.OrderByDescending(c => c.Name)
                    : query.OrderBy(c => c.Name)
            };

            var data = await query
                .Skip(request.Start)
                .Take(request.Length > 0 ? request.Length : 10)
                .Select(c => new PermissionDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.DescriptionS
                })
                .ToListAsync();

            return new DataTableResponseDto<PermissionDto>
            {
                Draw = request.Draw,
                RecordsTotal = recordsTotal,
                RecordsTotalFiltered = recordsFiltered,
                Data = data
            };
        }

        public async Task<bool> IsAssingnedToUsuarioAsync(int PermissionId)
        {
            return await context.usuarioPermisos
                .AnyAsync(up => up.PermisoId == PermissionId);
        }

        public async Task<bool> UpdateAsync(Permiso permiso)
        {
            var existingPermission = await context.permisos
                .FindAsync(permiso.Id);

            if (existingPermission == null)
                return false;

            existingPermission.Name = permiso.Name.Trim().ToLowerInvariant();
            existingPermission.DescriptionS = permiso.DescriptionS?.Trim();

            await context.SaveChangesAsync();

            return true;
        }
    }
}

