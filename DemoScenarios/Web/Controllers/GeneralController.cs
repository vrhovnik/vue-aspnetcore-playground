using System.Net.Mime;
using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Helpers;
using Web.Models;
using Web.Options;

namespace Web.Controllers;

[AllowAnonymous, ApiController, Route(RouteHelper.GeneralApiRoute),
 Produces(MediaTypeNames.Application.Json)]
public class GeneralController(ILogger<GeneralController> logger, IOptions<DefaultWebOptions> defaultWebOptions)
    : Controller
{
    [HttpGet]
    [Route(RouteHelper.HealthApiRoute)]
    public IActionResult IsAlive()
    {
        var currentTime = DateTime.Now;
        logger.LogInformation("IsAlive called at {DateCalled}.", currentTime);
        return Ok($"I'm alive at {currentTime} and running on {Environment.MachineName}!");
    }

    [HttpGet]
    [Produces(typeof(IEnumerable<SearchResult>))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route(RouteHelper.SearchApiRoute + "/{query?}")]
    public IActionResult Search(string? query)
    {
        logger.LogInformation("Search called with query: {Query}.", query);
        var recordSize = defaultWebOptions.Value.RecordSize;
        var list = new Faker<SearchResult>()
            .RuleFor(props => props.Id, f => f.Random.Int(1, recordSize))
            .RuleFor(props => props.Title, f => f.Random.Words(5))
            .RuleFor(props => props.Description, f => f.Lorem.Paragraph(4))
            .RuleFor(props => props.Url, f => f.Internet.Url())
            .RuleFor(props => props.GeneratedAt, f => f.Date.Recent())
            .GenerateLazy(recordSize);

        if (!string.IsNullOrEmpty(query))
            list = list.Where(x => x.Title.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

        logger.LogInformation("Returned {Count} results.", list.Count());
        return Ok(list);
    }
    
    
}