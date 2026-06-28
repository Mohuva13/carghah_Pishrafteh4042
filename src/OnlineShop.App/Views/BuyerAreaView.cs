using OnlineShop.Controllers;
using OnlineShop.Core.Entities;

namespace OnlineShop.App.Views;

/// <summary>ناحیه کاربری خریدار: منوی امکانات پس از ورود موفق.</summary>
public static class BuyerAreaView
{
    public static void Show(ShopContext context, Buyer buyer)
    {
        // سبد خرید موقت مهمان اینجا استفاده نمی شود چون کاربر لاگین کرده و سبد واقعی خودش را دارد؛
        // اما امضای متدهای مشترک با ProductBrowseView به یک ShoppingCart نیاز دارد.
        var unusedGuestCart = new ShoppingCart();

        while (true)
        {
            ConsoleUi.PrintHeader($"ناحیه کاربری خریدار - {buyer.Username}");
            Console.WriteLine($"اعتبار حساب: {buyer.Credit:N0}");
            Console.WriteLine();
            Console.WriteLine("1. مشاهده و ویرایش اطلاعات شخصی");
            Console.WriteLine("2. مشاهده کالاها / جستجو / فیلتر / خرید");
            Console.WriteLine("3. افزایش اعتبار حساب");
            Console.WriteLine("4. نمایش سابقه خرید (فاکتورها)");
            Console.WriteLine("5. مشاهده سبد خرید فعلی");
            Console.WriteLine("0. خروج از حساب کاربری");
            Console.Write("انتخاب شما: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    ProfileView.Show(context, buyer);
                    break;
                case "2":
                    ProductBrowseView.Show(context, unusedGuestCart);
                    break;
                case "3":
                    PaymentView.Show(context, buyer);
                    break;
                case "4":
                    InvoiceHistoryView.Show(buyer);
                    break;
                case "5":
                    CartView.Show(context, unusedGuestCart);
                    break;
                case "0":
                    new AuthController(context).Logout();
                    return;
                default:
                    ConsoleUi.PrintError("گزینه نامعتبر است.");
                    break;
            }
        }
    }
}
