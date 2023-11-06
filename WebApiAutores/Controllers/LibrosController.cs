using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper _mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            _mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "ObtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            var libros = await context.Libros.Include(libroBD => libroBD.Comentarios)
                .Include(libroDB => libroDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);

            libros.AutoresLibros = libros.AutoresLibros.OrderBy(x => x.Orden).ToList();
            return _mapper.Map<LibroDTOConAutores>(libros);
        }

        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            if (libroCreacionDTO.AutoresIds == null)
            {
                return BadRequest("No se puede craer un libro sin autores");
            }
            var autoresIds = await context.Autores
                .Where(autorBD =>
                    libroCreacionDTO.AutoresIds.Contains(autorBD.Id)
                )
                .Select(x => x.Id).ToListAsync();

            if (libroCreacionDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = _mapper.Map<Libro>(libroCreacionDTO);

            AsignarOrdenAutores(libro);


            context.Add(libro);
            await context.SaveChangesAsync();

            var libroDTO = _mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libroDTO);

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDB = await context.Libros
                .Include(x => x.AutoresLibros)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libroDB == null)
            {
                return NotFound();
            }

            libroDB = _mapper.Map(libroCreacionDTO, libroDB);
            AsignarOrdenAutores(libroDB);

            await context.SaveChangesAsync();

            return NoContent();
        }

        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }
    }
}

