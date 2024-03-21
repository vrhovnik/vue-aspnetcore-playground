using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Models;
using Web.Services;

namespace Web.Pages.Data;

public class SearchApiWithFiltersPageModel(ILogger<SearchApiWithFiltersPageModel> logger, CategoriesHttpService categoriesHttpService) : PageModel
{
    public async Task OnGetAsync()
    {
        logger.LogInformation("SearchApiWithFiltersPageModel.OnGet called at {DateLoaded}.", DateTime.Now);
        var list = await categoriesHttpService.GetGeneralCategories();
        Categories = list.ToList();
        logger.LogInformation("Loaded {Count} categories.", Categories.Count);
    }

    public List<CategoryModel> Categories { get; set; }
}