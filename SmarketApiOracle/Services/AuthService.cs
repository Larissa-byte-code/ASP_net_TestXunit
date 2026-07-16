using Microsoft.IdentityModel.Tokens;
using SmarketApiOracle.Data;
using SmarketApiOracle.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmarketApiOracle.Services
{
    public class AuthService
    {
        //_db pour accéder aux tables et _config pour lire la clé JWT.
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public string Register(string username, string email, string password, string role = "User")
        {
            var user = new TblUser
            {
                UserName = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role
            };

            _db.TblUser.Add(user);
            _db.SaveChanges();

            return "Utilisateur créé !";
        }

        public string? Login(string email, string password)
        {
            var user = _db.TblUser.SingleOrDefault(u => u.Email == email);
            if (user == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            /*
            Définit les claims (infos stockées dans le JWT).

            NameIdentifier → ID utilisateur.

            Name → nom d’utilisateur.

            Role → rôle (User/Admin).
            */
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty)
            };
            //generation du jeton JWT
            //Récupère la clé secrète depuis appsettings.json.

            //Prépare les credentials pour signer le token avec HMAC SHA256.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "SmarketApiOracle",
                audience: "SmarketApiOracleUsers",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);
                //Sérialise le JWT en chaîne de caractères.

                //Retourne le token au contrôleur.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
