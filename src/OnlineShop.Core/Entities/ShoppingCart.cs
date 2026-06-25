using OnlineShop.Core.Exceptions;

namespace OnlineShop.Core.Entities;

/// <summary>سبد خرید یک خریدار. ممکن است از یک کالا بیش از یک عدد در سبد وجود داشته باشد.</summary>
public sealed class ShoppingCart
{
    private readonly List<CartItem> _items = new();

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

    public decimal TotalAmount => _items.Sum(i => i.LineTotal);

    public bool IsEmpty => _items.Count == 0;

    public void AddProduct(Product product, int quantity = 1)
    {
        if (quantity <= 0)
        {
            throw new DomainException("تعداد درخواستی باید بزرگتر از صفر باشد.");
        }

        var existing = _items.FirstOrDefault(i => i.Product.Id == product.Id);
        if (existing is not null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            _items.Add(new CartItem(product, quantity));
        }
    }

    public void RemoveProduct(int productId)
    {
        _items.RemoveAll(i => i.Product.Id == productId);
    }

    public void DecreaseQuantity(int productId, int quantity = 1)
    {
        var existing = _items.FirstOrDefault(i => i.Product.Id == productId);
        if (existing is null)
        {
            return;
        }

        existing.Quantity -= quantity;
        if (existing.Quantity <= 0)
        {
            _items.Remove(existing);
        }
    }

    public void Clear() => _items.Clear();

    public override string ToString() =>
        IsEmpty ? "سبد خرید خالی است." : string.Join(Environment.NewLine, _items) + $"{Environment.NewLine}جمع کل: {TotalAmount:N0}";
}
