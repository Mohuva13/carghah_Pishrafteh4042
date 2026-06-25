namespace OnlineShop.Core.Entities.Products;

/// <summary>خودرو: زیر دسته وسایل نقلیه. ویژگی های اختصاصی: حجم موتور و بولین اتومات بودن.</summary>
public sealed class Car : Vehicle
{
    public Car(int id, string name, decimal price, int stockQuantity, string manufacturer,
        int engineVolume, bool isAutomatic)
        : base(id, name, price, stockQuantity, manufacturer)
    {
        EngineVolume = engineVolume;
        IsAutomatic = isAutomatic;
    }

    public int EngineVolume { get; set; }

    public bool IsAutomatic { get; set; }

    public override string GetSpecificDetails() =>
        $"خودرو | حجم موتور: {EngineVolume} | گیربکس: {(IsAutomatic ? "اتومات" : "دنده ای")} | شرکت سازنده: {Manufacturer}";
}
