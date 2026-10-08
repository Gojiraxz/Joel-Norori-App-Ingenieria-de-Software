namespace Joel_Norori_App_Ingenieria_de_Software.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? EmailReConfirmToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? PasswordResetToken { get; set; } 
        public DateTime? PasswordResetTokenExpiryTime { get; set; }

        public int RolId { get; set; }
        public virtual Rol Role { get; set; }

        public ICollection<UsuarioPermiso> usuarioPermisos { get; set; } = new List<UsuarioPermiso>();




    }
}
