using OnlineShop.Core.Entities;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.Controllers;

/// <summary>مسئول افزودن/حذف کالا از سبد خرید. طبق صورت پروژه، فقط کاربر لاگین کرده می تواند کالا به سبد اضافه کند.</summary>
public sealed class CartController
{
    private readonly ShopContext _context;

    public CartController(ShopContext context)
    {
        _context = context;
    }

    public void AddToCart(Buyer buyer, int productId, int quantity = 1)
    {
        var product = Manager.Instance.FindProduct(productId) ?? throw new EntityNotFoundException("کالا", productId);

        if (product.IsDeleted)
        {
            throw new DomainException("این کالا دیگر در فروشگاه موجود نیست.");
        }

        if (product.StockQuantity < quantity)
        {
            throw new InsufficientStockException(product.Name, product.StockQuantity, quantity);
        }

        buyer.Cart.AddProduct(product, quantity);
        _context.SaveChanges();
    }

    public void RemoveFromCart(Buyer buyer, int productId)
    {
        buyer.Cart.RemoveProduct(productId);
        _context.SaveChanges();
    }

    public void DecreaseQuantity(Buyer buyer, int productId, int quantity = 1)
    {
        buyer.Cart.DecreaseQuantity(productId, quantity);
        _context.SaveChanges();
    }
}
