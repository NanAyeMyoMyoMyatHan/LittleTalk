namespace LittleTalk.Model.Common;

public class PaginationRequestModel
{
    private int _pageNumber = 1;
    private int _pageSize = DevCode.DefaultPageSize;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value.NormalizePageNumber();
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value.NormalizePageSize();
    }

    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}
