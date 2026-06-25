namespace OnlineShop.Core.Entities;

/// <summary>
/// یک ردیف از فاکتور خرید. مشخصات کالا (اسم و قیمت) در لحظه خرید snapshot می شود
/// تا حتی اگر بعدا قیمت کالا در فروشگاه تغییر کند، فاکتور قدیمی دستکاری نشود.
/// </summary>
public sealed class InvoiceItem
{
    public InvoiceItem(int productId, string productName, decimal unitPrice, int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public int ProductId { get; }

    public string ProductName { get; }

    public decimal UnitPrice { get; }

    public int Quantity { get; }

    public decimal LineTotal => UnitPrice * Quantity;

    public override string ToString() => $"{ProductName} × {Quantity} = {LineTotal:N0}";
}
