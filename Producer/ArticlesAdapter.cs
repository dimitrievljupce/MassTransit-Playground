using Contracts;
using MassTransit;

namespace Producer;

// TODO: add logger
public class ArticlesAdapter(IPublishEndpoint publishEndpoint) : IArticlesAdapter
{
    private int _numberOfArticlesToProduce = 10;

    public async Task ProduceArticles()
    {
        while (_numberOfArticlesToProduce >= 0)
        {
            await publishEndpoint.Publish(new ArticleCreatedEvent()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
            });

            _numberOfArticlesToProduce -= 1;
            await Task.Delay(2000, new CancellationToken());
        }
    }
}