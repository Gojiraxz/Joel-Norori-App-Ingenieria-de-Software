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
        public string RolId { get; set; }
        public List<UserPermissionOverride> EffectivePermission { get; set; } = [];
        public List<UserPermissionOverride> DirectPermission { get; set; } = [];
    }
}
