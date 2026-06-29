using OnlineShop.Controllers.Cli;
using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Products;
using OnlineShop.Core.Exceptions;
using OnlineShop.Tests.TestHelpers;

namespace OnlineShop.Tests;

public class ManagerCliControllerTests
{
    [Fact]
    public void Execute_AddCar_MatchesProjectSpecExample()
    {
        var context = TestContextFactory.CreateFreshContext();
        var cli = new ManagerCliController(context);

        // مثال دقیق آورده شده در صورت پروژه
        cli.Execute("Add Vehicle 4300 true BMW X5 66000 3");

        var car = Assert.IsType<Car>(Manager.Instance.Products.Single());
        Assert.Equal(4300, car.EngineVolume);
        Assert.True(car.IsAutomatic);
        Assert.Equal("BMW", car.Manufacturer);
        Assert.Equal("X5", car.Name);
        Assert.Equal(66000, car.Price);
        Assert.Equal(3, car.StockQuantity);
    }

    [Fact]
    public void Execute_AddWithWrongArgumentCount_ThrowsInvalidCommandException()
    {
        var context = TestContextFactory.CreateFreshContext();
        var cli = new ManagerCliController(context);

        Assert.Throws<InvalidCommandException>(() => cli.Execute("Add Car 4300 true BMW"));
    }

    [Fact]
    public void Execute_UnknownCommand_ThrowsInvalidCommandException()
    {
        var context = TestContextFactory.CreateFreshContext();
        var cli = new ManagerCliController(context);

        Assert.Throws<InvalidCommandException>(() => cli.Execute("Fly To The Moon"));
    }

    [Fact]
    public void Execute_Help_ReturnsCommandList()
    {
        var context = TestContextFactory.CreateFreshContext();
        var cli = new ManagerCliController(context);

        var output = cli.Execute("Help");

        Assert.Contains("Add", output);
        Assert.Contains("Edit", output);
        Assert.Contains("Remove", output);
    }

    [Fact]
    public void Execute_AddPencil_ParsesEnumCorrectly()
    {
        var context = TestContextFactory.CreateFreshContext();
        var cli = new ManagerCliController(context);

        cli.Execute("Add Pencil 2H Germany \"Staedtler Pencil\" 15000 100");

        var pencil = Assert.IsType<Pencil>(Manager.Instance.Products.Single());
        Assert.Equal(Core.Enums.PencilType.TwoH, pencil.PencilType);
        Assert.Equal("Staedtler Pencil", pencil.Name);
    }

    [Fact]
    public void Execute_EditPrice_UpdatesProductPrice()
    {
        var context = TestContextFactory.CreateFreshContext();
        var product = new Pen(context.ProductIds.Next(), "Pen", 8000, 10, "France", "Blue");
        Manager.Instance.AddProduct(product);
        var cli = new ManagerCliController(context);

        cli.Execute($"Edit {product.Id} Price 9500");

        Assert.Equal(9500, product.Price);
    }

    [Fact]
    public void Execute_EditUsername_IsNotSupported()
    {
        var context = TestContextFactory.CreateFreshContext();
        var product = new Pen(context.ProductIds.Next(), "Pen", 8000, 10, "France", "Blue");
        Manager.Instance.AddProduct(product);
        var cli = new ManagerCliController(context);

        Assert.Throws<InvalidCommandException>(() => cli.Execute($"Edit {product.Id} Category Food"));
    }

    [Fact]
    public void Execute_Remove_HidesProductFromCatalogButKeepsRecord()
    {
        var context = TestContextFactory.CreateFreshContext();
        var product = new Pen(context.ProductIds.Next(), "Pen", 8000, 10, "France", "Blue");
        Manager.Instance.AddProduct(product);
        var cli = new ManagerCliController(context);

        cli.Execute($"Remove {product.Id}");

        Assert.True(product.IsDeleted);
        Assert.Contains(product, Manager.Instance.Products);
    }

    [Fact]
    public void Execute_Requests_ListsPendingRequests()
    {
        var context = TestContextFactory.CreateFreshContext();
        var buyer = new Buyer("newUser", "a@b.com", "09123456789", "Passw0rd1");
        Manager.Instance.RegisterBuyer(buyer);
        Manager.Instance.AddRequest(new Core.Entities.Requests.RegistrationRequest(context.RequestIds.Next(), buyer));

        var cli = new ManagerCliController(context);
        var output = cli.Execute("Requests");

        Assert.Contains("newUser", output);
    }

    [Fact]
    public void Execute_ApproveAndReject_ChangeRequestStatus()
    {
        var context = TestContextFactory.CreateFreshContext();
        var buyer = new Buyer("newUser", "a@b.com", "09123456789", "Passw0rd1");
        Manager.Instance.RegisterBuyer(buyer);
        var request = new Core.Entities.Requests.RegistrationRequest(context.RequestIds.Next(), buyer);
        Manager.Instance.AddRequest(request);

        var cli = new ManagerCliController(context);
        cli.Execute($"Approve {request.Id}");

        Assert.True(buyer.IsApproved);
    }

    [Fact]
    public void Execute_Users_ListsAllRegisteredBuyers()
    {
        var context = TestContextFactory.CreateFreshContext();
        Manager.Instance.RegisterBuyer(new Buyer("userOne", "a@b.com", "09123456789", "Passw0rd1"));

        var cli = new ManagerCliController(context);
        var output = cli.Execute("Users");

        Assert.Contains("userOne", output);
    }
}
