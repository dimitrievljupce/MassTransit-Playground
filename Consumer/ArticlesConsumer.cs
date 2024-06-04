using Contracts;
using MassTransit;
using Serilog;

namespace Consumer;

public sealed class ArticlesConsumer : IConsumer<ArticleCreatedEvent>
{
    public Task Consume(ConsumeContext<ArticleCreatedEvent> context)
    {
        var articleIdentifier = context.Message.Id;
        var createdOn = context.Message.CreatedOn;

        Log.Information("Consuming ArticleCreatedEvent with id {ArticleId}, published on {CreatedOn}",
            articleIdentifier,
            createdOn);

        return Task.CompletedTask;
    }
}