using Elastic.Channels;
using Elastic.CommonSchema.Serilog;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Producer.Implementation;  
using Producer.Interfaces;
using Serilog;

var producerHost = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureLogging((context, _) =>
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new[] { new Uri("http://localhost:9200") }, opts =>
            {
                opts.DataStream = new DataStreamName("producer", "console-example", "demo");
                opts.BootstrapMethod = BootstrapMethod.Failure;
                opts.ConfigureChannel = channelOpts => { channelOpts.BufferOptions = new BufferOptions(); };
            })
            .ReadFrom.Configuration(context.Configuration.GetSection("Serilog"))
            .CreateLogger();
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

Log.Information("Producer initiated. Start timestamp: {startingOn}", DateTime.UtcNow);
await producerHost.Services.GetRequiredService<IArticlesAdapter>().ProduceArticles();
await producerHost.RunAsync();
Log.Information("Producer shutdown complete. Timestamp: {downDate}", DateTime.UtcNow);

