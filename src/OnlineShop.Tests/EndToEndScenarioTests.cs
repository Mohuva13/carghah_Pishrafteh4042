using OnlineShop.Controllers;
using OnlineShop.Controllers.Catalog;
using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Products;
using OnlineShop.Core.Exceptions;
using OnlineShop.Tests.TestHelpers;

namespace OnlineShop.Tests;

/// <summary>
/// سناریوی کامل و سرتاسری پروژه، دقیقا مطابق فرآیندهای توضیح داده شده در صورت پروژه:
/// ثبت نام -> تایید مدیر -> ورود -> جستجو/فیلتر -> افزودن به سبد -> خرید ناموفق (اعتبار ناکافی)
/// -> شارژ اعتبار -> تایید مدیر -> خرید موفق -> ثبت نظر -> تایید نظر -> ثبت نمره.
/// </summary>
public class EndToEndScenarioTests
{
    [Fact]
    public void FullBuyerJourney_WorksEndToEnd()
    {
        var context = TestContextFactory.CreateFreshContext();
        var auth = new AuthController(context);
        var requests = new RequestController(context);

        // مدیر چند کالا اضافه می کند (شبیه سازی دستورات CLI)
        var cli = new Controllers.Cli.ManagerCliController(context);
        cli.Execute("Add Car 4300 true BMW X5 66000000 3");
        cli.Execute("Add Pencil HB Germany StaedtlerPencil 15000 100");

        // ۱. ثبت نام خریدار جدید
        var registrationRequest = auth.Register("newBuyer", "new@buyer.com", "09121112233", "Passw0rd1");

        // خریدار تا قبل از تایید نمی تواند وارد شود
        Assert.Throws<AuthenticationException>(() => auth.Login("newBuyer", "Passw0rd1"));

        // ۲. مدیر درخواست ثبت نام را تایید می کند
        requests.Approve(registrationRequest.Id);

        // ۳. ورود موفق خریدار
        var account = auth.Login("newBuyer", "Passw0rd1");
        var buyer = Assert.IsType<Buyer>(account);

        // ۴. جستجو و فیلتر کالاها (بدون نیاز به لاگین هم ممکن است، اینجا بعد از لاگین تست می شود)
        var catalog = new ProductCatalogController();
        var pencilResult = catalog.Search(new ProductFilter { NameContains = "Staedtler" });
        Assert.Single(pencilResult.Items);
        var pencil = pencilResult.Items[0];

        // ۵. افزودن به سبد خرید
        var cart = new CartController(context);
        cart.AddToCart(buyer, pencil.Id, 2);

        // ۶. تلاش برای خرید بدون اعتبار کافی -> باید شکست بخورد
        var order = new OrderController(context);
        Assert.Throws<InsufficientCreditException>(() => order.Checkout(buyer));

        // ۷. شارژ اعتبار حساب
        var payment = new PaymentController(context);
        var creditRequest = payment.RequestCreditCharge(buyer, "1234567812345678", "1234", "123", 100_000);
        Assert.Equal(0, buyer.Credit);

        // ۸. تایید درخواست شارژ توسط مدیر
        requests.Approve(creditRequest.Id);
        Assert.Equal(100_000, buyer.Credit);

        // ۹. خرید موفق
        var invoice = order.Checkout(buyer);
        Assert.Equal(30_000, invoice.PaidAmount);
        Assert.Equal(70_000, buyer.Credit);
        Assert.Equal(98, pencil.StockQuantity);
        Assert.Single(buyer.Invoices);

        // ۱۰. ثبت نظر برای کالای خریداری شده
        var commentController = new CommentController(context);
        var comment = commentController.AddComment(buyer, pencil.Id, "کیفیت عالی داشت");
        Assert.True(comment.HasPurchasedProduct);
        Assert.Empty(pencil.ApprovedComments);

        // ۱۱. تایید نظر توسط مدیر
        var commentRequest = requests.GetPendingRequests()
            .Single(r => r.Type == Core.Enums.RequestType.CommentApproval);
        requests.Approve(commentRequest.Id);
        Assert.Single(pencil.ApprovedComments);

        // ۱۲. ثبت نمره برای کالای خریداری شده
        new RatingController(context).RateProduct(buyer, pencil.Id, 5);
        Assert.Equal(5, pencil.AverageRating);

        // نمره دهی بدون خرید مجاز نیست
        var otherBuyer = new Buyer("outsider", "o@b.com", "09111112222", "Passw0rd1") { IsApproved = true };
        Manager.Instance.RegisterBuyer(otherBuyer);
        Assert.Throws<PurchaseRequiredException>(() => new RatingController(context).RateProduct(otherBuyer, pencil.Id, 1));
    }
}
