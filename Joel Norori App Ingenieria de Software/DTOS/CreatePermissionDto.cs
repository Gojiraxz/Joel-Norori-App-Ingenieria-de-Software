using System;
using System.ComponentModel.DataAnnotations;

namespace Joel_Norori_App_Ingenieria_de_Software.Models.Settings
{
	public class CreatePermissionDto
	{
		[Required(ErrorMessage = "El nombre es obligatorio")]
		[MaxLength(150, ErrorMessage = "El nombre no puede tener más de 150 caracteres")]
		[RegularExpression(@"^[a-zA-Z0-9._]*$", ErrorMessage = "Use el formato Recurso.Accion")]
		public string Name { get; set; } = string.Empty;
		[MaxLength(200)]
		public string Descripcion { get; set; }
	
	}
}
