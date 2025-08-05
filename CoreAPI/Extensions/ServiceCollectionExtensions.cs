using System.Reflection;
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

namespace CoreAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(
        this IServiceCollection services,
        IEnumerable<Assembly> modules
    )
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IRolesContext, RolesContext>();
        services.AddScoped<ITenantContext, TenantContext>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies([.. modules]));
        services.AddValidatorsFromAssemblies(modules);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddAutoMapper(cfg => cfg.AddMaps(modules));

        return services;
    }
}
