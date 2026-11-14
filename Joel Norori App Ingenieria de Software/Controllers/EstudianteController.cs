using Joel_Norori_App_Ingenieria_de_Software.DTOS;
using Joel_Norori_App_Ingenieria_de_Software.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ProyectoAuthJwt.Controllers
{
    public class EstudianteController(IEstudianteService _estudianteService, ICursoService _cursoService) : Controller
    {
        [Authorize(Policy = "estudiante.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Cursos = await _cursoService.GetAllAsync();
            return View();
        }

        [Authorize(Policy = "estudiante.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData(DatoaTablesRequestDto request)
        {
            var result = await _estudianteService.GetPagedAsync(request);
            return Ok(result);
        }

        [Authorize(Policy = "curso.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _estudianteService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Estudiante no encontrado" });
            return Ok(result);
        }

        [Authorize(Policy = "estudiante.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveEstudianteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Error en los datos a guardar" });
            var (success, errorMessage) = await _estudianteService.CreateAsync(dto);
            return success ?
                Ok(new { success = true, message = "Estudiante creado sastifactoriamente" })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "estudiante.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody]SaveEstudianteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Error en los datos a guardar" });
            var (success, errorMessage) = await _estudianteService.UpdateAsync(dto);
            return success ?
                Ok(new { success = true, message = "Estudiante actualizado sastifactoriamente" })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "estudiante.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) = await _estudianteService.DeleteAsync(id);
            return success ?
                Ok(new { success = true, message = "Estudiante eliminado sastifactoriamente" })
                : BadRequest(new { success = false, errorMessage });
        }
    }
}
