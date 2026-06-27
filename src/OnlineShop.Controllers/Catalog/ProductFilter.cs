using OnlineShop.Core.Enums;

namespace OnlineShop.Controllers.Catalog;

/// <summary>
/// معیارهای فیلتر و جستجوی کالا در صفحه محصولات. طبق صورت پروژه حداقل ۵ فیلتر پیاده سازی شده:
/// جستجو بر اساس نام، دسته کالا، وضعیت موجودی، بازه قیمت، بازه امتیاز؛ به همراه چند فیلتر
/// اختصاصی هر دسته (نوع مداد، نوع دوچرخه، اتومات بودن خودرو، حداقل ظرفیت حافظه، غیرمنقضی بودن خوراکی).
/// </summary>
public sealed class ProductFilter
{
    public string? NameContains { get; set; }

    public Category? Category { get; set; }

    /// <summary>فقط کالاهای موجود (موجودی بیشتر از صفر) نمایش داده شوند.</summary>
    public bool? OnlyAvailable { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public double? MinRating { get; set; }

    public double? MaxRating { get; set; }

    // فیلترهای اختصاصی هر دسته
    public PencilType? PencilType { get; set; }

    public BicycleType? BicycleType { get; set; }

    public bool? IsAutomatic { get; set; }

    public int? MinCapacityGb { get; set; }

    public bool? ExcludeExpiredFood { get; set; }

    public string? ManufacturerContains { get; set; }
}
