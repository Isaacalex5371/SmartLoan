namespace SmartLoan.Application.Common;

public class PagedRequest
{
    private const int MaxPageSize = 50;
    public int PageNumber { get; init; } = 1;
    private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            init => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public string? Search { get; init; }

}