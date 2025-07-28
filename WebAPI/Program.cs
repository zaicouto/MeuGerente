using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Modules.Orders.Application.Behaviors;
using Modules.Orders.Application.Commands;
using Modules.Orders.Application.Validators;
using Modules.Orders.Domain.Interfaces;
using Modules.Orders.Infrastructure.Persistence;
using Modules.Orders.Infrastructure.Persistence.Seed;
using Modules.Orders.Infrastructure.Repositories;
using Modules.Users.Domain.Interfaces;
using Modules.Users.Infrastructure.Repositories;
using Modules.Users.Infrastructure.Security;
using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Net;
using System.Reflection;
using System.Text;
using WebAPI.Infrastructure.Middlewares;

string? appName = Assembly.GetExecutingAssembly().GetName().Name;

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
        from: Environment.GetEnvironmentVariable("SYSTEM_FROM_EMAIL")!,
        to: Environment.GetEnvironmentVariable("SYSTEM_TO_EMAIL")!,
        host: Environment.GetEnvironmentVariable("SYSTEM_EMAIL_SERVER")!,
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

    string? connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

#if DEBUG
    Console.WriteLine("Connection string: " + connectionString);
#endif

    // Add services to the container.

    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
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
                    Url = new Uri(Environment.GetEnvironmentVariable("DEV_WEBSITE_URL")!),
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

    builder.Services.AddSingleton(sp => new OrdersDbContext(
        sp.GetRequiredService<IMongoClient>(),
        "meuGerente"
    ));

    builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    builder.Services.AddScoped<IAuthRepository, AuthRepository>();

    builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly)
    );
    builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderCommandValidator>();
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
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!)
                ),
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

    app.UseMiddleware<RequestLoggingMiddleware>();
    app.UseMiddleware<ValidationExceptionMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>(Log.Logger);

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
