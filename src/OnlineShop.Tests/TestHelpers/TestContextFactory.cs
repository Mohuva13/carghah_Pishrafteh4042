using OnlineShop.Controllers;
using OnlineShop.Core.Entities;
using OnlineShop.Data.Persistence;

namespace OnlineShop.Tests.TestHelpers;

/// <summary>
/// چون Manager به صورت Singleton پیاده سازی شده و بین تست ها مشترک است، این کلاس کمکی
/// قبل از هر تست، وضعیت آن را بازنشانی کرده و یک ShopContext تازه (با فایل داده موقت) می سازد.
/// </summary>
public static class TestContextFactory
{
    public static ShopContext CreateFreshContext()
    {
        Manager.ResetForTests();
        var tempFile = Path.Combine(Path.GetTempPath(), $"onlineshop-tests-{Guid.NewGuid():N}.json");
        var store = new DataStore(tempFile);
        return new ShopContext(store);
    }
}
