using System.Reflection;
using System.Text.Json.Serialization;
using CoreAPI.Extensions;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfigureSerilog();

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddCustomSwagger();
builder.Services.AddJwtAuthentication();
builder.Services.AddCustomHealthChecks();
builder.Services.AddMongoDatabase(builder.Configuration);

IEnumerable<Assembly> modules = AppDomain
    .CurrentDomain.GetAssemblies()
    .Where(a => a.FullName!.StartsWith("Modules"));

builder.Services.AddCoreServices(modules);

WebApplication app = builder.Build();

try
{
    Log.Information("Iniciando aplicação...");

    if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Testing")
    {
        await app.Services.SeedTestDataAsync();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Meu Gerente - Core API v1");
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCustomMiddlewares();

    app.MapHealthChecks("/healthcheck");
    app.MapControllers();

    app.Run();

    Log.Information("Aplicação finalizada com sucesso.");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro fatal ao iniciar a aplicação.");
}
finally
{
    Log.CloseAndFlush();
}
