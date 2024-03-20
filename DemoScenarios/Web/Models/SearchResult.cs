namespace Web.Models;

public class SearchResult
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public DateTime GeneratedAt { get; set; }
}