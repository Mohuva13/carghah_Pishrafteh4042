using OnlineShop.Core.Enums;

namespace OnlineShop.Core.Entities.Products;

/// <summary>مداد: زیر دسته لوازم تحریر. ویژگی اختصاصی: نوع مداد (enum).</summary>
public sealed class Pencil : Stationery
{
    public Pencil(int id, string name, decimal price, int stockQuantity, string countryOfOrigin, PencilType pencilType)
        : base(id, name, price, stockQuantity, countryOfOrigin)
    {
        PencilType = pencilType;
    }

    public PencilType PencilType { get; set; }

    public override string GetSpecificDetails() =>
        $"مداد | نوع: {DisplayPencilType()} | کشور سازنده: {CountryOfOrigin}";

    private string DisplayPencilType() => PencilType switch
    {
        PencilType.TwoH => "2H",
        PencilType.H => "H",
        PencilType.F => "F",
        PencilType.B => "B",
        PencilType.HB => "HB",
        _ => PencilType.ToString()
    };
}
