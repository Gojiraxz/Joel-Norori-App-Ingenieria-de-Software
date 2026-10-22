using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.DTOS
{
    public class SavePermissionDto
    {
        public int? Id { get; set; }
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"[a-z] [a-z0-9._]+$", ErrorMessage = "Use formato: recurso.accion(ej: estudiantes.crear)")]
        public string Name { get; set; }=string.Empty;
        [MaxLength(250)]
        public string? Descripcion { get; set; }
    }
}
