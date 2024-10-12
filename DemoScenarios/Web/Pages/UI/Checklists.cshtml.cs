using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.UI;

public class ChecklistsPageModel(ILogger<ChecklistsPageModel> logger) : PageModel
{
    public void OnGet() => logger.LogInformation("Checklist loaded at {Dateloaded}", DateTime.Now);
}