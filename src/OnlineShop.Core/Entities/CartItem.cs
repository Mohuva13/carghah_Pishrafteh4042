namespace OnlineShop.Core.Entities;

/// <summary>یک ردیف از سبد خرید: یک کالا به همراه تعداد درخواستی از آن.</summary>
public sealed class CartItem
{
    public CartItem(Product product, int quantity)
    {
        Product = product;
        Quantity = quantity;
    }

    public Product Product { get; }

    public int Quantity { get; set; }

    public decimal LineTotal => Product.Price * Quantity;

    public override string ToString() =>
        $"{Product.Name} × {Quantity} = {LineTotal:N0}";
}
