using Joel_Norori_App_Ingenieria_de_Software.Helpers;
using Joel_Norori_App_Ingenieria_de_Software.Models;
using JoelNororiAppIngenieriaDeSoftware.Extension;
using JoelNororiAppIngenieriaDeSoftware.Models.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Services
{
    public class TokenService(IOptions<JwtSettings> _JwtSettings) : ITokenService
    {
        private readonly JwtSettings _jwt = _JwtSettings.Value;
        public string GenerateAccessToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier,usuario.Id.ToString()),
                new(ClaimTypes.Name,usuario.NombreUsuario),
                new(ClaimTypes.Email,usuario.Email),
                new(ClaimTypes.Role,usuario.Role?.Name??"Usuario")

            };
            foreach (var permission in EffectivePermission.FromUser(usuario))
            {
                claims.Add(new Claim(ClaimPrincipalExtension.PermisoClaimType, permission));

            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creed = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(

                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwt.TokenExpirationInMinutes),
                signingCredentials: creed
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    

        public string GeneratePasswordResetToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
    }
}
