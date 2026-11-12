using Joel_Norori_App_Ingenieria_de_Software;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Joel_Norori_App_Ingenieria_de_Software.Extension
{
    public static class ClaimPrincipalExtension
    {
        public const string PermisoClaimType = "permission";

        public static bool HasPermiso(this ClaimsPrincipal user, string permission)
        {
            return user.Claims.Any(c =>
                c.Type == PermisoClaimType &&
                string.Equals(c.Value, permission, StringComparison.OrdinalIgnoreCase));
        }

        public static bool HasAnyPermiso(this ClaimsPrincipal user, params string[] permission)
        {
            return permission.Any(p => user.HasPermiso(p));
        }

        public static IReadOnlyList<string> GetPermisos(this ClaimsPrincipal user)
        {
            return user.Claims
                .Where(c => c.Type == PermisoClaimType)
                .Select(c => c.Value)
                .OrderBy(p => p)
                .ToList();
        }
    }
}
