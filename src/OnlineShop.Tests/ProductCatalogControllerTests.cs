using OnlineShop.Controllers.Catalog;
using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Products;
using OnlineShop.Core.Enums;
using OnlineShop.Tests.TestHelpers;

namespace OnlineShop.Tests;

public class ProductCatalogControllerTests
{
    [Fact]
    public void Search_Paginates_TenItemsPerPageByDefault()
    {
        TestContextFactory.CreateFreshContext();
        for (var i = 0; i < 25; i++)
        {
            Manager.Instance.AddProduct(new Pen(i + 1, $"Pen{i}", 1000, 10, "France", "Blue"));
        }

        var catalog = new ProductCatalogController();
        var page1 = catalog.Search(null, 1);
        var page3 = catalog.Search(null, 3);

        Assert.Equal(10, page1.Items.Count);
        Assert.Equal(5, page3.Items.Count);
        Assert.Equal(3, page1.TotalPages);
        Assert.Equal(25, page1.TotalCount);
    }

    [Fact]
    public void Search_DeletedProducts_AreExcludedFromCatalog()
    {
        TestContextFactory.CreateFreshContext();
        var product = new Pen(1, "Pen", 1000, 10, "France", "Blue") { IsDeleted = true };
        Manager.Instance.AddProduct(product);

        var catalog = new ProductCatalogController();
        var result = catalog.Search(null);

        Assert.Empty(result.Items);
    }

    [Fact]
    public void Search_FiltersByCategory()
    {
        TestContextFactory.CreateFreshContext();
        Manager.Instance.AddProduct(new Pen(1, "Pen", 1000, 10, "France", "Blue"));
        Manager.Instance.AddProduct(new Bicycle(2, "Bike", 500000, 5, "Giant", BicycleType.Mountain));

        var catalog = new ProductCatalogController();
        var result = catalog.Search(new ProductFilter { Category = Category.Vehicle });

        Assert.Single(result.Items);
        Assert.IsType<Bicycle>(result.Items[0]);
    }

    [Fact]
    public void Search_FiltersByAvailability()
    {
        TestContextFactory.CreateFreshContext();
        Manager.Instance.AddProduct(new Pen(1, "Pen A", 1000, 0, "France", "Blue"));
        Manager.Instance.AddProduct(new Pen(2, "Pen B", 1000, 5, "France", "Blue"));

        var catalog = new ProductCatalogController();
        var result = catalog.Search(new ProductFilter { OnlyAvailable = true });

        Assert.Single(result.Items);
        Assert.Equal("Pen B", result.Items[0].Name);
    }

    [Fact]
    public void Search_FiltersByPriceRange()
    {
        TestContextFactory.CreateFreshContext();
        Manager.Instance.AddProduct(new Pen(1, "Cheap", 1000, 10, "France", "Blue"));
        Manager.Instance.AddProduct(new Pen(2, "Mid", 5000, 10, "France", "Blue"));
        Manager.Instance.AddProduct(new Pen(3, "Expensive", 20000, 10, "France", "Blue"));

        var catalog = new ProductCatalogController();
        var result = catalog.Search(new ProductFilter { MinPrice = 2000, MaxPrice = 10000 });

        Assert.Single(result.Items);
        Assert.Equal("Mid", result.Items[0].Name);
    }

    [Fact]
    public void Search_FiltersByRatingRange()
    {
        TestContextFactory.CreateFreshContext();
        var low = new Pen(1, "LowRated", 1000, 10, "France", "Blue");
        var high = new Pen(2, "HighRated", 1000, 10, "France", "Blue");
        low.AddOrUpdateRating(new Rating("u1", 1, 1));
        high.AddOrUpdateRating(new Rating("u1", 2, 5));
        Manager.Instance.AddProduct(low);
        Manager.Instance.AddProduct(high);

        var catalog = new ProductCatalogController();
        var result = catalog.Search(new ProductFilter { MinRating = 4 });

        Assert.Single(result.Items);
        Assert.Equal("HighRated", result.Items[0].Name);
    }

    [Fact]
    public void Search_FiltersByNameSubstring_CaseInsensitive()
    {
        TestContextFactory.CreateFreshContext();
        Manager.Instance.AddProduct(new Pen(1, "Blue Pen", 1000, 10, "France", "Blue"));
        Manager.Instance.AddProduct(new Pen(2, "Red Marker", 1000, 10, "France", "Red"));

        var catalog = new ProductCatalogController();
        var result = catalog.Search(new ProductFilter { NameContains = "blue" });

        Assert.Single(result.Items);
        Assert.Equal("Blue Pen", result.Items[0].Name);
    }

    [Fact]
    public void Search_FiltersByPencilType()
    {
        TestContextFactory.CreateFreshContext();
        Manager.Instance.AddProduct(new Pencil(1, "HB Pencil", 1000, 10, "Germany", PencilType.HB));
        Manager.Instance.AddProduct(new Pencil(2, "2H Pencil", 1000, 10, "Germany", PencilType.TwoH));

        var catalog = new ProductCatalogController();
        var result = catalog.Search(new ProductFilter { PencilType = PencilType.TwoH });

        Assert.Single(result.Items);
        Assert.Equal("2H Pencil", result.Items[0].Name);
    }

    [Fact]
    public void Search_FiltersByCarAutomatic()
    {
        TestContextFactory.CreateFreshContext();
        Manager.Instance.AddProduct(new Car(1, "Automatic Car", 1_000_000, 2, "BMW", 2000, true));
        Manager.Instance.AddProduct(new Car(2, "Manual Car", 1_000_000, 2, "BMW", 2000, false));

        var catalog = new ProductCatalogController();
        var result = catalog.Search(new ProductFilter { IsAutomatic = true });

        Assert.Single(result.Items);
        Assert.Equal("Automatic Car", result.Items[0].Name);
    }

    [Fact]
    public void Search_ExcludesExpiredFood_WhenRequested()
    {
        TestContextFactory.CreateFreshContext();
        Manager.Instance.AddProduct(new Food(1, "Expired", 1000, 10, DateTime.Today.AddDays(-30), DateTime.Today.AddDays(-1)));
        Manager.Instance.AddProduct(new Food(2, "Fresh", 1000, 10, DateTime.Today, DateTime.Today.AddDays(30)));

        var catalog = new ProductCatalogController();
        var result = catalog.Search(new ProductFilter { ExcludeExpiredFood = true });

        Assert.Single(result.Items);
        Assert.Equal("Fresh", result.Items[0].Name);
    }
}
