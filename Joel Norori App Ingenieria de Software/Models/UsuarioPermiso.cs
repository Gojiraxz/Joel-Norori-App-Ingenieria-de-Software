namespace Joel_Norori_App_Ingenieria_de_Software.Models
{
    public class UsuarioPermiso
    {
        public int usuarioId { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;
        public int PermisoId { get; set; }
        public virtual Permiso Permiso { get; set; } = null!;
        public bool IsGranted { get; set; } = false;
    }
}
