using Joel_Norori_App_Ingenieria_de_Software.Models;
using Microsoft.EntityFrameworkCore;

namespace Joel_Norori_App_Ingenieria_de_Software.Data
{
    public class AppDBContext(DbContextOptions<AppDBContext> options) : DbContext(options)
    {
        public DbSet<Rol> roles { get; set; }
        public DbSet<Permiso> permisos { get; set; }
        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<RolePermiso> rolePermisos { get; set; }
        public DbSet<Estudiante> estudiantes { get; set; }
        public DbSet<UsuarioPermiso> usuarioPermisos { get; set; }
        public DbSet<Curso> cursos { get; set; }

    }
}
