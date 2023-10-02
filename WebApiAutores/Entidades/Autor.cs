namespace WebApiAutores.Entidades
{
    //Esta entidad corresponde con una tabla de la base de datos
    public class Autor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<Libro> Libro { get; set; }
    }
}
