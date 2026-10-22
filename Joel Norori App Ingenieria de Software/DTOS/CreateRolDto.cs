using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.DTOS
{
    public class CreateRolDto
    {
        [Required(ErrorMessage = "The name is required.")]
        [MaxLength(150, ErrorMessage = "The name cannot exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

    }
}
