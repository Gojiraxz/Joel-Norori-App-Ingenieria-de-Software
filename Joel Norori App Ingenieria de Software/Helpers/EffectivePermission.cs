using Joel_Norori_App_Ingenieria_de_Software.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Helpers
{
    public class EffectivePermission
    {
        public static IReadOnlyList<string> FromUser(Usuario usuario)
        {
            var fromRole = usuario.Role?.rolePermisos?.Select(rp => rp.Permiso.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

            var granted = usuario.usuarioPermisos?.Where(up => up.IsGranted)
                .Select(up => up.Permiso.Name)
                .ToList() ?? [];

            var denied = usuario.usuarioPermisos?
                .Where(up => !up.IsGranted)
                .Select(up => up.Permiso.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

            foreach(var permission in granted)
            {
                fromRole.Add(permission);
                fromRole.ExceptWith(denied);

            }
            return fromRole.OrderBy(p => p).ToList();

        }

    }
}
