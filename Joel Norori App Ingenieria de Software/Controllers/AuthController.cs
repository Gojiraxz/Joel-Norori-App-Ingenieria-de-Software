using Joel_Norori_App_Ingenieria_de_Software.DTOS;
using Joel_Norori_App_Ingenieria_de_Software.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Controllers
{
    public class AuthController(IAuthService authService) : Controller
    {
        private readonly IAuthService _authService = authService;

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {

            if (!ModelState.IsValid)
                return BadRequest("Hubo un error al registrar al usuario");
            var (success, error) = await _authService.RegisterAsync(dto);
            if (!success)
            {
                return Conflict(new
                {
                    message = error

                });

            }
            return Ok(new { success = true });

        }

        [HttpGet]
        public IActionResult Login()
        {
            var token = Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errorMessage = "Datos inválidos" });

            var (success, result, error) = await _authService.LoginAsync(dto);
            if (!success || result is null)
                return Unauthorized(new { success = false, errorMessage = error ?? "Credenciales inválidas" });

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.ExpiresAt
            };
            Response.Cookies.Append("access_token", result.AccessToken, cookieOptions);
            Response.Cookies.Append("refresh_token", result.RefreshToken, cookieOptions);
            return Ok(new
            {
                success = true,
                message = "Inicio de sesión exitoso"
            });

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            return Json(new { success = true, message = "Cierre de sesión exitoso" });
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return BadRequest("Parámetros inválidos");


            var confirmado = await _authService.ConfirmEmailAsync(email, token);
            if(confirmado)
            { 

                TempData["success"] = "Correo electrónico confirmado exitosamente. Ahora puedes iniciar sesión.";
                return RedirectToAction("Login");
            }
            TempData["error"] = "El Token es inválido o ha expirado.";
            return RedirectToAction("Login");

        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email))
            {
                return BadRequest(new { success = false, errorMessage = "Correo electrónico inválido" });
            }
            var sent= await _authService.SendResePasswordLinkAsync(model.Email);
            if(sent)
            {
                return Ok(new { success = true});
            }
            else
            {
                return NotFound(new { success = false, errorMessage = "Correo no encontrado" });

            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            var model = new ResetPasswordRequestDto
            {
                Email = email,
                Token = token
            };
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);
            var success = await _authService.SendResePasswordAsync(dto);
            if (success)
                return Ok(new { success = true });
            ModelState.AddModelError("", "Token inválido o expirado");
            return View(dto);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangePassword() => View();

        [Authorize]
        [HttpPost]
        public async Task<IActionResult>ChangePassword(ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            { 
                return View(dto);
            }
            var userName = User.Identity?.Name;
            var success = await _authService.ChangePasswordAsync(userName, dto);
            if(success)
            {
                TempData["Success"] = "Contraseña cambiada exitosamente.";
                return RedirectToAction("Logout");
            }
            ModelState.AddModelError("", "Error al cambiar la contraseña");
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();
            var newToken = await _authService.RefreshAccessTokenAsync(refreshToken);
            if(newToken is null)
                return Unauthorized();

            var accesOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = newToken.ExpiresAt
            };
            var refreshOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7) // Ajusta la duración del token de actualización según tus necesidades
            };

            Response.Cookies.Append("access_token", newToken.AccessToken, accesOptions);
            Response.Cookies.Append("refresh_token", newToken.RefreshToken, refreshOptions);
            return Ok();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
