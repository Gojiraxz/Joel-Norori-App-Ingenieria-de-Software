namespace Joel_Norori_App_Ingenieria_de_Software.Models
{
    public class RolePermiso
    {
        public int rolId { get; set; }
        public virtual Rol Role { get; set; }
        public int permisoId { get; set; }
        public virtual Permiso Permiso { get; set; } = null!;

    }
}
