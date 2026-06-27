namespace OnlineShop.Controllers.Catalog;

/// <summary>نتیجه صفحه بندی شده یک لیست؛ برای نمایش کالاها ۱۰ تا در هر صفحه استفاده می شود.</summary>
public sealed class PagedResult<T>
{
    public PagedResult(IReadOnlyList<T> items, int pageNumber, int pageSize, int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public IReadOnlyList<T> Items { get; }

    public int PageNumber { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
