namespace Joel_Norori_App_Ingenieria_de_Software.Models
{
    public class Permiso
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }

        public ICollection<UsuarioPermiso> UsuarioPermisos { get; set; } = new List<UsuarioPermiso>();
        public ICollection<RolePermiso> RolePermisos { get; set; } = new List<RolePermiso>();

    }
}
