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

[AllowAnonymous, ApiController, Route(RouteHelper.UiApiRoute),
 Produces(MediaTypeNames.Application.Json)]
public class UiController(
    ILogger<MemoryController> logger,
    IOptions<DefaultWebOptions> defaultWebOptions,
    IMemoryCache memoryCache)
    : Controller
{
    private const string CacheKey = "UIData";

    [HttpGet]
    [Route(RouteHelper.HealthApiRoute)]
    public IActionResult IsAlive()
    {
        var currentTime = DateTime.Now;
        logger.LogInformation("IsAlive called at {DateCalled}.", currentTime);
        return Ok($"I'm alive (UiController) at {currentTime} and running on {Environment.MachineName}!");
    }

    [HttpGet]
    [Route(RouteHelper.ChecklistApiRoute)]
    [Produces(typeof(IEnumerable<CheckListModelCategory>))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetChecklistDataAsync()
    {
        logger.LogInformation("Calling api {DateLoaded} and getting back random checklist items", DateTime.Now);

        var list = new Faker<CheckListModelCategory>()
            .RuleFor(props => props.Id, f => f.Random.Guid().ToString())
            .RuleFor(props => props.Name, f => f.Random.Words(3))
            .Generate(5)
            .ToList();

        foreach (var currentCategory in list)
        {
            logger.LogInformation("Adding list to category {CategoryName}", currentCategory.Name);
            var random = new Random().Next(3, 6);
            var checkListModels = new Faker<CheckListModel>()
                .RuleFor(props => props.CheckListModelId, f => f.Random.Guid().ToString())
                .RuleFor(props => props.Name, f => f.Random.Words(3))
                .RuleFor(props => props.VideoUrl, f => f.Internet.Url())
                .RuleFor(props => props.Description, f => f.Random.Words(6))
                .Generate(random);
            currentCategory.CheckLists = checkListModels.ToList();
            logger.LogInformation("Added {NumberOfChecklists} to category {CategoryName}", random,
                currentCategory.Name);
        }

        return Ok(list);
    }
}