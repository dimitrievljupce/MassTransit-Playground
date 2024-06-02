using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Producer.Interfaces;
using Serilog;

namespace Producer.Implementation;

public class ArticlesAdapter(IPublishEndpoint publishEndpoint, ILogger<IArticlesAdapter> logger) : IArticlesAdapter
{
    private int _numberOfArticlesToProduce = 50;

    public async Task ProduceArticles()
    {
        while (_numberOfArticlesToProduce >= 0)
        {
            Log.Information("Publishing ArticleCreatedEvent {publishedOn}", DateTime.UtcNow);
            await publishEndpoint.Publish(new ArticleCreatedEvent
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
            });

            _numberOfArticlesToProduce -= 1;
            await Task.Delay(5000);
        }
    }
}