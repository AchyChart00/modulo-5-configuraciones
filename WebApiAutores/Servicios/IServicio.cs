namespace WebApiAutores.Servicios;

public interface IServicio
{
    public void Rectangulo();
}

public class ServicioA : IServicio
{
    public void Rectangulo()
    {
        throw new Exception();
    }
}

public class ServicioB : IServicio
{
    public void Rectangulo()
    {
        throw new Exception();
    }
}