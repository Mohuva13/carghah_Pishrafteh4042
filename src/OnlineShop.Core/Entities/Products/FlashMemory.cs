namespace OnlineShop.Core.Entities.Products;

/// <summary>فلش مموری: زیر دسته تجهیزات ذخیره سازی اطلاعات. ویژگی اختصاصی: ورژن USB.</summary>
public sealed class FlashMemory : StorageDevice
{
    public FlashMemory(int id, string name, decimal price, int stockQuantity, double weight, string dimensions,
        int capacityGb, string usbVersion)
        : base(id, name, price, stockQuantity, weight, dimensions, capacityGb)
    {
        UsbVersion = usbVersion;
    }

    public string UsbVersion { get; set; }

    public override string GetSpecificDetails() =>
        $"فلش مموری | ظرفیت: {CapacityGb}GB | USB {UsbVersion} | وزن: {Weight}g | ابعاد: {Dimensions}";
}
