namespace Joel_Norori_App_Ingenieria_de_Software.Models
{
    public class Estudiante
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set;}
        public string Email { get; set; }
        public int CursoId { get; set; }

        public virtual Curso Curso { get; set; }

    }
}
