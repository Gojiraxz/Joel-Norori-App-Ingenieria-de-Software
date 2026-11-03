using Microsoft.AspNetCore.Mvc;
using Joel_Norori_App_Ingenieria_de_Software.Services;
using Microsoft.AspNetCore.Authorization;

using Joel_Norori_App_Ingenieria_de_Software.DTOS;

namespace Joel_Norori_App_Ingenieria_de_Software.Controllers
{
    public class CursoController(ICursoService _cursoService) : Controller
    {
        [Authorize(Policy = "curso.ver")]
        public async Task<IActionResult> GetData(DatoaTablesRequestDto request)
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
                Ok(new { succes = true, message = "Curso Eliminado satisfactoriamente" })
                : BadRequest(new { succes = false, message = errormessage });
        }

        [Authorize(Policy = "curso.crear")]
        [HttpPost]
        public async  Task<IActionResult> Create (SaveCursoDto dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(new { succes = false, message = "Error en los datos" });
                
            var (succes, errorMessage) = await _cursoService.CreateAsync(dto);
            return succes ?
               Ok(new { succes = true, message = "Curso Creado satisfactoriamente" })
               : BadRequest(new { succes = false, errorMessage });
        }

        [Authorize(Policy = "curso.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update (SaveCursoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { succes = false, message = "Error en los datos" });
              

            var (succes, errorMessage) = await _cursoService.UpdateAync(dto);
            return succes ?
               Ok(new { succes = true, message = "Curso Actualizado satisfactoriamente" })
               : BadRequest(new { succes = false, errorMessage });
        }


    }
}

