using Joel_Norori_App_Ingenieria_de_Software.DTOS;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Services
{
    public interface IAuthService
    {
        Task<(bool Succes, string? ErrorMessage)> RegisterAsync(RegisterRequest dto);
        Task<(bool Succes, LoginResponseDto? result,string? ErrorMessage)> LoginAsync(LoginRequestDto dto);
        Task<bool> ConfirmEmailAsync(string email, string token);
        Task<bool> SendResePasswordLinkAsync(string email);
        Task<bool> SendResePasswordAsync(ResetPasswordRequestDto dto);
        Task<bool> ResendConfirmationEmailAsync(string email);
        Task<bool> ChangePasswordAsync(string userName, ChangePasswordDto dto);
        Task<LoginResponseDto?> RefreshAccessTokenAsync(string refreshToken);
    }
}
