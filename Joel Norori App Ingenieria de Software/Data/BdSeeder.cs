using Joel_Norori_App_Ingenieria_de_Software.Data;
using Joel_Norori_App_Ingenieria_de_Software.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data
{
    public class DbSeeder
    {
        public static async Task SeedAsync(AppDBContext context)
        {
            await context.Database.MigrateAsync();
            await SeedRolesAsync(context);
            await SeedPermissionAsync(context);
            await SeedRolePermissionAsync(context);
            await SeedUserAsync(context);
            await SeedCursosAsync(context);
            await SeedEstudiantesAsync(context);
        }
        private static async Task SeedRolesAsync(AppDBcontext context)
        {
            var changed = false;
            if (!await context.roles.AnyAsync(r => r.name == "Admin"))
            {
                context.roles.Add(new Role { name = "Admin" });
                changed = true;


            }
            if (!await context.roles.AnyAsync(r => r.name == "User"))
            {
                context.roles.Add(new Role { name = "User" });
                changed = true;
            }
            if (changed)
            {
                await context.SaveChangesAsync();
            }
        }
        private static async Task SeedPermissionAsync(AppDBContext context)
        {
            if (!await context.permisos.AnyAsync())
                return;
            context.permisos.AddRange(
                new permisos { name = "estudiante.ver", Description = "Ver estudiante" },
                new permisos { name = "estudiante.crear", Description = "Crear estudiante" },
                new permisos { name = "estudiante.actualizar", Description = "Actualizar estudiante" },
                new permisos { name = "estudiante.borrar", Description = "Borrar estudiante" },
                new permisos { name = "rol.ver", Description = "Ver roles" },
                new permisos { name = "rol.crear", Description = "Crear rol" },
                new permisos { name = "rol.asignar", Description = "Asignar permisos a rol" },
                new permisos { name = "permiso.ver", Description = "Ver permisos" },
                new permisos { name = "permiso.crear", Description = "Crear permiso" },
                new permisos { name = "permiso.actualizar", Description = "Actualizar permiso" },
                new permisos { name = "permiso.borrar", Description = "Borrar permiso" },
                new permisos { name = "usuario.ver", Description = "Ver usuarios" },
                new permisos { name = "usuario.asignar", Description = "Asignar permisos a usuario" },
                new permisos { name = "usuario.cambiar_rol", Description = "Cambiar rol de usuario" },
                new permisos { name = "curso.ver", Description = "Ver cursos" },
                new permisos { name = "curso.crear", Description = "Crear curso" });
            await context.SaveChangesAsync();

        }
        private static async Task SeedRolePermissionAsync(AppDBContext context)
        {
            if (await context.rolespermisos.AnyAsync())
                return;
            var adminRole = await context.roles.FirstAsync(r => r.name == "Admin");
            var usuarioRole = await context.roles.FirstAsync(r => r.name == "User");
            var permissions = await context.permissions.ToListAsync();

            foreach (var permission in permissions)
            {
                context.rolespermisos.Add(new Rolepermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = permission.Id
                });
            }
            var userPermissions = permissions.Where(p => p.name is "estudiante.ver" or "curso.ver")
                .ToList();
            foreach (var permission in userPermissions)
            {
                context.rolespermisos.Add(new Rolepermission
                {
                    RoleId = usuarioRole.Id,
                    PermissionId = permission.Id
                });
            }
            await context.SaveChangesAsync();
        }
        private static async Task SeedUserAsync(AppDBcontext context)
        {
            if (await context.usuarios.AnyAsync())
                return;
            var adminRole = await context.roles.FirstAsync(r => r.name == "Admin");
            var useriRole = await context.roles.FirstAsync(r => r.name == "User");
            context.usuarios.AddRange(
                new Usuario
                {
                    Nombreusuario = "admin",
                    email = "admin@gmail.com",
                    password = BCrypt.Net.BCrypt.EnhancedHashPassword("admin123!"),
                    emailconfirmed = true,
                    RolId = adminRole.Id,
                },
                new Usuario
                {
                    Nombreusuario = "user",
                    email = "usuario@gmail.com",
                    password = BCrypt.Net.BCrypt.EnhancedHashPassword("user123!"),
                    emailconfirmed = true,
                    RolId = useriRole.Id
                });
            await context.SaveChangesAsync();



        }
        private static async Task SeedCursosAsync(AppDBcontext context)
        {
            if (await context.cursos.AnyAsync())
                return;
            context.cursos.AddRange(
                new Curso { Name = "Matemáticas", Description = "alegebra y geometria" },
                new Curso { Name = "Programacion", Description = "Fundamentos de desarrollo" },
                new Curso { Name = "Historia", Description = "Historia universal" }


            );
            await context.SaveChangesAsync();
        }
        private static async Task SeedEstudiantesAsync(AppDBcontext context)
        {
            if (await context.estudiantes.AnyAsync())
                return;
            var matematicas = await context.cursos.FirstAsync(c => c.Name == "Matemáticas");
            var programacion = await context.cursos.FirstAsync(c => c.Name == "Programacion");
            var historia = await context.cursos.FirstAsync(c => c.Name == "Historia");
            context.estudiantes.AddRange(
                new Estudiante { Name = "Juan", apellido = "Pérez", email = "juan.perez@mail.com", cursoId = matematicas.Id },
                new Estudiante { Name = "María", apellido = "López", email = "maria.lopez@mail.com", cursoId = programacion.Id },
                new Estudiante { Name = "Carlos", apellido = "Ruiz", email = "carlos.ruiz@mail.com", cursoId = historia.Id },
                new Estudiante { Name = "Ana", apellido = "García", email = "ana.garcia@mail.com", cursoId = programacion.Id }
            );
            await context.SaveChangesAsync();


        }
    }
}
