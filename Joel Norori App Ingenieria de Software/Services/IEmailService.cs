using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Services
{
    public interface IEmailService
    {
        void SendPasswordResetEmail(string toEmail, string body);
    }
}
