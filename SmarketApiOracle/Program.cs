using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SmarketApiOracle.Data;
using SmarketApiOracle.Services;
using SmarketApiOracle.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Connexion Oracle
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDb")));

// Swagger / OpenAPI
builder.Services.AddOpenApi();

// Ajouter les contrôleurs
builder.Services.AddControllers();

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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

// Enregistrer les services (DI)
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<SellerService>();
builder.Services.AddScoped<SellingService>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Middleware personnalisé
app.UseMiddleware<ErrorHandlingMiddleware>();

// IMPORTANT : Authentification AVANT Autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
