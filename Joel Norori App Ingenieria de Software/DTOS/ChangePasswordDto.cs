using System;

namespace Joel_Norori_App_Ingenieria_de_Software.DTOS
{
	public class ChangePasswordDto
	{
		public required string CurrentPassword { get; set; }
		public string NewPassword { get; set; }

}
}
