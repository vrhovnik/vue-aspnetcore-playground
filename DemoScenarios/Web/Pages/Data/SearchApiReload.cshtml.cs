using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;

namespace Web.Pages.Data;

public class SearchApiReloadPageModel(ILogger<SearchApiReloadPageModel> logger, GeneralHttpService generalHttpService)
    : PageModel
{
    public void OnGet() => logger.LogInformation("Loading page at {DateLoaded}", DateTime.Now);

    public async Task<IActionResult> OnGetSearchAsync(string query)
    {
        logger.LogInformation("Calling search API with query {Query}", query);
        var data = await generalHttpService.SearchAsync(query);
        logger.LogInformation("Search API returned {Count} results", data.Length);
        return new JsonResult(data);
    }
}