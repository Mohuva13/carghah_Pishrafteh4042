namespace OnlineShop.Core.Entities.Products;

/// <summary>کامپیوتر شخصی: زیر دسته کالای دیجیتال. ویژگی های اختصاصی: مدل پردازنده و ظرفیت رم.</summary>
public sealed class PersonalComputer : DigitalGood
{
    public PersonalComputer(int id, string name, decimal price, int stockQuantity, double weight, string dimensions,
        string cpuModel, int ramCapacityGb)
        : base(id, name, price, stockQuantity, weight, dimensions)
    {
        CpuModel = cpuModel;
        RamCapacityGb = ramCapacityGb;
    }

    public string CpuModel { get; set; }

    public int RamCapacityGb { get; set; }

    public override string GetSpecificDetails() =>
        $"کامپیوتر شخصی | پردازنده: {CpuModel} | رم: {RamCapacityGb}GB | وزن: {Weight}g | ابعاد: {Dimensions}";
}
