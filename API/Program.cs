using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Modules.Orders.Application.Commands;
using Modules.Orders.Application.Validators;
using Modules.Orders.Domain.Interfaces;
using Modules.Orders.Infrastructure.Persistence;
using Modules.Orders.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseMySql(
        // TODO: usar variável de ambiente para a string de conexão
        builder.Configuration.GetConnectionString("OrdersDb"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("OrdersDb"))
    )
);
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly)
);

builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
