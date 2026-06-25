using OnlineShop.Core.Enums;

namespace OnlineShop.Core.Entities.Products;

/// <summary>
/// کالای دیجیتال (abstract). ویژگی های مشترک این دسته: وزن و ابعاد.
/// زیر دسته های آن «تجهیزات ذخیره سازی اطلاعات» و «کامپیوتر شخصی» هستند.
/// </summary>
public abstract class DigitalGood : Product
{
    protected DigitalGood(int id, string name, decimal price, int stockQuantity, double weight, string dimensions)
        : base(id, name, price, stockQuantity, Category.DigitalGood)
    {
        Weight = weight;
        Dimensions = dimensions;
    }

    /// <summary>وزن کالا بر حسب گرم.</summary>
    public double Weight { get; set; }

    /// <summary>ابعاد کالا، مثلا "20x15x3 cm".</summary>
    public string Dimensions { get; set; }
}
