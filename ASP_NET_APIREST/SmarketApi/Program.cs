using Microsoft.EntityFrameworkCore;
using SmarketApi.Data;

namespace SmarketApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Ajout du DbContext avec la chaîne de connexion
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Ajout de CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200") // ton front Angular
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            // Add services to the container
            builder.Services.AddControllers(); // pour API REST
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            // Activer CORS
            app.UseCors("AllowAngular");

            app.UseAuthorization();

            // Mappe les contrôleurs REST automatiquement
            app.MapControllers();

            app.Run();
        }
    }
}



/*using Microsoft.EntityFrameworkCore;
using SmarketApi.Data;

namespace SmarketApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Ajout du DbContext avec la chaîne de connexion
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add services to the container
            builder.Services.AddControllers(); // pour API REST
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            // Mappe les contrôleurs REST automatiquement
            app.MapControllers();

            app.Run();
        }
    }
}
*/