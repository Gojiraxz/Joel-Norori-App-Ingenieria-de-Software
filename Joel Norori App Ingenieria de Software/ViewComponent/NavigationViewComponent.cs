using Joel_Norori_App_Ingenieria_de_Software.Extension;
using Joel_Norori_App_Ingenieria_de_Software.Helpers;
using Joel_Norori_App_Ingenieria_de_Software.Models;
using Joel_Norori_App_Ingenieria_de_Software.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Practica_JWT_creo.ViewComponents
{
    public class NavigationViewComponent(IUsuarioRespository _usuarioRepositorio) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = HttpContext.User as ClaimsPrincipal;
            var permission = user?.GetPermisos() ?? [];
            if (user?.Identity?.IsAuthenticated == true &&
                int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id))
            {
                var dbUser = await _usuarioRepositorio.GetUsuarioByIdWithPermissionAsync(id);
                if (dbUser != null)
                    permission = EffectivePermission.FromUser(dbUser);
            }
            return View(permission);
        }


    }
}