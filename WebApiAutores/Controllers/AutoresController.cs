﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper _mapper;

        public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            _mapper = mapper;
        }
        
        [HttpGet]// api/autores
        public async Task<List<Autor>> Get()
        {
            return await context.Autores.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Autor>> Get(int id)
        {
            var autor =  await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            if (autor == null)
            {
                return NotFound();
            }

            return autor;
        }
        
        [HttpGet("{nombre}")] // api/autores/{id}   api/autores/1
        public async Task<ActionResult<Autor>> Get([FromRoute]string nombre)
        {
            var autor =  await context.Autores.FirstOrDefaultAsync(x => x.Nombre == nombre);
            if (autor == null)
            {
                return NotFound();
            }

            return autor;
        }
        
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]AutorCreacionDTO autorCreacionDto)
        {
            //validación personalizada en el método de nuestro controlador
            var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x=> x.Nombre == autorCreacionDto.Nombre);

            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autorCreacionDto.Nombre}");
            }

            // var autor = new Autor()
            // {
            //     Nombre = autorCreacionDto.Nombre
            // };
            var autor = _mapper.Map<Autor>(autorCreacionDto);
            context.Add(autor);
            //Para guardar los cambios de manera asincrona
            await context.SaveChangesAsync();
            return Ok();
        }
        //se agrega un parametro de ruta por medio de llaves y ponemos que sea un int
        [HttpPut("{id:int}")]// api/autores/1
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            if (autor.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de la URL");
            }

            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor(){Id = id});
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
