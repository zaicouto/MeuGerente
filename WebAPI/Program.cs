using FluentValidation;
using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Modules.Orders.Application.Behaviors;
using Modules.Orders.Application.Commands;
using Modules.Orders.Application.Validators;
using Modules.Orders.Domain.Interfaces;
using Modules.Orders.Infrastructure.Persistence;
using Modules.Orders.Infrastructure.Persistence.Seed;
using Modules.Orders.Infrastructure.Repositories;
using MongoDB.Driver;
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
builder.Services.AddSwaggerGen();

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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseAuthorization();
app.MapHealthChecks("/healthcheck");
app.MapControllers();

app.Run();
