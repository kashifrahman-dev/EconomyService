using EconomyService.Data;
using EconomyService.Models;
using Microsoft.AspNetCore.Mvc;
using EconomyService.Interfaces;
using EconomyService.DTOs.Auth;
using Microsoft.EntityFrameworkCore;

namespace EconomyService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly ApplicationDbContext _context;

        public AuthController(ITokenService tokenService, IPasswordService passwordService, ApplicationDbContext context)
        {
            _tokenService = tokenService;
            _passwordService = passwordService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (existingUser != null)
            {
                return BadRequest(new
                {
                    Message = "Email already exists."
                });
            }

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = _passwordService.HashPassword(request.Password),
                Role = "User"
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "User Registered Successfully."
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    Message = "Invalid email or password."
                });
            }

            var isPasswordValid = _passwordService.VerifyPassword(
                request.Password,
                user.PasswordHash);

            if (!isPasswordValid)
            {
                return Unauthorized(new
                {
                    Message = "Invalid email or password."
                });
            }

            var token = _tokenService.GenerateToken(
                user.Email,
                user.Role);

            return Ok(new LoginResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(60)
            });
        }
    }
}