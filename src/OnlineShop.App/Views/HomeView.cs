using OnlineShop.Controllers;
using OnlineShop.Core.Entities;

namespace OnlineShop.App.Views;

/// <summary>صفحه اصلی برنامه: راهی به سمت ثبت نام، ورود یا مشاهده محصولات (بدون نیاز به ورود).</summary>
public static class HomeView
{
    public static void Run(ShopContext context)
    {
        // سبد خرید موقت برای کاربر مهمان که هنوز لاگین نکرده است.
        var guestCart = new ShoppingCart();

        while (true)
        {
            ConsoleUi.PrintHeader("فروشگاه آنلاین - صفحه اصلی");
            Console.WriteLine("1. ثبت نام (خریدار)");
            Console.WriteLine("2. ورود به ناحیه کاربری");
            Console.WriteLine("3. مشاهده محصولات فروشگاه");
            Console.WriteLine("0. خروج از برنامه");
            Console.Write("انتخاب شما: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    RegisterView.Show(context);
                    break;
                case "2":
                    HandleLogin(context);
                    break;
                case "3":
                    ProductBrowseView.Show(context, guestCart);
                    break;
                case "0":
                    context.SaveChanges();
                    ConsoleUi.PrintInfo("با تشکر از استفاده شما از فروشگاه آنلاین. خدانگهدار!");
                    return;
                default:
                    ConsoleUi.PrintError("گزینه نامعتبر است.");
                    break;
            }
        }
    }

    private static void HandleLogin(ShopContext context)
    {
        var account = LoginView.TryLogin(context);
        switch (account)
        {
            case Manager manager:
                ManagerAreaView.Show(context);
                break;
            case Buyer buyer:
                BuyerAreaView.Show(context, buyer);
                break;
        }
    }
}
