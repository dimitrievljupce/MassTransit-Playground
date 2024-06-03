using Consumer;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;

var consumerHost = Host.CreateDefaultBuilder(args)
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
            .WriteTo.Elasticsearch(
                new[] { new Uri(context.Configuration.GetSection("ElasticConfiguration:Uri").Value!) }, opts =>
                {
                    opts.DataStream = new DataStreamName("consumer", "articles", "df");
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
            busConfigurator.AddConsumer<ArticlesConsumer>();
        });
    }).Build();

Log.Information("Consumer initiated. Start timestamp: {startingOn}", DateTime.UtcNow);
await consumerHost.RunAsync();
Log.Information("Consumer shutdown complete. Timestamp: {downDate}", DateTime.UtcNow);
