using System;
using System.Security.Claims;

namespace JoelNororiAppIngenieriaDeSoftware.Extension
{
	public class ClaimPrincipalExtension
	{
		public const string PermisoClaimType = "Permiso";

		public static bool HasPermiso(this ClaimsPrincipal user, string permiso)
		{
			return user.Claims.Any(c => c.Type == PermisoClaimType &&
			string.Equals(c.Value, permiso, StringComparison.OrdinalIgnoreCase));

		}

		public static bool HasAnyPermiso(this ClaimsPrincipal user, params string[] permisos)
		{
			return permisos.any(user.HasPermiso(permiso));

		}
		public static IreadOnlyList< <string> GetPermisos(this ClaimsPrincipal user)
		{
			return user.Claims
			 .Where(c => c.Type == PermisoClaimType)
			 .Select(c => c.Value)
			 .orderBy(p => p)
			 .ToList();
		}
	}

