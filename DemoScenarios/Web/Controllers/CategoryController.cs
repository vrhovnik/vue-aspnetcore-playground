using System.Net.Mime;
using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Helpers;
using Web.Models;

namespace Web.Controllers;

[AllowAnonymous, ApiController, Route(RouteHelper.CategoryApiRoute),
 Produces(MediaTypeNames.Application.Json)]
public class CategoryController(ILogger<CategoryController> logger) : Controller
{
    [HttpGet]
    [Route(RouteHelper.HealthApiRoute)]
    public IActionResult IsAlive()
    {
        var currentTime = DateTime.Now;
        logger.LogInformation("IsAlive in categories called at {DateCalled}.", currentTime);
        return Ok($"I'm alive at {currentTime} and running on {Environment.MachineName}!");
    }

    [HttpGet]
    [Produces(typeof(IEnumerable<CategoryModel>))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route(RouteHelper.GetAllApiRoute + "/{count?}")]
    public IActionResult GetGeneralCategories(int? count)
    {
        var currentCount = count ?? 20;
        logger.LogInformation("Get general categories called at {DateLoaded} with {Count}", DateTime.Now, currentCount);

        var list = new Faker<CategoryModel>()
            .RuleFor(props => props.CategoryId, f => f.Random.Int(1, currentCount))
            .RuleFor(props => props.Name, f => f.Random.Words(2))
            .GenerateLazy(currentCount);

        logger.LogInformation("Returned {Count} categories.", list.Count());
        return Ok(list);
    }
}