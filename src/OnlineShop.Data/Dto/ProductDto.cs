using OnlineShop.Core.Enums;

namespace OnlineShop.Data.Dto;

/// <summary>
/// نسخه قابل سریالایز (flat) از یک کالا برای ذخیره در فایل JSON.
/// چون کالاها سلسله مراتب ارثی دارند، فیلد <see cref="TypeName"/> مشخص می کند
/// این رکورد باید به کدام کلاس واقعی (مثلا Car یا Pencil) تبدیل شود؛ فیلدهایی که
/// به آن نوع خاص مربوط نیستند، null باقی می مانند.
/// </summary>
public sealed class ProductDto
{
    public int Id { get; set; }

    public string TypeName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public bool IsDeleted { get; set; }

    // DigitalGood
    public double? Weight { get; set; }

    public string? Dimensions { get; set; }

    // StorageDevice
    public int? CapacityGb { get; set; }

    // FlashMemory
    public string? UsbVersion { get; set; }

    // Ssd
    public int? ReadSpeedMbps { get; set; }

    public int? WriteSpeedMbps { get; set; }

    // PersonalComputer
    public string? CpuModel { get; set; }

    public int? RamCapacityGb { get; set; }

    // Stationery
    public string? CountryOfOrigin { get; set; }

    // Pencil
    public PencilType? PencilType { get; set; }

    // Pen
    public string? Color { get; set; }

    // Notebook
    public int? PageCount { get; set; }

    public string? PaperType { get; set; }

    // Vehicle
    public string? Manufacturer { get; set; }

    // Bicycle
    public BicycleType? BicycleType { get; set; }

    // Car
    public int? EngineVolume { get; set; }

    public bool? IsAutomatic { get; set; }

    // Food
    public DateTime? ProductionDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public List<CommentDto> Comments { get; set; } = new();

    public List<RatingDto> Ratings { get; set; } = new();
}
