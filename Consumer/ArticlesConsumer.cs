using Contracts;
using MassTransit;
using Serilog;

namespace Consumer;

public sealed class ArticlesConsumer : IConsumer<ArticleCreatedEvent>
{
    public Task Consume(ConsumeContext<ArticleCreatedEvent> context)
    {
        var articleIdentifier = context.Message.Id;

        Log.Information("Consuming ArticleCreatedEvent with id {id}, published on {createdOn}", articleIdentifier,
            DateTime.UtcNow);

        return Task.CompletedTask;
    }
}