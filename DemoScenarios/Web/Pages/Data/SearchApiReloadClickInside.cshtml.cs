using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;

namespace Web.Pages.Data;

public class SearchApiReloadClickInsidePageModel(
    ILogger<SearchApiReloadPageModel> logger,
    MemoryHttpService memoryHttpService)
    : PageModel
{
    public void OnGet() =>
        logger.LogInformation("Loading page SearchApiReloadClickInsidePageModel at {DateLoaded}", DateTime.Now);

    public async Task<IActionResult> OnGetSearchAsync(string query)
    {
        logger.LogInformation("Calling search API with query {Query}", query);
        var data = await memoryHttpService.SearchAsync(query);
        logger.LogInformation("Search API returned {Count} results", data.Length);
        return new JsonResult(data);
    }
}