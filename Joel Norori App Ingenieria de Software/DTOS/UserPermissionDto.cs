using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.DTOS
{
    public class UserPermissionDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RolId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public List<string> EfectivePermission { get; set; } = [];
        public List<UserPermissionOverrideDto> DirectPermission { get; set; } = [];
    }
}
