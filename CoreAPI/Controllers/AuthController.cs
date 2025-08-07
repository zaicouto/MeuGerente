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
using Swashbuckle.AspNetCore.Annotations;

namespace CoreAPI.Controllers;

[Route("api/auth")]
public class AuthController(
    IMediator mediator,
    IPasswordHasher hasher,
    ILogger<AuthController> logger
) : ApiControllerBase
{
    /// <summary>
    /// Realiza login de usuário e retorna um JWT válido.
    /// </summary>
    /// <param name="dto">Credenciais de login (email e senha).</param>
    /// <returns>JWT e TenantId em caso de sucesso.</returns>
    /// <response code="200">Login bem-sucedido. Retorna o token JWT.</response>
    /// <response code="401">Credenciais inválidas.</response>
    /// <response code="404">Usuário não encontrado.</response>
    /// <response code="500">Erro interno do servidor.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    [SwaggerResponse(StatusCodes.Status200OK, "Login realizado com sucesso.")]
    [SwaggerOperation(
        Summary = "Realiza login de usuário.",
        Description = "Recebe email e senha, valida as credenciais e retorna um JWT válido."
    )]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        string token;

#if DEBUG
        if (SuperAdminCreds.IsValid(dto.Email, dto.Password))
        {
            string adminTenantId = SuperAdminCreds.TenantId;
            token = JwtHelper.GenerateJwtToken(dto.Email, adminTenantId, UserRoles.SuperAdmin);
            logger.LogInformation("SuperAdmin login successful: {Email}.", dto.Email);
            return Ok(new LoginResponseDto(adminTenantId, token));
        }
#endif

        User user = await mediator.Send(new GetUserByEmailQuery(dto.Email));
        if (!user.VerifyPassword(dto.Password, hasher))
        {
            throw new UnauthorizedException();
        }

        token = JwtHelper.GenerateJwtToken(dto.Email, user.TenantId);
        logger.LogInformation("Login successful: {Email}.", dto.Email);
        return Ok(new LoginResponseDto(user.TenantId, token));
    }
}
