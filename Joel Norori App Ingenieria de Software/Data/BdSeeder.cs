using Joel_Norori_App_Ingenieria_de_Software.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Data
{
    public static class BdSeeder
    {
        private static async Task SeedRolesAsync(AppDBContext context)
        {
          var changed = false;
          if(!await context.roles.AnyAsync(r => r.Name == "Admin"))
          {
                context.roles.Add(new Rol { Name = "Admin" });
                changed = true;
          }
          if (!await context.roles.AnyAsync(r => r.Name == "User"))
          {
                context.roles.Add(new Rol { Name = "User" });
                changed = true;
          }
            if (changed)
            {
                await context.SaveChangesAsync();
            }
        }
        private static async Task SeedPermisosAsync(AppDBContext context)
        {
            if (await context.permisos.AnyAsync())
            {
                return;
            }
            context.permisos.AddRange(
                new Permiso { Name = "Create", DescriptionS = "Allows creating new records" },
                new Permiso { Name = "Read", DescriptionS = "Allows reading existing records" },
                new Permiso { Name = "Update", DescriptionS = "Allows updating existing records" },
                new Permiso { Name = "Delete", DescriptionS = "Allows deleting existing records" },
                new Permiso { Name = "ManageUsers", DescriptionS = "Allows managing user accounts" },
                new Permiso { Name = "ManageRoles", DescriptionS = "Allows managing roles and permissions" },
                new Permiso { Name = "ViewReports", DescriptionS = "Allows viewing reports and analytics" },
                new Permiso { Name = "ExportData", DescriptionS = "Allows exporting data to external formats" });
                await context.SaveChangesAsync();

        }
        private static async Task SeedRolePermisosAsync(AppDBContext context)
        {
            if (await context.rolePermisos.AnyAsync())
            {
                return;
            }
            var adminRole = await context.roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            var userRole = await context.roles.FirstOrDefaultAsync(r => r.Name == "User");
            var permisos = await context.permisos.ToListAsync();
            if (adminRole != null && userRole != null && permisos.Count > 0)
            {
                foreach (var permiso in permisos)
                {
                    context.rolePermisos.Add(new RolePermiso { RolId = adminRole.Id, PermisoId = permiso.Id });
                }
                // Assign only Read permission to User role
                var readPermiso = permisos.FirstOrDefault(p => p.Name == "Read");
                if (readPermiso != null)
                {
                    context.rolePermisos.Add(new RolePermiso { RolId = userRole.Id, PermisoId = readPermiso.Id });
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
