using OnlineShop.Core.Entities;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.Controllers;

/// <summary>مسئول نمره دهی به کالا. طبق صورت پروژه تنها خریدارِ واقعیِ کالا می تواند نمره ثبت کند.</summary>
public sealed class RatingController
{
    private readonly ShopContext _context;

    public RatingController(ShopContext context)
    {
        _context = context;
    }

    public void RateProduct(Buyer buyer, int productId, int score)
    {
        var product = Manager.Instance.FindProduct(productId) ?? throw new EntityNotFoundException("کالا", productId);

        if (!buyer.HasPurchased(productId))
        {
            throw new PurchaseRequiredException("نمره دهی به این کالا");
        }

        product.AddOrUpdateRating(new Rating(buyer.Username, productId, score));
        _context.SaveChanges();
    }
}
