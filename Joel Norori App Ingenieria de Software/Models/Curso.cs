namespace Joel_Norori_App_Ingenieria_de_Software.Models
{
    public class Curso
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }

        public ICollection<Estudiante> Estudiantes { get; set; } = [];
    }
}
