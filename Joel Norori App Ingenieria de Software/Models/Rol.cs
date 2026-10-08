namespace Joel_Norori_App_Ingenieria_de_Software.Models
{
    public class Rol
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }=string.Empty;
        public ICollection<Usuario> usuarios{ get; set; } = [];
        public ICollection<RolePermiso> rolePermisos { get; set; } = [];
    }
}
