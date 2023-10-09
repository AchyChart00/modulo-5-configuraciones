using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers
{
    [ApiController]
    //[Route("api/[controller]")] api/Autores
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ServicioTransient _servicioTransient;
        private readonly ServicioScoped _servicioScoped;
        private readonly ServicioSingleton _servicioSingleton;
        private readonly IServicio _servicio;
        private readonly ILogger<AutoresController> _logger;

        public AutoresController(ApplicationDbContext context, 
            ServicioTransient servicioTransient, 
            ServicioScoped servicioScoped, 
            ServicioSingleton servicioSingleton, 
            IServicio servicio,
            ILogger<AutoresController> logger) 
        {
            this.context = context;
            _servicioTransient = servicioTransient;
            _servicioScoped = servicioScoped;
            _servicioSingleton = servicioSingleton;
            _servicio = servicio;
            _logger = logger;
        }

        [HttpGet("GUID")]
        public ActionResult ObtenerGuids()
        {
            return Ok( new
            {
                AutoresControllerTransient = _servicioTransient.Guid,
                Servicio_Transient = _servicio.ObtenerTransient(),
                AutoresControllerScoped = _servicioScoped.Guid,
                Servicio_Scoped = _servicio.ObtenerScoped(),
                AutoresControllerSingleton = _servicioSingleton.Guid,
                Servicio_Singleton = _servicio.ObtenerSingleton(),
            });
        }
        
        [HttpGet]// api/autores
        [HttpGet("listado")]// api/autores/listado
        [HttpGet("/listado")] //listado
        public async Task<List<Autor>> Get()
        {
            _logger.LogInformation("Estamos obteniendo los autores");
            return await context.Autores.Include(x => x.Libro).ToListAsync();
        }

        [HttpGet("Primero")]
        public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int miValor, [FromQuery] string nombre)
        {
            return await context.Autores.FirstOrDefaultAsync();
        }
        
        [HttpGet("Primero2")]
        public ActionResult<Autor> PrimerAutor2()
        {
            return new Autor() { Nombre = "Inventado"};
        }

        //[HttpGet("{id}")] // api/autores/{id}   api/autores/1
        //[HttpGet("{id:int}/{param2}")] // api/autores/{id}   api/autores/1
        //[HttpGet("{id:int}/{param2?}")]
        [HttpGet("{id:int}/{param2=mx}")]
        public async Task<ActionResult<Autor>> Get(int id, string param2)
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
        public async Task<ActionResult> Post([FromBody]Autor autor)
        {
            //validación personalizada en el método de nuestro controlador
            var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x=> x.Nombre == autor.Nombre);

            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autor.Nombre}");
            }
            
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
