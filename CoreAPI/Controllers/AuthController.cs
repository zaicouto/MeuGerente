using MediatR;
using Microsoft.AspNetCore.Mvc;
using Modules.Users.Application.DTOs;
using Modules.Users.Application.Queries;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Interfaces;
using Modules.Users.Infrastructure.Security;
using Shared.Domain.Abstractions;
using Shared.Domain.Enums;
using Shared.Domain.Exceptions;

namespace CoreAPI.Controllers;

[Route("api/auth")]
public class AuthController(IMediator mediator, IPasswordHasher hasher) : ApiControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        string token;

#if DEBUG
        if (SuperAdminCreds.IsValid(dto.Email, dto.Password))
        {
            string adminTenantId = SuperAdminCreds.TenantId;
            token = JwtHelper.GenerateJwtToken(dto.Email, adminTenantId, UserRoles.SuperAdmin);
            return Ok(new { adminTenantId, token });
        }
#endif

        User user = await mediator.Send(new GetUserByEmailQuery(dto.Email));
        if (!user.VerifyPassword(dto.Password, hasher))
        {
            throw new UnauthorizedException();
        }

        token = JwtHelper.GenerateJwtToken(dto.Email, user.TenantId);
        return Ok(new { user.TenantId, token });
    }
}
