using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.DTOS
{
    public class SaveEstudianteDto
    {
        public int? Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }=string.Empty;
        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public int CursoId { get; set; }
    }
}
