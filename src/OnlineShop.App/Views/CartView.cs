using OnlineShop.Controllers;
using OnlineShop.Core.Entities;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.App.Views;

/// <summary>مشاهده سبد خرید و نهایی کردن آن. اگر کاربر لاگین نکرده باشد، سبد موقت مهمان نمایش داده می شود.</summary>
public static class CartView
{
    public static void Show(ShopContext context, ShoppingCart guestCart)
    {
        while (true)
        {
            var buyer = context.CurrentBuyer;
            var cart = buyer?.Cart ?? guestCart;

            ConsoleUi.PrintHeader("سبد خرید");
            if (cart.IsEmpty)
            {
                Console.WriteLine("سبد خرید شما خالی است.");
            }
            else
            {
                foreach (var item in cart.Items)
                {
                    Console.WriteLine($"#{item.Product.Id} - {item}");
                }

                Console.WriteLine($"جمع کل: {cart.TotalAmount:N0}");
            }

            Console.WriteLine();
            Console.WriteLine("1. حذف یک کالا از سبد");
            Console.WriteLine("2. نهایی کردن خرید");
            Console.WriteLine("0. بازگشت");
            Console.Write("انتخاب شما: ");

            switch (ConsoleUi.ReadMenuChoice())
            {
                case "1":
                    RemoveItem(context, guestCart, cart);
                    break;
                case "2":
                    Checkout(context, guestCart);
                    break;
                case "0":
                    return;
                default:
                    ConsoleUi.PrintError("گزینه نامعتبر است.");
                    break;
            }
        }
    }

    private static void RemoveItem(ShopContext context, ShoppingCart guestCart, ShoppingCart cart)
    {
        if (cart.IsEmpty)
        {
            ConsoleUi.PrintError("سبد خرید خالی است.");
            ConsoleUi.Pause();
            return;
        }

        var id = ConsoleUi.ReadInt("شناسه کالایی که می خواهید حذف کنید");
        var buyer = context.CurrentBuyer;
        if (buyer is null)
        {
            guestCart.RemoveProduct(id);
        }
        else
        {
            new CartController(context).RemoveFromCart(buyer, id);
        }

        ConsoleUi.PrintSuccess("کالا از سبد خرید حذف شد.");
        ConsoleUi.Pause();
    }

    private static void Checkout(ShopContext context, ShoppingCart guestCart)
    {
        var buyer = context.CurrentBuyer;

        if (buyer is null)
        {
            ConsoleUi.PrintInfo("برای نهایی کردن خرید ابتدا باید وارد سامانه شوید.");
            if (!ConsoleUi.ReadYesNo("می خواهید وارد شوید؟ (در صورت نداشتن حساب، از منوی اصلی ثبت نام کنید)"))
            {
                return;
            }

            var account = LoginView.TryLogin(context);
            if (account is not Buyer loggedInBuyer)
            {
                ConsoleUi.PrintError("ورود ناموفق بود یا حساب مدیر امکان خرید ندارد.");
                ConsoleUi.Pause();
                return;
            }

            // ادغام سبد خرید موقت مهمان با سبد خرید واقعی خریدار پس از ورود موفق
            foreach (var item in guestCart.Items)
            {
                loggedInBuyer.Cart.AddProduct(item.Product, item.Quantity);
            }

            guestCart.Clear();
            buyer = loggedInBuyer;
        }

        try
        {
            var invoice = new OrderController(context).Checkout(buyer);
            ConsoleUi.PrintSuccess("خرید شما با موفقیت نهایی شد.");
            Console.WriteLine(invoice);
        }
        catch (DomainException ex)
        {
            ConsoleUi.PrintError(ex.Message);
        }

        ConsoleUi.Pause();
    }
}
