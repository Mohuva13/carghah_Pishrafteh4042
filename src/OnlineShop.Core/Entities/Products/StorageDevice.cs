namespace OnlineShop.Core.Entities.Products;

/// <summary>تجهیزات ذخیره سازی اطلاعات (abstract). ویژگی مشترک: ظرفیت (بر حسب گیگابایت).</summary>
public abstract class StorageDevice : DigitalGood
{
    protected StorageDevice(int id, string name, decimal price, int stockQuantity, double weight, string dimensions, int capacityGb)
        : base(id, name, price, stockQuantity, weight, dimensions)
    {
        CapacityGb = capacityGb;
    }

    public int CapacityGb { get; set; }
}
