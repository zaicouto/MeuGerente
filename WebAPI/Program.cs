using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Modules.Orders.Application.Behaviors;
using Modules.Orders.Domain.Interfaces;
using Modules.Orders.Infrastructure.Persistence;
using Modules.Orders.Infrastructure.Persistence.Seed;
using Modules.Orders.Infrastructure.Repositories;
using Modules.Users.Domain.Interfaces;
using Modules.Users.Infrastructure.Persistence;
using Modules.Users.Infrastructure.Persistence.Seed;
using Modules.Users.Infrastructure.Repositories;
using Modules.Users.Infrastructure.Security;
using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Shared.Domain.Exceptions;
using Shared.Domain.Interfaces;
using Shared.Infrastructure.Context;
using System.Net;
using System.Reflection;
using System.Text;
using WebAPI.Infrastructure.Middlewares;

string? appName = Assembly.GetExecutingAssembly().GetName().Name;

string? sysFromEmail = Environment.GetEnvironmentVariable("SYSTEM_FROM_EMAIL");
if (string.IsNullOrEmpty(sysFromEmail))
{
    throw new InvalidOperationException("SYSTEM_FROM_EMAIL environment variable is not set.");
}

string? sysToEmail = Environment.GetEnvironmentVariable("SYSTEM_TO_EMAIL");
if (string.IsNullOrEmpty(sysToEmail))
{
    throw new InvalidOperationException("SYSTEM_TO_EMAIL environment variable is not set.");
}

string? sysEmailServer = Environment.GetEnvironmentVariable("SYSTEM_EMAIL_SERVER");
if (string.IsNullOrEmpty(sysEmailServer))
{
    throw new InvalidOperationException("SYSTEM_EMAIL_SERVER environment variable is not set.");
}

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", appName)
    .Enrich.WithProperty(
        "Environment",
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
    )
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
    )
    .WriteTo.File(
        new CompactJsonFormatter(),
        "logs/log-.json",
        rollingInterval: RollingInterval.Day
    )
    .WriteTo.Email(
        from: sysFromEmail,
        to: sysToEmail,
        host: sysEmailServer,
        credentials: new NetworkCredential(
            Environment.GetEnvironmentVariable("SYSTEM_EMAIL_USER"),
            Environment.GetEnvironmentVariable("SYSTEM_EMAIL_PASSWORD")
        ),
        restrictedToMinimumLevel: LogEventLevel.Error
    )
    .CreateLogger();

try
{
    Log.Information($"Starting {appName}...");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    string? connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");

#if DEBUG
    Console.WriteLine("Connection string: " + connectionString);
#endif

    // Add services to the container.

    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        string? devWebsiteUrl = Environment.GetEnvironmentVariable("DEV_WEBSITE_URL");
        if (string.IsNullOrEmpty(devWebsiteUrl))
        {
            throw new InvalidOperationException("DEV_WEBSITE_URL environment variable is not set.");
        }

        c.SwaggerDoc(
            "v1",
            new OpenApiInfo
            {
                Title = "Meu Gerente - Core API",
                Version = "v1",
                Description = "Documentação de Meu Gerente - Core API com Swagger",
                Contact = new OpenApiContact
                {
                    Name = Environment.GetEnvironmentVariable("DEV_NAME"),
                    Email = Environment.GetEnvironmentVariable("DEV_EMAIL"),
                    Url = new Uri(devWebsiteUrl),
                },
            }
        );

        string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (!File.Exists(xmlPath))
        {
            xmlPath = Path.Combine(AppContext.BaseDirectory, "bin/Debug/net8.0", xmlFile);
        }

        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }

        Console.WriteLine("Path of XML: " + xmlPath);
        c.EnableAnnotations();
    });

    builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));

    string? mongoDb = Environment.GetEnvironmentVariable("MONGO_INITDB_DATABASE");
    if (string.IsNullOrEmpty(mongoDb))
    {
        throw new InvalidOperationException(
            "MONGO_INITDB_DATABASE environment variable is not set."
        );
    }

    builder.Services.AddSingleton(sp => new OrdersDbContext(
        sp.GetRequiredService<IMongoClient>(),
        mongoDb
    ));

    builder.Services.AddSingleton(sp => new AuthDbContext(
        sp.GetRequiredService<IMongoClient>(),
        mongoDb
    ));

    builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    builder.Services.AddScoped<IAuthRepository, AuthRepository>();

    builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddScoped<ITenantContext, TenantContext>();

    IEnumerable<Assembly> modules = AppDomain
        .CurrentDomain.GetAssemblies()
        .Where(a => a.FullName!.StartsWith("Modules"));

    foreach (Assembly? module in modules)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(module));
        builder.Services.AddValidatorsFromAssembly(module);
    }

    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    builder
        .Services.AddHealthChecks()
        .AddCheck("api_alive", () => HealthCheckResult.Healthy("API is running"))
        .AddMongoDb(
            sp => sp.GetRequiredService<IMongoClient>(),
            name: "mongodb",
            timeout: TimeSpan.FromSeconds(5),
            tags: ["db", "nosql"]
        );

    builder
        .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            string? jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT_KEY environment variable is not set.");
            }

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            };
        });

    WebApplication app = builder.Build();

    // Populate with test data ONLY if the environment is Dev or Test
    if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Testing")
    {
        using IServiceScope scope = app.Services.CreateScope();
        OrdersDbContext dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

        // Populate fake data
        await OrdersDbSeeder.SeedAsync(dbContext);

        AuthDbContext authDbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        await AuthDbSeeder.SeedAsync(authDbContext);
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Meu Gerente - Core API v1");
        });
    }

    app.UseHttpsRedirection();

    app.UseMiddleware<TenantMiddleware>();

    app.UseMiddleware<ValidationExceptionMiddleware>();
    app.UseMiddleware<WebExceptionHandlingMiddleware>(Log.Logger);

    app.UseMiddleware<RequestLoggingMiddleware>();

    // Middleware customizado para capturar exceções de debug.
    app.Use(
        async (context, next) =>
        {
            try
            {
                await next.Invoke();
            }
            catch (DebugException ex)
            {
                Console.WriteLine(ex.ToString());

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(ex.ToString());
            }
        }
    );

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHealthChecks("/healthcheck");
    app.MapControllers();

    app.Run();

    Log.Information($"{appName} finished.");
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occurred during the execution of the program.");
}
finally
{
    Log.CloseAndFlush();
}
