using OnlineShop.Core.Enums;

namespace OnlineShop.Core.Entities.Products;

/// <summary>لوازم تحریر (abstract). ویژگی مشترک: کشور تولید کننده.</summary>
public abstract class Stationery : Product
{
    protected Stationery(int id, string name, decimal price, int stockQuantity, string countryOfOrigin)
        : base(id, name, price, stockQuantity, Category.Stationery)
    {
        CountryOfOrigin = countryOfOrigin;
    }

    public string CountryOfOrigin { get; set; }
}
