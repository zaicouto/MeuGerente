using FluentValidation;
//using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Modules.Orders.Application.Behaviors;
using Modules.Orders.Application.Commands;
using Modules.Orders.Application.Validators;
using Modules.Orders.Domain.Interfaces;
using Modules.Orders.Infrastructure.Persistence;
using Modules.Orders.Infrastructure.Persistence.Seed;
using Modules.Orders.Infrastructure.Repositories;
using WebAPI.Infrastructure.Middlewares;

var builder = WebApplication.CreateBuilder(args);

#if DEBUG
Console.WriteLine("Connection string: " + builder.Configuration.GetConnectionString("OrdersDb"));
#endif

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO: use environment variable for connection string
builder.Services.AddDbContext<OrdersDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("OrdersDb"),
        new MySqlServerVersion(new Version(11, 8, 2))
    );

    if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Testing")
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly)
);
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

//builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder
    .Services.AddHealthChecks()
    .AddCheck("api_alive", () => HealthCheckResult.Healthy("API is running"))
    .AddMySql(
        builder.Configuration.GetConnectionString("OrdersDb")!,
        name: "mariadb",
        tags: ["db", "sql", "mariadb"]
    );

var app = builder.Build();

// Populate with test data ONLY if the environment is Dev or Test
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Testing")
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

    // Execute pending migrations
    dbContext.Database.Migrate();

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
