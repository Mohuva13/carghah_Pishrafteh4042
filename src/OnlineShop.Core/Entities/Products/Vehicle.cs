using OnlineShop.Core.Enums;

namespace OnlineShop.Core.Entities.Products;

/// <summary>وسایل نقلیه (abstract). ویژگی مشترک: نام شرکت سازنده.</summary>
public abstract class Vehicle : Product
{
    protected Vehicle(int id, string name, decimal price, int stockQuantity, string manufacturer)
        : base(id, name, price, stockQuantity, Category.Vehicle)
    {
        Manufacturer = manufacturer;
    }

    public string Manufacturer { get; set; }
}
