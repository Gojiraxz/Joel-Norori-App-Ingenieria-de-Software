using Azure.Identity;
using Joel_Norori_App_Ingenieria_de_Software.DTOS;
using Joel_Norori_App_Ingenieria_de_Software.Models;
using Joel_Norori_App_Ingenieria_de_Software.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Services
{
    public class CursoService(ICursoRepository repo) : ICursoService
    {
        public async Task<(bool Succes, string? ErrorMessage)> CreateAsync(SaveCursoDto dto)
        {
            if (await repo.NombreExisteAsync(dto.Nombre.Trim()))
                return (false, "El nombre del curso ya existe");
            await repo.CreateAsync(new Curso
            {
                Nombre= dto.Nombre,
                Descripcion= dto.Descripcion

            });
            return (true, null);
        }

        public async Task<(bool Succes, string? ErrorMessage)> DeleteAsync(int id)
        {
            if(await repo.HasAnyEstudianteAsync(id)) 
                return (false, "No se puede eliminar el curso debido a que tiene estudiantes");
            var deleted=await repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Curso no encontrado");
        }

        public async Task<IReadOnlyList<CursoTableDto>> GetAllAsync()
        {
            var curso = await repo.GetAllAsync();
            return curso.Select(c => new CursoTableDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,


            }).ToList();
        }

        public async Task<SaveCursoDto?> GetByIdAsync(int id)
        {
            var cursos= await repo.GetByIdAsync(id);
            if(cursos == null) return null;
            return new SaveCursoDto
            {
               Id= cursos.Id,
               Nombre= cursos.Nombre,
               Descripcion= cursos.Descripcion,
            };
        }

        public Task<DataTableResponseDto<CursoTableDto>> GetPagedAsync(DatoaTablesRequestDto request)
           =>repo.GetPagedAsync(request);
        

        public async Task<(bool Succes, string? ErrorMessage)> UpdateAync(SaveCursoDto dto)
        {
            if (!dto.Id.HasValue)
               return(false, "Id Invalido");

            var cursos = await repo.GetByIdAsync(dto.Id.Value);
            if (cursos == null) 
                return (false, "Curso no encontrado");

            if (await repo.NombreExisteAsync(dto.Nombre.Trim(), dto.Id))
                return (false, "Ya existe un curso con ese nombre");
            cursos.Nombre = dto.Nombre;
            cursos.Descripcion = dto.Descripcion;
            await repo.UpdateAsync(cursos);
            return (true, null);
        }
    }
}
