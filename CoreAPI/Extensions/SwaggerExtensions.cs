using Microsoft.OpenApi.Models;
using System.Reflection;

namespace CoreAPI.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            string devWebsiteUrl =
                Environment.GetEnvironmentVariable("DEV_WEBSITE_URL")
                ?? throw new InvalidOperationException("Variável DEV_WEBSITE_URL indefinida.");
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
                xmlPath = Path.Combine(AppContext.BaseDirectory, "bin/Debug/net8.0", xmlFile);
            if (File.Exists(xmlPath))
                c.IncludeXmlComments(xmlPath);
            c.EnableAnnotations();
        });
        return services;
    }
}
