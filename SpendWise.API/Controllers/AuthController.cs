using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SpendWise.API.Data;
using SpendWise.API.DTOs.Auth;
using SpendWise.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpendWise.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                return BadRequest("Email already exists.");
            }

            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                UserName = dto.Email,
                Email = dto.Email,
                MonthlyIncome = dto.MonthlyIncome
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, "User");

            var currentDate = DateTime.Now;

            var recommendedBudgets = new List<Budget>
            {
                new Budget { Name = "Food", Percentage = 25, Amount = dto.MonthlyIncome * 25 / 100, Month = currentDate.Month, Year = currentDate.Year, UserId = user.Id },
                new Budget { Name = "Transport", Percentage = 10, Amount = dto.MonthlyIncome * 10 / 100, Month = currentDate.Month, Year = currentDate.Year, UserId = user.Id },
                new Budget { Name = "Bills & Subscriptions", Percentage = 20, Amount = dto.MonthlyIncome * 20 / 100, Month = currentDate.Month, Year = currentDate.Year, UserId = user.Id },
                new Budget { Name = "Savings", Percentage = 20, Amount = dto.MonthlyIncome * 20 / 100, Month = currentDate.Month, Year = currentDate.Year, UserId = user.Id },
                new Budget { Name = "Unexpected", Percentage = 10, Amount = dto.MonthlyIncome * 10 / 100, Month = currentDate.Month, Year = currentDate.Year, UserId = user.Id },
                new Budget { Name = "Fun", Percentage = 15, Amount = dto.MonthlyIncome * 15 / 100, Month = currentDate.Month, Year = currentDate.Year, UserId = user.Id }
            };

            _context.Budgets.AddRange(recommendedBudgets);
            await _context.SaveChangesAsync();

            var token = await GenerateJwtToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                FullName = user.FullName
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!isPasswordValid)
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = await GenerateJwtToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                FullName = user.FullName
            });
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}