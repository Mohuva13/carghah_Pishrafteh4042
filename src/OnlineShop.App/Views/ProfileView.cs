using OnlineShop.Controllers;
using OnlineShop.Core.Entities;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.App.Views;

/// <summary>مشاهده اطلاعات شخصی خریدار و ویرایش آن ها (به جز نام کاربری).</summary>
public static class ProfileView
{
    public static void Show(ShopContext context, Buyer buyer)
    {
        var accountController = new AccountController(context);

        while (true)
        {
            ConsoleUi.PrintHeader("اطلاعات حساب کاربری من");
            Console.WriteLine(buyer);
            Console.WriteLine();
            Console.WriteLine("1. ویرایش ایمیل");
            Console.WriteLine("2. ویرایش شماره تلفن");
            Console.WriteLine("3. تغییر رمز عبور");
            Console.WriteLine("0. بازگشت");
            Console.Write("انتخاب شما: ");

            try
            {
                switch (ConsoleUi.ReadMenuChoice())
                {
                    case "1":
                        accountController.UpdateEmail(buyer, ConsoleUi.ReadNonEmpty("ایمیل جدید"));
                        ConsoleUi.PrintSuccess("ایمیل با موفقیت بروزرسانی شد.");
                        ConsoleUi.Pause();
                        break;
                    case "2":
                        accountController.UpdatePhoneNumber(buyer, ConsoleUi.ReadNonEmpty("شماره تلفن جدید"));
                        ConsoleUi.PrintSuccess("شماره تلفن با موفقیت بروزرسانی شد.");
                        ConsoleUi.Pause();
                        break;
                    case "3":
                        accountController.UpdatePassword(buyer, ConsoleUi.ReadNonEmpty("رمز عبور جدید"));
                        ConsoleUi.PrintSuccess("رمز عبور با موفقیت بروزرسانی شد.");
                        ConsoleUi.Pause();
                        break;
                    case "0":
                        return;
                    default:
                        ConsoleUi.PrintError("گزینه نامعتبر است.");
                        break;
                }
            }
            catch (DomainException ex)
            {
                ConsoleUi.PrintError(ex.Message);
                ConsoleUi.Pause();
            }
        }
    }
}
