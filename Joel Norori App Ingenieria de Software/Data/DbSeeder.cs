using Joel_Norori_App_Ingenieria_de_Software.Data;
using Joel_Norori_App_Ingenieria_de_Software.Models;
using Microsoft.EntityFrameworkCore;
using System;

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
        private static async Task SeedRolesAsync(AppDBContext context)
        {
            var changed = false;
            if (!await context.roles.AnyAsync(r => r.Name == "Admin"))
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
        private static async Task SeedPermissionAsync(AppDBContext context)
        {
            if (await context.permisos.AnyAsync())
                return;
            context.permisos.AddRange(
                new Permiso { Name = "estudiante.ver", DescriptionS = "Ver estudiante" },
                new Permiso { Name = "estudiante.crear", DescriptionS = "Crear estudiante" },
                new Permiso { Name = "estudiante.actualizar", DescriptionS = "Actualizar estudiante" },
                new Permiso { Name = "estudiante.borrar", DescriptionS = "Borrar estudiante" },
                new Permiso { Name = "rol.ver", DescriptionS = "Ver roles" },
                new Permiso{ Name = "rol.crear", DescriptionS = "Crear rol" },
                new Permiso { Name = "rol.asignar", DescriptionS = "Asignar permisos a rol" },
                new Permiso { Name = "permiso.ver", DescriptionS = "Ver permisos" },
                new Permiso { Name = "permiso.crear", DescriptionS = "Crear permiso" },
                new Permiso { Name = "permiso.actualizar", DescriptionS = "Actualizar permiso" },
                new Permiso { Name = "permiso.borrar", DescriptionS = "Borrar permiso" },
                new Permiso { Name = "usuario.ver", DescriptionS = "Ver usuarios" },
                new Permiso { Name = "usuario.asignar", DescriptionS = "Asignar permisos a usuario" },
                new Permiso { Name = "usuario.cambiar_rol", DescriptionS = "Cambiar rol de usuario" },
                new Permiso { Name = "curso.ver", DescriptionS = "Ver cursos" },
                new Permiso { Name = "curso.actualizar", DescriptionS = "actualizar cursos" },
                new Permiso { Name = "curso.borrar", DescriptionS = "borrar cursos" },
                new Permiso { Name = "curso.crear", DescriptionS = "Crear curso" });
            await context.SaveChangesAsync();

        }
        private static async Task SeedRolePermissionAsync(AppDBContext context)
        {
            if (await context.rolePermisos.AnyAsync())
                return;
            var adminRole = await context.roles.FirstAsync(r => r.Name == "Admin");
            var usuarioRole = await context.roles.FirstAsync(r => r.Name == "User");
            var permissions = await context.permisos.ToListAsync();

            foreach (var permission in permissions)
            {
                context.rolePermisos.Add(new RolePermiso
                {
                    rolId = adminRole.Id,
                    permisoId = permission.Id
                });
            }
            var userPermissions = permissions.Where(p => p.Name is "estudiante.ver" or "curso.ver")
                .ToList();
            foreach (var permission in userPermissions)
            {
                context.rolePermisos.Add(new RolePermiso
                {
                    rolId = usuarioRole.Id,
                    permisoId = permission.Id
                });
            }
            await context.SaveChangesAsync();
        }
        private static async Task SeedUserAsync(AppDBContext context)
        {
            if (await context.usuarios.AnyAsync())
                return;
            var adminRole = await context.roles.FirstAsync(r => r.Name == "Admin");
            var useriRole = await context.roles.FirstAsync(r => r.Name == "User");
            context.usuarios.AddRange(
                new Usuario
                {
                    NombreUsuario = "admin",
                    Email = "admin@gmail.com",
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword("admin123!"),
                    EmailConfirmed = true,
                    RolId = adminRole.Id,
                },
                new Usuario
                {
                    NombreUsuario = "user",
                    Email = "usuario@gmail.com",
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword("user123!"),
                    EmailConfirmed = true,
                    RolId = useriRole.Id
                });
            await context.SaveChangesAsync();



        }
        private static async Task SeedCursosAsync(AppDBContext context)
        {
            if (await context.cursos.AnyAsync())
                return;
            context.cursos.AddRange(
                new Curso { Nombre = "Matemáticas", Descripcion = "alegebra y geometria" },
                new Curso { Nombre = "Programacion", Descripcion = "Fundamentos de desarrollo" },
                new Curso { Nombre = "Historia", Descripcion = "Historia universal" }


            );
            await context.SaveChangesAsync();
        }
        private static async Task SeedEstudiantesAsync(AppDBContext context)
        {
            if (await context.estudiantes.AnyAsync())
                return;
            var matematicas = await context.cursos.FirstAsync(c => c.Nombre == "Matemáticas");
            var programacion = await context.cursos.FirstAsync(c => c.Nombre == "Programacion");
            var historia = await context.cursos.FirstAsync(c => c.Nombre == "Historia");
            context.estudiantes.AddRange(
                new Estudiante { Nombre = "Juan", Apellido = "Pérez", Email = "juan.perez@mail.com", CursoId = matematicas.Id },
                new Estudiante { Nombre = "María", Apellido = "López", Email = "maria.lopez@mail.com", CursoId = programacion.Id },
                new Estudiante { Nombre = "Carlos", Apellido = "Ruiz", Email = "carlos.ruiz@mail.com", CursoId = historia.Id },
                new Estudiante { Nombre = "Ana", Apellido = "García", Email = "ana.garcia@mail.com", CursoId = programacion.Id }
            );
            await context.SaveChangesAsync();


        }
    }
}
