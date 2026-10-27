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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RolePermiso>().HasKey(rp => new { rp.rolId, rp.permisoId });

            modelBuilder.Entity<RolePermiso>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.rolePermisos)
                .HasForeignKey(rp => rp.rolId)
                .OnDelete(DeleteBehavior.Cascade);  

            modelBuilder.Entity<RolePermiso>()
                .HasOne(rp => rp.Permiso)
                .WithMany(p => p.RolePermisos)
                .HasForeignKey(rp => rp.permisoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioPermiso>()
                .HasOne(up => up.Usuario)
                .WithMany(u => u.usuarioPermisos)
                .HasForeignKey(up => up.usuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioPermiso>()
                .HasOne(up => up.Permiso)
                .WithMany(p => p.UsuarioPermisos)
                .HasForeignKey(up => up.PermisoId)
                .OnDelete(DeleteBehavior.Cascade);

           modelBuilder.Entity<Estudiante>()
           .HasOne(e => e.Curso)
           .WithMany(e => e.Estudiantes)
           .HasForeignKey(e => e.CursoId)
           .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
