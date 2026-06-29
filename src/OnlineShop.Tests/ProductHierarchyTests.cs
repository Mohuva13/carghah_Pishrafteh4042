using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Products;
using OnlineShop.Core.Enums;

namespace OnlineShop.Tests;

public class ProductHierarchyTests
{
    [Fact]
    public void Car_IsAssignableTo_VehicleAndProduct()
    {
        var car = new Car(1, "X5", 66000, 3, "BMW", 4300, true);

        Assert.IsAssignableFrom<Vehicle>(car);
        Assert.IsAssignableFrom<Product>(car);
        Assert.Equal(Category.Vehicle, car.Category);
    }

    [Fact]
    public void FlashMemory_IsAssignableTo_StorageDeviceAndDigitalGood()
    {
        var flash = new FlashMemory(1, "Kingston", 350000, 10, 10, "5x1x1", 64, "3.2");

        Assert.IsAssignableFrom<StorageDevice>(flash);
        Assert.IsAssignableFrom<DigitalGood>(flash);
        Assert.Equal(Category.DigitalGood, flash.Category);
    }

    [Fact]
    public void Food_ThrowsWhenExpirationIsBeforeProduction()
    {
        Assert.Throws<Core.Exceptions.DomainException>(() =>
            new Food(1, "Milk", 10000, 5, DateTime.Today, DateTime.Today.AddDays(-1)));
    }

    [Fact]
    public void Food_IsExpired_TrueWhenPastExpirationDate()
    {
        var expired = new Food(1, "Old Bread", 5000, 5, DateTime.Today.AddDays(-30), DateTime.Today.AddDays(-1));
        var fresh = new Food(2, "Fresh Bread", 5000, 5, DateTime.Today, DateTime.Today.AddDays(5));

        Assert.True(expired.IsExpired);
        Assert.False(fresh.IsExpired);
    }

    [Fact]
    public void Product_AverageRating_IsZeroWithNoRatings()
    {
        var pen = new Pen(1, "Pen", 8000, 10, "France", "Blue");
        Assert.Equal(0, pen.AverageRating);
    }

    [Fact]
    public void Product_AverageRating_ComputesCorrectly()
    {
        var pen = new Pen(1, "Pen", 8000, 10, "France", "Blue");
        pen.AddOrUpdateRating(new Rating("user1", 1, 4));
        pen.AddOrUpdateRating(new Rating("user2", 1, 2));

        Assert.Equal(3, pen.AverageRating);
    }

    [Fact]
    public void Product_AddOrUpdateRating_ReplacesExistingRatingFromSameUser()
    {
        var pen = new Pen(1, "Pen", 8000, 10, "France", "Blue");
        pen.AddOrUpdateRating(new Rating("user1", 1, 2));
        pen.AddOrUpdateRating(new Rating("user1", 1, 5));

        Assert.Single(pen.Ratings);
        Assert.Equal(5, pen.AverageRating);
    }

    [Fact]
    public void Product_ApprovedComments_OnlyReturnsApprovedOnes()
    {
        var pen = new Pen(1, "Pen", 8000, 10, "France", "Blue");
        var pending = new Comment(1, "user1", 1, "خوب بود", true);
        var approved = new Comment(2, "user2", 1, "عالی بود", false);
        approved.Approve();

        pen.AddComment(pending);
        pen.AddComment(approved);

        Assert.Single(pen.ApprovedComments);
        Assert.Equal("عالی بود", pen.ApprovedComments.First().Text);
    }

    [Fact]
    public void Product_IsAvailable_FalseWhenStockIsZeroOrDeleted()
    {
        var pen = new Pen(1, "Pen", 8000, 0, "France", "Blue");
        Assert.False(pen.IsAvailable);

        pen.StockQuantity = 5;
        Assert.True(pen.IsAvailable);

        pen.IsDeleted = true;
        Assert.False(pen.IsAvailable);
    }

    [Fact]
    public void Pencil_GetSpecificDetails_ContainsPencilTypeAndCountry()
    {
        var pencil = new Pencil(1, "Pencil", 15000, 20, "Germany", PencilType.TwoH);
        Assert.Contains("2H", pencil.GetSpecificDetails());
        Assert.Contains("Germany", pencil.GetSpecificDetails());
    }
}
