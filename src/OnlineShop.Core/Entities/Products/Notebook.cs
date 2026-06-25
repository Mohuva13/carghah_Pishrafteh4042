namespace OnlineShop.Core.Entities.Products;

/// <summary>دفتر: زیر دسته لوازم تحریر. ویژگی های اختصاصی: تعداد برگ و نوع کاغذ.</summary>
public sealed class Notebook : Stationery
{
    public Notebook(int id, string name, decimal price, int stockQuantity, string countryOfOrigin,
        int pageCount, string paperType)
        : base(id, name, price, stockQuantity, countryOfOrigin)
    {
        PageCount = pageCount;
        PaperType = paperType;
    }

    public int PageCount { get; set; }

    public string PaperType { get; set; }

    public override string GetSpecificDetails() =>
        $"دفتر | تعداد برگ: {PageCount} | نوع کاغذ: {PaperType} | کشور سازنده: {CountryOfOrigin}";
}
