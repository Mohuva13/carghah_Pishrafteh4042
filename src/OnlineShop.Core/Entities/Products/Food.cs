using OnlineShop.Core.Enums;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.Core.Entities.Products;

/// <summary>خوراکی. ویژگی های اختصاصی: تاریخ تولید و تاریخ انقضا.</summary>
public sealed class Food : Product
{
    public Food(int id, string name, decimal price, int stockQuantity, DateTime productionDate, DateTime expirationDate)
        : base(id, name, price, stockQuantity, Category.Food)
    {
        if (expirationDate <= productionDate)
        {
            throw new DomainException("تاریخ انقضا باید بعد از تاریخ تولید باشد.");
        }

        ProductionDate = productionDate;
        ExpirationDate = expirationDate;
    }

    public DateTime ProductionDate { get; set; }

    public DateTime ExpirationDate { get; set; }

    public bool IsExpired => DateTime.Now > ExpirationDate;

    public override string GetSpecificDetails() =>
        $"خوراکی | تاریخ تولید: {ProductionDate:yyyy/MM/dd} | تاریخ انقضا: {ExpirationDate:yyyy/MM/dd}{(IsExpired ? " (منقضی شده)" : string.Empty)}";
}
