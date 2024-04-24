using Newtonsoft.Json;
using Web.Helpers;
using Web.Models;

namespace Web.Services;

public class GeneralHttpService(
    HttpClient httpClient,
    IHttpContextAccessor httpContextAccessor,
    ILogger<GeneralHttpService> logger)
{
    public async Task<string> IsAlive()
    {
        logger.LogInformation("IsAlive from server side called at {DateLoaded}, waiting for response from API.",
            DateTime.Now);
        return await httpClient.GetStringAsync(RouteHelper.HealthApiRoute);
    }

    public async Task<SearchResult[]> SearchAsync(string query)
    {
        logger.LogInformation(
            "Get genera from server side called at {DateLoaded} with {Query} request, waiting for response from API.",
            DateTime.Now, query);
        var url =
            $"{httpContextAccessor.HttpContext?.Request.Scheme}://" +
            $"{httpContextAccessor.HttpContext?.Request.Host}{httpContextAccessor.HttpContext?.Request.PathBase}" +
            $"/{RouteHelper.GeneralApiRoute}/{RouteHelper.SearchApiRoute}/{query}";
        var result = await httpClient.GetAsync(url);
        if (!result.IsSuccessStatusCode)
        {
            logger.LogError("Error while fetching categories from api. {StatusCode} {ReasonPhrase}",
                result.StatusCode, result.ReasonPhrase);
            return Array.Empty<SearchResult>();
        }

        var data = await result.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(data))
        {
            logger.LogInformation("No data found for categories.");
            return Array.Empty<SearchResult>();
        }

        var currentList = JsonConvert.DeserializeObject<SearchResult[]>(data) ?? [];
        logger.LogInformation("Found {Count} categories from api.", currentList.Length);
        return currentList;
    }
}