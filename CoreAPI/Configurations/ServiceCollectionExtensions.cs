using FluentValidation;
using MediatR;
using Modules.Orders.Application.Behaviors;
using Modules.Orders.Domain.Interfaces;
using Modules.Orders.Infrastructure.Repositories;
using Modules.Users.Domain.Interfaces;
using Modules.Users.Infrastructure.Repositories;
using Modules.Users.Infrastructure.Security;
using Shared.Domain.Interfaces;
using Shared.Infrastructure.Contexts;
using System.Reflection;

namespace CoreAPI.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies
    )
    {
        // Repositórios
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();

        // Criptografia de Senhas
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        // Contextos HTTP
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IUserContext, UserContext>();

        // MediatR e Validação
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies([.. assemblies]));
        services.AddValidatorsFromAssemblies(assemblies);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // AutoMapper
        services.AddAutoMapper(cfg => cfg.AddMaps(assemblies));
        return services;
    }
}
