using AutoMapper;
using BCrypt.Net;
using Joel_Norori_App_Ingenieria_de_Software.DTOS;
using Joel_Norori_App_Ingenieria_de_Software.Models;
using Joel_Norori_App_Ingenieria_de_Software.Repositories;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Services
{
    public class AuthService(IUsuarioRespository userRepo, IRoleRepository roleRepo, ITokenService tokenService, IEmailService emailService,IMapper mapper) : IAuthService
    {
        private readonly IUsuarioRespository _userRepo = userRepo;
        private readonly IRoleRepository _roleRepo = roleRepo;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IEmailService _emailService = emailService;
        private readonly IMapper _mapper = mapper;

        public async Task<bool> ChangePasswordAsync(string userName, ChangePasswordDto dto)
        {
            var user = await _userRepo.GetUsuarioByUserName(userName);
            if (user == null || !_userRepo.ValidatePassword(user, dto.CurrentPassword))
            {
                return false;
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _userRepo.SaveAsync();
            return true;

        }
        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userRepo.GetUsuarioByEmail(email);
            if(user == null || user.EmailReConfirmToken != token)
                return false;
            var decodeToken =Uri.UnescapeDataString(token.Trim());
            if(!string.Equals(user.EmailReConfirmToken, decodeToken, StringComparison.OrdinalIgnoreCase))
                return false;
            user.EmailConfirmed = true;
            user.EmailReConfirmToken = null;
            await _userRepo.SaveAsync();
            return true;

        }

        public async Task<(bool Succes, LoginResponseDto? result, string? ErrorMessage)> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepo.GetUsuarioByEmail(dto.Email);
            if(user == null || !_userRepo.ValidatePassword(user, dto.Password))
                return (false, null, "Credenciales invalidos");
            if(!user.EmailConfirmed)
                return (false, null, "El correo no ha sido confirmado");
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken =  refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userRepo.SaveAsync();
            var result = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.Now.AddMinutes(15)
            };
            return (true, result, null);


        }

        public async Task<LoginResponseDto?> RefreshAccessTokenAsync(string refreshToken)
        {
            var user = await _userRepo.GetUsuarioByRefreshTokenAsync(refreshToken);
           if(user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return null;
            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
           user.RefreshToken = newRefreshToken;
            await _userRepo.SaveAsync();
            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpiresAt = user.RefreshTokenExpiryTime ?? DateTime.UtcNow.AddDays(7)
            };




        }

        public async Task<(bool Succes, string? ErrorMessage)> RegisterAsync(RegisterRequest dto)
        {
            var exist = await _userRepo.GetUsuarioByEmail(dto.Email);
            if(exist != null)
                return (false, "El correo ya esta registrado");

            var usuarioRole = await _roleRepo.GetByNameAsync("Usuario");
            if(usuarioRole == null)
                return (false, "El rol de usuario no esta configurado");

            var user = _mapper.Map<Usuario>(dto); 
            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password);
            user.RolId = usuarioRole.Id;

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var rawToken = Convert.ToBase64String(tokenBytes);
            var encodedToken = Uri.EscapeDataString(rawToken);

            user.EmailReConfirmToken = rawToken;
            user.EmailConfirmed = false;
            await _userRepo.CreateAsync(user);

            string confirmationLink = $"https://localhost:7005//Auth/ConfirmEmail?email={user.Email} &token={encodedToken}"; 

            string htmlBody = $"""
                <!DOCTYPE html>
                <html>
                <P>Hola,</P>
                <p>
                Haz clic en el siguiente enlace para confirmar tu correo electrónico:
                </p>
                <p> <a href="{confirmationLink}" style='background:#007BBF;color:white;padding:10px 20px;border-radius:5px;text-decoration:none;</p>
                </html>
                """;
            _emailService.SendPasswordResetEmail(user.Email,htmlBody);
            return (true, null);

        }

        public async Task<bool> ResendConfirmationEmailAsync(string email)
        {
            var user = await _userRepo.GetUsuarioByEmail(email);
            if (user == null || user.EmailConfirmed)
                return false;

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var rawToken = Convert.ToBase64String(tokenBytes);
            var encodedToken = Uri.EscapeDataString(rawToken);
            
            user.EmailReConfirmToken = rawToken;
            await _userRepo.SaveAsync();

            string confirmationLink = $"https://localhost:7005//Auth/ConfirmEmail?email={user.Email} &token={encodedToken}";

            string htmlBody = $"""
                <!DOCTYPE html>
                <html>
                <P>Hola,</P>
                <p>
                Haz clic en el siguiente enlace para confirmar tu correo electrónico:
                </p>
                <p> <a href="{confirmationLink}" style='background:#007BBF;color:white;padding:10px 20px;border-radius:5px;text-decoration:none;</p>
                </html>
                """;
            _emailService.SendPasswordResetEmail(user.Email, htmlBody);
            return true;

        }

        public async Task<bool> SendResePasswordAsync(ResetPasswordRequestDto dto)
        {
            var user = await _userRepo.GetUsuarioByEmail(dto.Email);
            if (user == null)
                return false;
            var providedToken = Uri.UnescapeDataString(dto.Token ?? string.Empty);
            var storedToken = user.PasswordResetToken ?? string.Empty;
            // Compara los tokens tanto el que esta guardodo en la bd como el que se envio por correo
            if (!storedToken.Equals(providedToken,StringComparison.Ordinal)) return false;

            if (!user.PasswordResetTokenExpiryTime.HasValue || user.PasswordResetTokenExpiryTime.Value < DateTime.Now)
                return false;

            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiryTime = null;
            await _userRepo.SaveAsync();
            return true;


        }

        public async Task<bool> SendResePasswordLinkAsync(string email)
        {
            var user = await _userRepo.GetUsuarioByEmail(email);
            if(user == null)
                return false;

            var token = _tokenService.GeneratePasswordResetToken();
            var encodedToken = Uri.EscapeDataString(token);
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiryTime = DateTime.Now.AddHours(1);
            await _userRepo.SaveAsync();

            string confirmationLink = $"https://localhost:7005//Auth/ConfirmEmail?email={user.Email} &token={encodedToken}";

            string htmlBody = $"""
                <!DOCTYPE html>
                <html>
                <P>Hola,</P>
                <p>
                Haz clic en el siguiente enlace para confirmar tu correo electrónico:
                </p>
                <p> <a href="{confirmationLink}" style='background:#007BBF;color:white;padding:10px 20px;border-radius:5px;text-decoration:none;</p>
                </html>
                """;
            _emailService.SendPasswordResetEmail(user.Email, htmlBody);

            return true;
        }
    }
}
