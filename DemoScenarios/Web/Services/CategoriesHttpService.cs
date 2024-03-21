using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Web.Helpers;
using Web.Models;

namespace Web.Services;

public class CategoriesHttpService
{
    private readonly HttpClient httpClient;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILogger<CategoriesHttpService> logger;

    public CategoriesHttpService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor,
        ILogger<CategoriesHttpService> logger)
    {
        this.httpClient = httpClient;
        this.httpContextAccessor = httpContextAccessor;
        this.logger = logger;
    }

    public async Task<string> IsAlive()
    {
        logger.LogInformation("IsAlive  from server side called at {DateLoaded}, waiting for response from API.",
            DateTime.Now);
        return await httpClient.GetStringAsync(RouteHelper.HealthApiRoute);
    }

    public async Task<IEnumerable<CategoryModel>> GetGeneralCategories(int count = 20)
    {
        logger.LogInformation(
            "GetGeneralCategories from server side called at {DateLoaded} with {Count} categories request, waiting for response from API.",
            DateTime.Now, count);
        var url =
            $"{httpContextAccessor.HttpContext?.Request.Scheme}://" +
            $"{httpContextAccessor.HttpContext?.Request.Host}{httpContextAccessor.HttpContext?.Request.PathBase}" +
            $"/{RouteHelper.CategoryApiRoute}/{RouteHelper.GetAllApiRoute}/{count}";
        var result = await httpClient.GetAsync(url);
        if (!result.IsSuccessStatusCode)
        {
            logger.LogError("Error while fetching categories from api. {StatusCode} {ReasonPhrase}",
                result.StatusCode, result.ReasonPhrase);
            return Array.Empty<CategoryModel>();
        }

        var data = await result.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(data))
        {
            logger.LogInformation("No data found for categories.");
            return Array.Empty<CategoryModel>();
        }

        var currentList = JsonConvert.DeserializeObject<CategoryModel[]>(data) ??
                          Array.Empty<CategoryModel>();
        logger.LogInformation("Found {Count} categories from api.", currentList.Length);
        return currentList;
    }
}