namespace OnlineShop.Core.Entities.Products;

/// <summary>خودکار: زیر دسته لوازم تحریر. ویژگی اختصاصی: رنگ.</summary>
public sealed class Pen : Stationery
{
    public Pen(int id, string name, decimal price, int stockQuantity, string countryOfOrigin, string color)
        : base(id, name, price, stockQuantity, countryOfOrigin)
    {
        Color = color;
    }

    public string Color { get; set; }

    public override string GetSpecificDetails() =>
        $"خودکار | رنگ: {Color} | کشور سازنده: {CountryOfOrigin}";
}
