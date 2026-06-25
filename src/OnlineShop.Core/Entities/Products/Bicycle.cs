using OnlineShop.Core.Enums;

namespace OnlineShop.Core.Entities.Products;

/// <summary>دوچرخه: زیر دسته وسایل نقلیه. ویژگی اختصاصی: نوع دوچرخه (enum).</summary>
public sealed class Bicycle : Vehicle
{
    public Bicycle(int id, string name, decimal price, int stockQuantity, string manufacturer, BicycleType bicycleType)
        : base(id, name, price, stockQuantity, manufacturer)
    {
        BicycleType = bicycleType;
    }

    public BicycleType BicycleType { get; set; }

    public override string GetSpecificDetails() =>
        $"دوچرخه | نوع: {DisplayBicycleType()} | شرکت سازنده: {Manufacturer}";

    private string DisplayBicycleType() => BicycleType switch
    {
        BicycleType.Mountain => "کوهستانی",
        BicycleType.Road => "جاده ای",
        BicycleType.Urban => "شهری",
        BicycleType.Hybrid => "هیبرید",
        _ => BicycleType.ToString()
    };
}
