using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Producer.Interfaces;

namespace Producer.Implementation;

public class ArticlesAdapter(IPublishEndpoint publishEndpoint, ILogger<IArticlesAdapter> logger) : IArticlesAdapter
{
    private int _numberOfArticlesToProduce = 10;

    public async Task ProduceArticles()
    {
        while (_numberOfArticlesToProduce >= 0)
        {
            logger.LogInformation("Publishing ArticleCreatedEvent {publishedOn}", DateTime.UtcNow);
            
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