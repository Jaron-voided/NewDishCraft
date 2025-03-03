namespace Application.Core;

public class PagingParams
{
    private const int MaxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    
    private int _pageSize = 10;

    public int PageSize
    {
        get => _pageSize;
        //get { return _pageSize = 10; }
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        //set { _pageSize = 10 = value; }
    }
}