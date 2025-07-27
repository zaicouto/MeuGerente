using MediatR;
using Microsoft.AspNetCore.Mvc;
using Modules.Users.Application.DTOs;
using Modules.Users.Application.Queries;
using Modules.Users.Application.Services;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Interfaces;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IMediator mediator, IPasswordHasher hasher) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            string token;

#if DEBUG
            if (dto.Email == "admin@admin.com" && dto.Password == "Pass@12345")
            {
                token = JwtService.GenerateJwtToken(dto.Email);
                return Ok(new { token });
            }
#endif

            User user = await mediator.Send(new GetUserByEmailQuery(dto.Email));
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            if (!user.VerifyPassword(dto.Password, hasher))
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            token = JwtService.GenerateJwtToken(dto.Email);
            return Ok(new { token });
        }
    }
}
