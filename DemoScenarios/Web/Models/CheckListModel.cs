namespace Web.Models;

public class CheckListModelCategory
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<CheckListModel> CheckLists { get; set; } = new();
}

public class CheckListModel
{
    public string CheckListModelId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string VideoUrl { get; set; }
}