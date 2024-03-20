using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Data;

public class SearchApiCallPageModel(ILogger<SearchApiCallPageModel> logger) : PageModel
{
    public void OnGet() => logger.LogInformation("SearchApiCallPageModel.OnGet called at {DateLoaded}", DateTime.Now);
}