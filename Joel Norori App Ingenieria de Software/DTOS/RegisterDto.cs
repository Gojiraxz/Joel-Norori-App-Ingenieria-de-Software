using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.DTOS
{
    public class RegisterDto
    {
        public required string Username { get; set; }
        public required string Email { get; set; } 
        public required string Password { get; set; }
        public int RoleId { get; set; } = 2;
    }
}
