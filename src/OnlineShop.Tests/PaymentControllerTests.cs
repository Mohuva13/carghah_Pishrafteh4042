using OnlineShop.Controllers;
using OnlineShop.Core.Entities;
using OnlineShop.Core.Exceptions;
using OnlineShop.Tests.TestHelpers;

namespace OnlineShop.Tests;

public class PaymentControllerTests
{
    private static (ShopContext context, Buyer buyer) Setup()
    {
        var context = TestContextFactory.CreateFreshContext();
        var buyer = new Buyer("buyer1", "b@b.com", "09123456789", "Passw0rd1") { IsApproved = true };
        Manager.Instance.RegisterBuyer(buyer);
        return (context, buyer);
    }

    [Fact]
    public void RequestCreditCharge_ThrowsOnInvalidCardNumber()
    {
        var (context, buyer) = Setup();
        Assert.Throws<InvalidFormatException>(() =>
            new PaymentController(context).RequestCreditCharge(buyer, "123", "1234", "123", 100_000));
    }

    [Fact]
    public void RequestCreditCharge_ThrowsOnInvalidCvv2()
    {
        var (context, buyer) = Setup();
        Assert.Throws<InvalidFormatException>(() =>
            new PaymentController(context).RequestCreditCharge(buyer, "1234567812345678", "1234", "12345", 100_000));
    }

    [Fact]
    public void RequestCreditCharge_CreatesPendingRequest_AndDoesNotChangeCreditYet()
    {
        var (context, buyer) = Setup();

        var request = new PaymentController(context).RequestCreditCharge(buyer, "1234567812345678", "1234", "123", 200_000);

        Assert.Equal(0, buyer.Credit);
        Assert.Equal(Core.Enums.RequestStatus.Pending, request.Status);
        Assert.Contains(Manager.Instance.PendingRequests, r => r.Id == request.Id);
    }

    [Fact]
    public void RequestCreditCharge_ApprovedByManager_IncreasesCredit()
    {
        var (context, buyer) = Setup();
        var request = new PaymentController(context).RequestCreditCharge(buyer, "1234567812345678", "1234", "123", 200_000);

        new RequestController(context).Approve(request.Id);

        Assert.Equal(200_000, buyer.Credit);
    }
}
