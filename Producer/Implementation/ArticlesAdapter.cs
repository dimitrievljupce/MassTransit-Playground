using Contracts;
using MassTransit;
using Producer.Interfaces;
using Serilog;

namespace Producer.Implementation;

public class ArticlesAdapter(IPublishEndpoint publishEndpoint) : IArticlesAdapter
{
    private int _numberOfArticlesToProduce = 50;

    public async Task ProduceArticles()
    {
        while (_numberOfArticlesToProduce > 0)
        {
            var articleIdentifier = Guid.NewGuid();
            var currentTime = DateTime.UtcNow;

            Log.Information("Starting to publish ArticleCreatedEvent. Id: {ArticleId}, Time: {PublishedOn}",
                articleIdentifier, currentTime);

            try
            {
                await publishEndpoint.Publish(new ArticleCreatedEvent
                {
                    Id = articleIdentifier,
                    CreatedOn = currentTime
                });

                Log.Information("Successfully published ArticleCreatedEvent. Id: {ArticleId}", articleIdentifier);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to publish ArticleCreatedEvent. Id: {ArticleId}", articleIdentifier);
            }

            _numberOfArticlesToProduce -= 1;
            Log.Information("{RemainingCount} articles left to produce.", _numberOfArticlesToProduce);

            await Task.Delay(10000);
        }

        Log.Information("Article production completed. No more articles left to produce.");
    }
}