using OnlineShop.Controllers;
using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Products;
using OnlineShop.Core.Exceptions;
using OnlineShop.Tests.TestHelpers;

namespace OnlineShop.Tests;

public class CommentAndRatingControllerTests
{
    [Fact]
    public void AddComment_AutoDetectsPurchaseStatus_WhenNotPurchased()
    {
        var context = TestContextFactory.CreateFreshContext();
        var product = new Pen(context.ProductIds.Next(), "خودکار", 8000, 10, "France", "Blue");
        Manager.Instance.AddProduct(product);
        var buyer = new Buyer("buyer1", "b@b.com", "09123456789", "Passw0rd1") { IsApproved = true };
        Manager.Instance.RegisterBuyer(buyer);

        var comment = new CommentController(context).AddComment(buyer, product.Id, "نظر تست");

        Assert.False(comment.HasPurchasedProduct);
        Assert.Equal(Core.Enums.CommentStatus.Pending, comment.Status);
        Assert.Contains(Manager.Instance.PendingRequests, r => r.Type == Core.Enums.RequestType.CommentApproval);
    }

    [Fact]
    public void AddComment_AutoDetectsPurchaseStatus_WhenPurchased()
    {
        var context = TestContextFactory.CreateFreshContext();
        var product = new Pen(context.ProductIds.Next(), "خودکار", 8000, 10, "France", "Blue");
        Manager.Instance.AddProduct(product);
        var buyer = new Buyer("buyer1", "b@b.com", "09123456789", "Passw0rd1") { IsApproved = true };
        buyer.IncreaseCredit(1_000_000);
        Manager.Instance.RegisterBuyer(buyer);

        new CartController(context).AddToCart(buyer, product.Id, 1);
        new OrderController(context).Checkout(buyer);

        var comment = new CommentController(context).AddComment(buyer, product.Id, "خیلی خوب بود");

        Assert.True(comment.HasPurchasedProduct);
    }

    [Fact]
    public void RateProduct_ThrowsWhenBuyerHasNotPurchased()
    {
        var context = TestContextFactory.CreateFreshContext();
        var product = new Pen(context.ProductIds.Next(), "خودکار", 8000, 10, "France", "Blue");
        Manager.Instance.AddProduct(product);
        var buyer = new Buyer("buyer1", "b@b.com", "09123456789", "Passw0rd1") { IsApproved = true };
        Manager.Instance.RegisterBuyer(buyer);

        Assert.Throws<PurchaseRequiredException>(() => new RatingController(context).RateProduct(buyer, product.Id, 5));
    }

    [Fact]
    public void RateProduct_SucceedsAfterPurchase_AndUpdatesAverage()
    {
        var context = TestContextFactory.CreateFreshContext();
        var product = new Pen(context.ProductIds.Next(), "خودکار", 8000, 10, "France", "Blue");
        Manager.Instance.AddProduct(product);
        var buyer = new Buyer("buyer1", "b@b.com", "09123456789", "Passw0rd1") { IsApproved = true };
        buyer.IncreaseCredit(1_000_000);
        Manager.Instance.RegisterBuyer(buyer);

        new CartController(context).AddToCart(buyer, product.Id, 1);
        new OrderController(context).Checkout(buyer);

        new RatingController(context).RateProduct(buyer, product.Id, 4);

        Assert.Equal(4, product.AverageRating);
    }
}
