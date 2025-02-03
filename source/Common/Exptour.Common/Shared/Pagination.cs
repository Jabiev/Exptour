namespace Exptour.Common.Shared;

public class Pagination<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPage { get; set; }
}
