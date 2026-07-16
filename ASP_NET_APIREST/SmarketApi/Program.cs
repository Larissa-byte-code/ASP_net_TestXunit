using Microsoft.EntityFrameworkCore;
using SmarketApi.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SmarketApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Récupération de la clé JWT (évite null)
            var jwtKey = builder.Configuration["Jwt:Key"] ?? "DefaultSecretKey";

            // Ajout du DbContext avec la chaîne de connexion
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Ajout de CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            // Ajout de l’authentification JWT
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "SmarketApi",
                        ValidAudience = "SmarketApiUsers",
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey)
                        )
                    };
                });

            builder.Services.AddAuthorization();

            // Add services to the container
            builder.Services.AddControllers();
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

            // Activer Authentification + Autorisation
            app.UseAuthentication();
            app.UseAuthorization();

            // Mappe les contrôleurs REST automatiquement
            app.MapControllers();

            app.Run();
        }
    }
}
