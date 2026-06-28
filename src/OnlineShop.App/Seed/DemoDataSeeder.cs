using OnlineShop.Controllers;
using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Products;
using OnlineShop.Core.Entities.Requests;
using OnlineShop.Core.Enums;

namespace OnlineShop.App.Seed;

/// <summary>
/// در اولین اجرای برنامه (وقتی هنوز فایل داده ای وجود ندارد)، چند کالای نمونه از هر دسته
/// و یک خریدار تایید شده برای تست سریع امکانات ایجاد می کند تا نیازی به ثبت نام دستی نباشد.
/// </summary>
public static class DemoDataSeeder
{
    public static void Seed(ShopContext context)
    {
        var manager = Manager.Instance;

        manager.AddProduct(new FlashMemory(context.ProductIds.Next(), "Kingston DataTraveler 64GB", 350_000, 25, 10, "5x1x1 cm", 64, "3.2"));
        manager.AddProduct(new Ssd(context.ProductIds.Next(), "Samsung 970 EVO 1TB", 3_200_000, 15, 8, "8x2x0.3 cm", 1000, 3500, 3000));
        manager.AddProduct(new PersonalComputer(context.ProductIds.Next(), "ASUS ROG Desktop", 65_000_000, 4, 9000, "45x20x45 cm", "Intel Core i7-13700K", 32));
        manager.AddProduct(new Pencil(context.ProductIds.Next(), "مداد استدلر", 15_000, 200, "آلمان", PencilType.HB));
        manager.AddProduct(new Pencil(context.ProductIds.Next(), "مداد فابر کاستل طراحی", 25_000, 80, "آلمان", PencilType.TwoH));
        manager.AddProduct(new Pen(context.ProductIds.Next(), "خودکار بیک آبی", 8_000, 500, "فرانسه", "آبی"));
        manager.AddProduct(new Notebook(context.ProductIds.Next(), "دفتر ۱۰۰ برگ", 45_000, 150, "ایران", 100, "تحریر"));
        manager.AddProduct(new Bicycle(context.ProductIds.Next(), "دوچرخه کوهستان جاینت", 28_000_000, 6, "Giant", BicycleType.Mountain));
        manager.AddProduct(new Bicycle(context.ProductIds.Next(), "دوچرخه شهری بیانچی", 19_500_000, 0, "Bianchi", BicycleType.Urban));
        manager.AddProduct(new Car(context.ProductIds.Next(), "X5", 3_600_000_000m, 3, "BMW", 4300, true));
        manager.AddProduct(new Car(context.ProductIds.Next(), "206", 850_000_000m, 10, "پژو", 1600, false));
        manager.AddProduct(new Food(context.ProductIds.Next(), "شکلات تلخ ۷۰٪", 120_000, 60,
            DateTime.Today.AddDays(-10), DateTime.Today.AddMonths(8)));
        manager.AddProduct(new Food(context.ProductIds.Next(), "شیر کم چرب", 35_000, 40,
            DateTime.Today.AddDays(-3), DateTime.Today.AddDays(10)));

        var approvedBuyer = new Buyer("ali_reza", "ali.reza@example.com", "09121234567", "Passw0rd1")
        {
            IsApproved = true
        };
        approvedBuyer.IncreaseCredit(50_000_000);
        manager.RegisterBuyer(approvedBuyer);

        var pendingBuyer = new Buyer("sara99", "sara99@example.com", "09359876543", "Sara@1234");
        manager.RegisterBuyer(pendingBuyer);
        manager.AddRequest(new RegistrationRequest(context.RequestIds.Next(), pendingBuyer));

        context.SaveChanges();
    }
}
