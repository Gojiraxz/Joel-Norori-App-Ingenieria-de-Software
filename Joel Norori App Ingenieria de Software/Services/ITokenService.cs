using Joel_Norori_App_Ingenieria_de_Software.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(Usuario usuario);
        string GenerateRefreshToken();
        string GeneratePasswordResetToken();
    }
}
