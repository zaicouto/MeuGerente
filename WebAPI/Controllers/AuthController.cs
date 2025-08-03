using MediatR;
using Microsoft.AspNetCore.Mvc;
using Modules.Users.Application.DTOs;
using Modules.Users.Application.Queries;
using Modules.Users.Application.Services;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Interfaces;
using Shared.Core;
using Shared.Domain.Enums;
using Shared.Domain.Exceptions;

namespace WebAPI.Controllers;

[Route("api/auth")]
public class AuthController(IMediator mediator, IPasswordHasher hasher) : ApiControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        string token;

#if DEBUG
        if (SuperAdminCredentials.IsValid(dto.Email, dto.Password))
        {
            string adminTenantId = SuperAdminCredentials.TenantId;
            token = JwtService.GenerateJwtToken(dto.Email, adminTenantId, UserRoles.SuperAdmin);
            return Ok(new { adminTenantId, token });
        }
#endif

        User user = await mediator.Send(new GetUserByEmailQuery(dto.Email));
        if (!user.VerifyPassword(dto.Password, hasher))
        {
            throw new UnauthorizedException("Credenciais inválidas.");
        }

        token = JwtService.GenerateJwtToken(dto.Email, user.TenantId);
        return Ok(new { user.TenantId, token });
    }
}
