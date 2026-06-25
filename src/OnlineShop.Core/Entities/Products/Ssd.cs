namespace OnlineShop.Core.Entities.Products;

/// <summary>اس اس دی (SSD): زیر دسته تجهیزات ذخیره سازی اطلاعات. ویژگی های اختصاصی: سرعت خواندن و نوشتن.</summary>
public sealed class Ssd : StorageDevice
{
    public Ssd(int id, string name, decimal price, int stockQuantity, double weight, string dimensions,
        int capacityGb, int readSpeedMbps, int writeSpeedMbps)
        : base(id, name, price, stockQuantity, weight, dimensions, capacityGb)
    {
        ReadSpeedMbps = readSpeedMbps;
        WriteSpeedMbps = writeSpeedMbps;
    }

    public int ReadSpeedMbps { get; set; }

    public int WriteSpeedMbps { get; set; }

    public override string GetSpecificDetails() =>
        $"SSD | ظرفیت: {CapacityGb}GB | سرعت خواندن: {ReadSpeedMbps}MB/s | سرعت نوشتن: {WriteSpeedMbps}MB/s";
}
