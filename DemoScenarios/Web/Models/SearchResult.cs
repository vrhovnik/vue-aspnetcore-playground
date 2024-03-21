namespace Web.Models;

public class SearchResult
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Url { get; set; }
    public DateTime GeneratedAt { get; set; }
}