using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores
{
    public class ApplicationDbContext : DbContext
    {
        //Generamos un constructor para pasar el connection string
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        //de esta forma generamos las tablas
        public DbSet<Autor> Autores { get; set; }
        
    }
}
