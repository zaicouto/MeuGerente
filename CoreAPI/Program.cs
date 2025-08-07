using CoreAPI.Extensions;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configura o Serilog para logging
builder.ConfigureSerilog();

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Serviços personalizados
builder.Services.AddCustomSwagger();
builder.Services.AddJwtAuthentication();
builder.Services.AddCustomHealthChecks();
builder.Services.AddMongoDatabase(builder.Configuration);

// Carrega os módulos dinamicamente a partir do AppDomain atual
IEnumerable<Assembly> modules = AppDomain
    .CurrentDomain.GetAssemblies()
    .Where(a => a.FullName!.StartsWith("Modules"));

builder.Services.AddCoreServices(modules);

WebApplication app = builder.Build();

try
{
    Log.Information("Iniciando aplicação...");

    // Passos extras para configurar o ambiente de desenvolvimento
    if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Testing")
    {
        await app.Services.SeedTestDataAsync();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Meu Gerente - Core API v1");
        });
    }

    // Congigura os middlewares
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCustomMiddlewares();

    // Configura o roteamento
    app.MapHealthChecks("/healthcheck");
    app.MapControllers();

    // Inicia a aplicação
    Log.Information("Aplicação iniciada com sucesso.");
    app.Run();

    Log.Information("Aplicação finalizada com sucesso.");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro fatal ao iniciar a aplicação.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
