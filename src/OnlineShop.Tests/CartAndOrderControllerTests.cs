using OnlineShop.Controllers;
using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Products;
using OnlineShop.Core.Exceptions;
using OnlineShop.Tests.TestHelpers;

namespace OnlineShop.Tests;

public class CartAndOrderControllerTests
{
    private static (ShopContext context, Buyer buyer, Product product) Setup(int stock = 5, decimal price = 100_000, decimal credit = 1_000_000)
    {
        var context = TestContextFactory.CreateFreshContext();
        var product = new Pen(context.ProductIds.Next(), "خودکار", price, stock, "France", "Blue");
        Manager.Instance.AddProduct(product);

        var buyer = new Buyer("buyer1", "b@b.com", "09123456789", "Passw0rd1") { IsApproved = true };
        buyer.IncreaseCredit(credit);
        Manager.Instance.RegisterBuyer(buyer);

        return (context, buyer, product);
    }

    [Fact]
    public void AddToCart_IncreasesQuantity_WhenSameProductAddedTwice()
    {
        var (context, buyer, product) = Setup();
        var cartController = new CartController(context);

        cartController.AddToCart(buyer, product.Id, 2);
        cartController.AddToCart(buyer, product.Id, 3);

        Assert.Single(buyer.Cart.Items);
        Assert.Equal(5, buyer.Cart.Items[0].Quantity);
    }

    [Fact]
    public void AddToCart_ThrowsWhenStockInsufficient()
    {
        var (context, buyer, product) = Setup(stock: 2);
        var cartController = new CartController(context);

        Assert.Throws<InsufficientStockException>(() => cartController.AddToCart(buyer, product.Id, 5));
    }

    [Fact]
    public void Checkout_ReducesStockAndCredit_AndCreatesInvoice()
    {
        var (context, buyer, product) = Setup(stock: 10, price: 50_000, credit: 1_000_000);
        new CartController(context).AddToCart(buyer, product.Id, 3);

        var invoice = new OrderController(context).Checkout(buyer);

        Assert.Equal(7, product.StockQuantity);
        Assert.Equal(1_000_000 - 150_000, buyer.Credit);
        Assert.Single(buyer.Invoices);
        Assert.Equal(150_000, invoice.PaidAmount);
        Assert.True(buyer.Cart.IsEmpty);
        Assert.True(buyer.HasPurchased(product.Id));
    }

    [Fact]
    public void Checkout_ThrowsInsufficientCredit_WhenCreditTooLow()
    {
        var (context, buyer, product) = Setup(stock: 10, price: 500_000, credit: 100_000);
        new CartController(context).AddToCart(buyer, product.Id, 1);

        Assert.Throws<InsufficientCreditException>(() => new OrderController(context).Checkout(buyer));

        // موجودی و اعتبار نباید تغییر کرده باشند چون خرید ناموفق بوده است
        Assert.Equal(10, product.StockQuantity);
        Assert.Equal(100_000, buyer.Credit);
    }

    [Fact]
    public void Checkout_ThrowsOnEmptyCart()
    {
        var (context, buyer, _) = Setup();
        Assert.Throws<DomainException>(() => new OrderController(context).Checkout(buyer));
    }
}
