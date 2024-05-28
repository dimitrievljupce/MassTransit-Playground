using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Producer;
using Producer.Implementation;
using Producer.Interfaces;

var producerHost = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.AddConfiguration(context.Configuration.GetSection("Logging"));
        logging.AddConsole();
        logging.AddDebug();
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.UsingRabbitMq((context, configurator) =>
            {
                configurator.Host(new Uri(configuration.GetSection("MessageBroker:Host").Value!), h =>
                {
                    h.Username(configuration.GetSection("MessageBroker:Username").Value!);
                    h.Password(configuration.GetSection("MessageBroker:Password").Value!);
                });

                configurator.ConfigureEndpoints(context);
            });
        });

        services.AddSingleton<IArticlesAdapter, ArticlesAdapter>();
    }).Build();

await producerHost.Services.GetRequiredService<IArticlesAdapter>().ProduceArticles();
await producerHost.RunAsync();