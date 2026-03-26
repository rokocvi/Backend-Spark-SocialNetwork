using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Spark.API.Data;
using Spark.API.DTOs.Auth;
using Spark.API.Interfaces;
using Spark.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Spark.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> Register(RegisterDto dto)
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
                throw new Exception("Korisnik s tim emailom već postoji");

            var existingUsername = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (existingUsername != null)
                throw new Exception("Korisnik s tim usernameom već postoji");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                DisplayName = dto.DisplayName,
                Bio = dto.Bio
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.DisplayName
            };

        }

        public async Task<AuthResponseDto> Login(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);

            if(user == null)
                 throw new Exception("Neispravan email ili lozinka");


            var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if(!isPasswordValid)
                throw new Exception("Neispravan email ili lozinka");

            var token = GenerateJwtToken(user);

            return new AuthResponseDto { 
                Token = token,
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.DisplayName
             };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims su podaci koji ce biti zapisani unutar tokena
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username", user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(
                    int.Parse(jwtSettings["ExpirationInDays"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
