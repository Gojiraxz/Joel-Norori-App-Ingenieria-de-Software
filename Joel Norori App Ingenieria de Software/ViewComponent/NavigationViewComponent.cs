using Joel_Norori_App_Ingenieria_de_Software.Helpers;
using Joel_Norori_App_Ingenieria_de_Software.Repositories;
using Microsoft.AspNetCore.Mvc;
using Joel_Norori_App_Ingenieria_de_Software.Models;
using System.Security.Claims;

namespace Practica_JWT_creo.ViewComponents
{
    public class NavigationViewComponent(IUsuarioRespository usuarioRepositorie) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = HttpContext.User as ClaimsPrincipal;
            IReadOnlyList<string> permissions = Array.Empty<string>();
            if (user?.Identity?.IsAuthenticated ?? false && int.TryParse(user?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int Id))
                {
                    var dbUser = await usuarioRepositorie.GetUsuarioByIdWithPermissionAsync(Id);
                    if (dbUser != null)
                    {
                        permissions = EffectivePermission.FromUser(dbUser);
                    }
                }
            }
            return View(permissions);

        }
    }
}