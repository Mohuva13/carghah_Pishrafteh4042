namespace OnlineShop.Core.Utils;

/// <summary>
/// تولید کننده شناسه های عددی یکتا و افزایشی برای موجودیت های مختلف (کالا، فاکتور، نظر، درخواست ...).
/// هر موجودیت شمارنده مستقل خودش را دارد تا با بارگذاری مجدد داده از فایل هم دچار تداخل نشویم.
/// </summary>
public sealed class IdGenerator
{
    private int _lastId;

    public IdGenerator(int startFrom = 0)
    {
        _lastId = startFrom;
    }

    public int Next() => ++_lastId;

    /// <summary>در زمان بارگذاری داده از فایل، شمارنده را همگام می کنیم تا شناسه تکراری تولید نشود.</summary>
    public void EnsureAtLeast(int value)
    {
        if (value > _lastId)
        {
            _lastId = value;
        }
    }
}
