using Microsoft.AspNetCore.Mvc;
using Joel_Norori_App_Ingenieria_de_Software.Services;
using Microsoft.AspNetCore.Authorization;

using Joel_Norori_App_Ingenieria_de_Software.DTOS;

namespace Joel_Norori_App_Ingenieria_de_Software.Controllers
{
    public class CursoController(ICursoService _cursoService) : Controller
    {
        [Authorize(Policy ="curso.ver")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "curso.ver")]
        public async Task<IActionResult> GetData([FromForm] DatoaTablesRequestDto request)
        {
            var result= await _cursoService.GetPagedAsync(request);
            return Ok(result);
        }


        [Authorize(Policy = "curso.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result =  await _cursoService.GetByIdAsync(id);
            if(result == null)
                return NotFound(new {message="Curso no encontrado"});
            return Ok(result);
        }

        [Authorize(Policy = "curso.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete (int id)
        {
            var (success, errormessage) = await _cursoService.DeleteAsync(id);
            return success ?
                Ok(new { success = true, message = "Curso Eliminado satisfactoriamente" })
                : BadRequest(new { success = false, message = errormessage });
        }

        [Authorize(Policy = "curso.crear")]
        [HttpPost]
        public async  Task<IActionResult> Create ([FromBody] SaveCursoDto dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Error en los datos" });
                
            var (success, errorMessage) = await _cursoService.CreateAsync(dto);
            return success ?
               Ok(new { success = true, message = "Curso Creado satisfactoriamente" })
               : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "curso.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update ([FromBody] SaveCursoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Error en los datos" });
              

            var (success, errorMessage) = await _cursoService.UpdateAync(dto);
            return success ?
               Ok(new { success = true, message = "Curso Actualizado satisfactoriamente" })
               : BadRequest(new { success = false, errorMessage });
        }


    }
}

