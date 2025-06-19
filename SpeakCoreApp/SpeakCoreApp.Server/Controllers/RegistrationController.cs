using Microsoft.AspNetCore.Mvc;
using speakcore.API.Model;
using Microsoft.EntityFrameworkCore;
using speakcore.API.Common;
using SpeakCoreApp.Server.Common.Interfaces;
using SpeakCoreApp.Server.Common.Services;
using SpeakCoreApp.Server.DTOs;

namespace speakcore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly SpeakCoreDBContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly JwtService _jwtService;

        public RegistrationController(SpeakCoreDBContext context, IEmailService emailService, IConfiguration configuration, JwtService jwtService)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
            _jwtService = jwtService;
        }

        // POST /api/registrations
        [HttpPost("register")]
        public async Task<IActionResult> CreateRegistration([FromBody] RegistrationRequestViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if email already exists
            var existingUser = await _context.Registrations
                .FirstOrDefaultAsync(r => r.Email == request.Email);

            if (existingUser != null)
            {
                return Conflict(new { message = "Email already registered" });
            }

            string hashed = BCrypt.Net.BCrypt.HashPassword(request.Password);
            bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, hashed);

            if (!isValid)
                return Conflict(new { message = "Not a valid password" });

            var registration = new Registration
            {
                Key = Guid.NewGuid().ToString(),
                RegistrationId = GenerateRegistrationId(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = hashed,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow
            };

            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();

            // Send confirmation email
            await _emailService.SendConfirmationEmailAsync(
                registration.Email,
                registration.FirstName,
                registration.RegistrationId);

            return Ok(new
            {
                key = registration.Key,
                registrationId = registration.RegistrationId,
                firstName = registration.FirstName,
                lastName = registration.LastName,
                email = registration.Email
            });
        }

        // GET /api/registrations/{registrationId}
        [HttpGet("{registrationId}")]
        public async Task<IActionResult> GetRegistration(string registrationId)
        {
            var registration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.RegistrationId == registrationId);

            if (registration == null)
            {
                return NotFound(new { message = "Registration not found" });
            }

            return Ok(new
            {
                key = registration.Key,
                registrationId = registration.RegistrationId,
                firstName = registration.FirstName,
                lastName = registration.LastName,
                email = registration.Email,
                createdAt = registration.CreatedAt
            });
        }

        // DELETE /api/registrations/{registrationId}
        [HttpDelete("{registrationId}")]
        public async Task<IActionResult> DeleteRegistration(string registrationId)
        {
            var registration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.RegistrationId == registrationId);

            if (registration == null)
            {
                return NotFound(new { message = "Registration not found" });
            }

            _context.Registrations.Remove(registration);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration deleted successfully" });
        }

        // POST /api/registrations/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestViewModel request)
        {
            var user = await _context.Registrations.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return Unauthorized("Invalid email or password.");

            var token = _jwtService.GenerateJwtToken(user);

            return Ok(new
            {
                token,
                key = user.Key,
                registrationId = user.RegistrationId,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email
            });
        }

        private string GenerateRegistrationId()
        {
            return "REG" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + Random.Shared.Next(1000, 9999);
        }
    }
}
