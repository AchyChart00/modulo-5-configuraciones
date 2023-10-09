using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Controllers;
using WebApiAutores.Servicios;

namespace WebApiAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            
            // Add services to the container.
            services.AddControllers()
                .AddJsonOptions(
                    x=> x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
                    );

            //Agregamos la configuración de nuestro DBContext
            services.AddDbContext<ApplicationDbContext>(opt=>
            opt.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
            
            services.AddTransient<IServicio, ServicioA>();

            services.AddTransient<ServicioTransient>();
            services.AddScoped<ServicioScoped>();
            services.AddSingleton<ServicioSingleton>();
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.Use(async (contexto, siguiente) =>
            {
                using (var ms = new MemoryStream())
                {
                    var cuerpoOriginalRespuesta = contexto.Response.Body;
                    contexto.Response.Body = ms;

                    await siguiente.Invoke();

                    ms.Seek(0, SeekOrigin.Begin);
                    string respuesta = new StreamReader(ms).ReadToEnd();
                    ms.Seek(0, SeekOrigin.Begin);

                    await ms.CopyToAsync(cuerpoOriginalRespuesta);

                    contexto.Response.Body = cuerpoOriginalRespuesta;
                    
                    logger.LogInformation(respuesta);
                    
                }
            });
            
            //bifurcación de nuestra tubería de procesos, 
            //Nos permite asignar una ruta específica para el middleware. 
            app.Map("/ruta1", app =>
            {
                app.Run(async contexto =>
                {
                    await contexto.Response.WriteAsync("Estoy interceptando la tubería");
                });
            });
            
            //Creación de un middleware
            // app.Run(async contexto =>
            // {
            //     await contexto.Response.WriteAsync("Estoy interceptando la tubería");
            // });
            
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();   

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); 
            });
        }
    }
}
