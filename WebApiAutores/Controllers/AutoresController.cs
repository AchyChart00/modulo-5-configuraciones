using Microsoft.AspNetCore.Mvc;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Autor>> Get()
        {
            return new List<Autor>()
            {
                new Autor()
                {
                    id = 1,
                    Nombre = "Jacob"
                },
                new Autor()
                {
                    id = 2,
                    Nombre = "Test1"
                }
            }; 
        }

        [HttpPost]
        public async Task<ActionResult> Post()
        {

        }
    }
}
