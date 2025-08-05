using System.Text.Json.Serialization;
using CoreAPI.Extensions;
using Modules.Orders.Infrastructure.Persistence;
using Modules.Users.Infrastructure.Persistence;
using MongoDB.Driver;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

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

string? connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
string? mongoDb = Environment.GetEnvironmentVariable("MONGO_INITDB_DATABASE");

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
builder.Services.AddSingleton(_ => new OrdersDbContext(
    new MongoClient(connectionString),
    mongoDb!
));
builder.Services.AddSingleton(_ => new AuthDbContext(new MongoClient(connectionString), mongoDb!));

IEnumerable<System.Reflection.Assembly> modules = AppDomain
    .CurrentDomain.GetAssemblies()
    .Where(a => a.FullName!.StartsWith("Modules"));
builder.Services.AddCoreServices(modules);

WebApplication app = builder.Build();

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
