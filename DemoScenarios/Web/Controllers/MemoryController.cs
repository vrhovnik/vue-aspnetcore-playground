using System.Net.Mime;
using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Web.Helpers;
using Web.Models;
using Web.Options;

namespace Web.Controllers;

[AllowAnonymous, ApiController, Route(RouteHelper.MemoryApiRoute),
 Produces(MediaTypeNames.Application.Json)]
public class MemoryController(
    ILogger<MemoryController> logger,
    IOptions<DefaultWebOptions> defaultWebOptions,
    IMemoryCache memoryCache)
    : Controller
{
    private const string CacheKey = "SearchResults";

    [HttpGet]
    [Route(RouteHelper.HealthApiRoute)]
    public IActionResult IsAlive()
    {
        var currentTime = DateTime.Now;
        logger.LogInformation("IsAlive called at {DateCalled}.", currentTime);
        return Ok($"I'm alive (MemoryController) at {currentTime} and running on {Environment.MachineName}!");
    }

    [HttpGet]
    [Produces(typeof(IEnumerable<SearchResult>))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route(RouteHelper.SearchApiRoute + "/{query?}")]
    public IActionResult Search(string? query)
    {
        logger.LogInformation("Search called with query: {Query}.", query);
        List<SearchResult> list;
        if (memoryCache.TryGetValue(CacheKey, out IEnumerable<SearchResult>? results))
        {
            logger.LogInformation("Returning cached results.");
            list = (results ?? Array.Empty<SearchResult>()).ToList();
        }
        else
        {
            var recordSize = defaultWebOptions.Value.RecordSize;
            var currentList = new Faker<SearchResult>()
                .RuleFor(props => props.Id, f => f.Random.Int(1, recordSize))
                .RuleFor(props => props.Title, f => f.Random.Words(5))
                .RuleFor(props => props.Description, f => f.Lorem.Paragraph(4))
                .RuleFor(props => props.Url, f => f.Internet.Url())
                .RuleFor(props => props.GeneratedAt, f => f.Date.Recent())
                .GenerateLazy(recordSize);
            list = currentList.ToList();
            memoryCache.Set(CacheKey, list);
        }

        if (!string.IsNullOrEmpty(query))
            list = list.Where(x => x.Title.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

        logger.LogInformation("Returned {Count} results.", list.Count);
        return Ok(list);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Route(RouteHelper.AddApiRoute)]
    public IActionResult ChangeValueAsync(SearchResultPostModel postModel)
    {
        logger.LogInformation("ChangeValueAsync called at {DateCalled} with {Id} and new title {Title}",
            DateTime.Now, postModel.Id, postModel.Title);
        if (!memoryCache.TryGetValue(CacheKey, out List<SearchResult>? results))
        {
            logger.LogInformation("No data in cache, first load cache");
            return BadRequest("No results found.");
        }

        if (results == null)
        {
            logger.LogInformation("No data in cache, first load cache");
            return BadRequest("No results found.");
        }

        var currentResult = results.Find(searchResult => searchResult.Id == postModel.Id);
        if (currentResult == null)
        {
            logger.LogInformation("No data found with id {Id}", postModel.Id);
            return BadRequest("No results found.");
        }
        currentResult.Title = postModel.Title;
        memoryCache.Set(CacheKey, results);
        return Ok();
    }
}