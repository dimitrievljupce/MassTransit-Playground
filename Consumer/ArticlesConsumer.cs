using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Consumer;

public sealed class ArticlesConsumer(ILogger<ArticlesConsumer> logger) : IConsumer<ArticleCreatedEvent>
{
    public Task Consume(ConsumeContext<ArticleCreatedEvent> context)
    {
        var message =
            $"Consuming ArticleCreatedEvent {DateTime.UtcNow}, published on {context.Message.CreatedOn}";
        logger.LogInformation(message);
        return Task.CompletedTask;
    }
}