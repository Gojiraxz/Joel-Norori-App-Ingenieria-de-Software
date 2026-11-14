using Joel_Norori_App_Ingenieria_de_Software.DTOS;
using Joel_Norori_App_Ingenieria_de_Software.Helpers;
using Joel_Norori_App_Ingenieria_de_Software.Models;
using Joel_Norori_App_Ingenieria_de_Software.Repositories;
using Joel_Norori_App_Ingenieria_de_Software.Services;
using Joel_Norori_App_Ingenieria_de_Software.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;
using System.Security.Claims;

namespace Joel_Norori_App_Ingenieria_de_Software.Controllers
{
    public class AdminController(IUsuarioRespository _usuarioRepository, IRolePermissionService _rolePermissionService) : Controller
    {
        private async Task<IReadOnlyList<string>> GetEffectivePermissionAsync()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return User.GetPermisos();
            var dbUser = await _usuarioRepository.GetUsuarioByIdWithPermissionAsync(userId);
            return dbUser != null ? EffectivePermission.FromUser(dbUser) : User.GetPermisos();
        }
        [Authorize(Policy = "admin.panel")]
        public IActionResult Index()
        {
            ViewBag.Permissions = User.GetPermisos();
            return View();
        }
        [Authorize(Policy = "permiso.ver")]
        [HttpPost]
        public async Task<IActionResult> GetPermissionData([FromForm] DatoaTablesRequestDto request)
        {
            var result = await _rolePermissionService.GetPermissionPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy ="permiso.ver")]
        public async Task<IActionResult> Permissions()
        {
            var effective = await GetEffectivePermissionAsync();

            ViewBag.CanCreate = effective.Contains("permiso.crear", StringComparer.OrdinalIgnoreCase);
            ViewBag.CanUpdae = effective.Any(p =>
            string.Equals(p, "permiso.actualizar", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(p, "permiso.crear", StringComparison.OrdinalIgnoreCase));
            ViewBag.CanDelete = effective.Any(p =>
            string.Equals(p, "permiso.borrar", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(p, "permiso.crear", StringComparison.OrdinalIgnoreCase));
            return View();
        }
        [Authorize(Policy = "permiso.ver")]
        [HttpGet]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            var permission = await _rolePermissionService.GetPermissionByIdAsync(id);
            if (permission == null)
                return NotFound(new { message = "Permiso no encontrado" });
            return Ok(permission);
        }
        [Authorize(Policy = "permiso.crear")]
        [HttpGet]
        public async Task<IActionResult> CreatePermission([FromBody] SavePermissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Revise los datos del formulario" });

            var (success,_,errorMessage) = await _rolePermissionService.CreatePermissionAsync(dto);
            if (success)
                return BadRequest(new { success = false, message = errorMessage });

            return Ok(new { success = true, messge = $"Permiso '{dto.Name}' Creado Correctamente" });
        }
        [Authorize(Policy = "permiso.actualizar")]
        [HttpGet]
        public async Task<IActionResult> UpdatePermission([FromBody] SavePermissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Revise los datos del formulario" });

            var (success, errorMessage) = await _rolePermissionService.UpdatePermissionAsync(dto);
            if (success)
                return BadRequest(new { success = false, message = errorMessage });

            return Ok(new { success = true, messge = $"Permiso '{dto.Name}' Actualizado Correctamente" });
        }

        [Authorize(Policy = "permiso.borrar")]
        [HttpPost]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var (success, errorMessage) = await _rolePermissionService.DeletePermissionAsync(id);
            if (!success)
                return BadRequest(new { success = false, message = errorMessage });

            return Ok(new { success = true, messge = $"Permiso Borrado Correctamente" });

        }
        [Authorize(Policy = "rol.ver")]
        public async Task<IActionResult> Roles()
        {
            var roles = await _rolePermissionService.GetAllRolesAsync();
            return View(roles);
        }

        [Authorize(Policy = "rol.crear")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRol(CreateRolDto dto)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _rolePermissionService.GetAllRolesAsync();
                ViewBag.CreateError = "El nombre del rol es obligatorio";
                return View("Roles", roles);
            }

            var (success, _, errorMessage) = await _rolePermissionService.CreateRoleAsync(dto);
            if (!success)
            {
                var roles = await _rolePermissionService.GetAllRolesAsync();
                ViewBag.CreateError = errorMessage;
                return View("Roles", roles);    
            }
            TempData["Success"] = $"Rol '{dto.Name}' Creado Correctamente";
            return RedirectToAction(nameof(Roles));

        }
        [Authorize(Policy ="rol.asignar")]
        public async Task<IActionResult> RoleDetails(int id)
        {
            var role = await _rolePermissionService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound();
            var allPermission = await _rolePermissionService.GetAllPermissionsAsync();
            ViewBag.AllPermission = allPermission;
            return View(role);
        }
        [Authorize(Policy ="rol.asignar")]
        [HttpPost]
        public async Task<IActionResult> AssignPermissionToRole(int roleId, int permissionId)
        {
            var (success, errorMessage) = await _rolePermissionService.AssignPermissionToRoleAsync(roleId, permissionId);
            return Ok(new { success = true, message = success ? "Permiso Asignado" : errorMessage });
        }
        [Authorize(Policy ="rol.asignar")]
        [HttpPost]
        public async Task<IActionResult> RemovePermissionFromRole(int roleId, int permissionId)
        {
            var (succes, errorMessage) = await _rolePermissionService.RemovePermissionFromRoleAsync(roleId, permissionId);
            return Json(new { succes, message = succes ? "Permiso Removido": errorMessage });
        }

        [Authorize(Policy = "usuario.ver")]
        [HttpPost]
        public async Task<IActionResult> Users()
        {
            var users = await _rolePermissionService.GetAllPermissionsAsync();
            return View(users);
        }
        [Authorize(Policy ="usuario.asignar")]
        public async Task<IActionResult> UserPermissions(int id)
        {
            var user = await _rolePermissionService.GetUserPermissionAsync(id);
            if (user == null)
                return NotFound();
            var allPermission = await _rolePermissionService.GetAllPermissionsAsync();
            var allRoles = await _rolePermissionService.GetAllRolesAsync();
            ViewBag.AllPermissions = allPermission;
            ViewBag.AllRoles = allRoles;
            ViewBag.CanChangeRole = User.HasPermiso("usuario.cambiar_rol");
            return View(user);
        }
        [Authorize(Policy = "usuario.asignar")]
        [HttpPost]
        public async Task<IActionResult> AssingPermissionToUser(int userId, [FromBody] AssingUserPermissionDto dto)
        {
            var (success, errorMessage) = await _rolePermissionService.AssignPermissionToUserAsync(userId, dto);
            return Json(new
            {
                success,
                message = success
                ? (dto.IsGranted ? "Permiso concedido al usuario." : "Permiso denegado al usuario") : errorMessage
            });
        
        }
        [Authorize(Policy ="usuario.asignar")]
        [HttpPost]
        public async Task<IActionResult> RemoveUserPermission(int userId, int permissionId)
        {
            var (success, errorMessage) = await _rolePermissionService.RemovePermissionFromRoleAsync(userId, permissionId);
            return Json(new { success, message = success ? "Asignacion directa removida" : errorMessage });

        }
        [Authorize(Policy = "usuario.cambiar_rol")]
        [HttpPost]
        public async Task<IActionResult> UpdateRol(int userId,[FromBody] UpdateUserRoleDto dto )
        {
            var (success, errorMessage) = await _rolePermissionService.UpdateUserRoleAsync(userId,dto.RolId);
            return Json(new { success, message = success ? "Rol actualizado" : errorMessage });

        }

    }
}
