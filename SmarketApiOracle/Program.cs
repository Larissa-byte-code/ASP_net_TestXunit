using Microsoft.EntityFrameworkCore;
using SmarketApiOracle.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmarketApiOracle.Services;
using SmarketApiOracle.Middleware;

namespace SmarketApiOracle
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Récupération de la clé JWT (évite null)
            var jwtKey = builder.Configuration["Jwt:Key"] ?? "DefaultSecretKey";

            // Connexion Oracle
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseOracle(builder.Configuration.GetConnectionString("OracleDb")));

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

            // Authentification JWT
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "SmarketApiOracle",
                        ValidAudience = "SmarketApiOracleUsers",
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey)
                        )
                    };
                });

            builder.Services.AddAuthorization();

            // Services DI
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<ClientService>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<SellerService>();
            builder.Services.AddScoped<SellingService>();
            builder.Services.AddScoped<AuthService>();

            // Contrôleurs + Swagger
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Garde PascalCase pour correspondre au modèle C#
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            builder.Services.AddOpenApi();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            // Activer CORS
            app.UseCors("AllowAngular");

            // Middleware personnalisé
            app.UseMiddleware<ErrorHandlingMiddleware>();

            // Authentification + Autorisation
            app.UseAuthentication();
            app.UseAuthorization();

            // Mappe les contrôleurs REST automatiquement
            app.MapControllers();

            app.Run();
        }
    }
}
