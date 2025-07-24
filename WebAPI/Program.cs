using FluentValidation;
using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Modules.Orders.Application.Behaviors;
using Modules.Orders.Application.Commands;
using Modules.Orders.Application.Validators;
using Modules.Orders.Domain.Interfaces;
using Modules.Orders.Infrastructure.Persistence;
using Modules.Orders.Infrastructure.Persistence.Seed;
using Modules.Orders.Infrastructure.Repositories;
using MongoDB.Driver;
using System.Reflection;
using WebAPI.Infrastructure.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

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
            Description = "Documentação de Meu Gerente Core API com Swagger",
            Contact = new OpenApiContact
            {
                Name = Environment.GetEnvironmentVariable("DEV_NAME"),
                Email = Environment.GetEnvironmentVariable("DEV_EMAIL"),
                Url = new Uri(Environment.GetEnvironmentVariable("DEV_URL")!),
            },
        }
    );

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
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

var app = builder.Build();

// Populate with test data ONLY if the environment is Dev or Test
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Testing")
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

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
app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseAuthorization();
app.MapHealthChecks("/healthcheck");
app.MapControllers();

app.Run();
