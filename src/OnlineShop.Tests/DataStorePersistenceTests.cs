using OnlineShop.Controllers;
using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Products;
using OnlineShop.Data.Persistence;
using OnlineShop.Tests.TestHelpers;

namespace OnlineShop.Tests;

/// <summary>تست رفتار ذخیره سازی و بازیابی (round-trip) وضعیت فروشگاه در فایل JSON.</summary>
public class DataStorePersistenceTests : IDisposable
{
    private readonly string _tempFile = Path.Combine(Path.GetTempPath(), $"onlineshop-persist-{Guid.NewGuid():N}.json");

    [Fact]
    public void SaveAndLoad_RoundTrips_ProductsBuyersInvoicesAndRequests()
    {
        Manager.ResetForTests();
        var store = new DataStore(_tempFile);
        var context = new ShopContext(store);

        var car = new Car(context.ProductIds.Next(), "X5", 66000, 3, "BMW", 4300, true);
        var pencil = new Pencil(context.ProductIds.Next(), "Pencil", 15000, 50, "Germany", Core.Enums.PencilType.HB);
        Manager.Instance.AddProduct(car);
        Manager.Instance.AddProduct(pencil);

        var buyer = new Buyer("buyer1", "b@b.com", "09123456789", "Passw0rd1") { IsApproved = true };
        buyer.IncreaseCredit(1_000_000);
        Manager.Instance.RegisterBuyer(buyer);

        new CartController(context).AddToCart(buyer, pencil.Id, 2);
        new OrderController(context).Checkout(buyer);

        var comment = new CommentController(context).AddComment(buyer, pencil.Id, "کیفیت خوبی داشت");
        new RequestController(context).Approve(Manager.Instance.PendingRequests.Single(r => r.Type == Core.Enums.RequestType.CommentApproval).Id);
        new RatingController(context).RateProduct(buyer, pencil.Id, 5);

        store.Save();

        // شبیه سازی راه اندازی مجدد برنامه: Singleton بازنشانی و از فایل بارگذاری می شود
        Manager.ResetForTests();
        var reloadedStore = new DataStore(_tempFile);
        reloadedStore.Load();

        var reloadedProducts = Manager.Instance.Products;
        Assert.Equal(2, reloadedProducts.Count);

        var reloadedCar = Assert.IsType<Car>(reloadedProducts.First(p => p.Id == car.Id));
        Assert.Equal("BMW", reloadedCar.Manufacturer);
        Assert.Equal(4300, reloadedCar.EngineVolume);
        Assert.True(reloadedCar.IsAutomatic);

        var reloadedPencil = reloadedProducts.First(p => p.Id == pencil.Id);
        Assert.Equal(48, reloadedPencil.StockQuantity); // ۲ عدد از ۵۰ خریداری شد
        Assert.Equal(5, reloadedPencil.AverageRating);
        Assert.Single(reloadedPencil.ApprovedComments);

        var reloadedBuyer = Manager.Instance.FindBuyer("buyer1");
        Assert.NotNull(reloadedBuyer);
        Assert.True(reloadedBuyer!.VerifyPassword("Passw0rd1"));
        Assert.Single(reloadedBuyer.Invoices);
        Assert.Equal(1_000_000 - 30_000, reloadedBuyer.Credit);
    }

    public void Dispose()
    {
        if (File.Exists(_tempFile))
        {
            File.Delete(_tempFile);
        }
    }
}
