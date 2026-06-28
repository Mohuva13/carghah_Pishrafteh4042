using OnlineShop.Controllers;
using OnlineShop.Core.Entities;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.App.Views;

/// <summary>صفحه لاگین؛ کاربر بر اساس نقش خود (مدیر یا خریدار) به ناحیه کاربری مربوطه هدایت می شود.</summary>
public static class LoginView
{
    /// <summary>تلاش برای ورود؛ در صورت موفقیت، حساب کاربری واردشده را برمی گرداند، در غیر این صورت null.</summary>
    public static Account? TryLogin(ShopContext context)
    {
        ConsoleUi.PrintHeader("ورود به سامانه");
        var authController = new AuthController(context);

        while (true)
        {
            var username = ConsoleUi.ReadNonEmpty("نام کاربری");
            var password = ConsoleUi.ReadNonEmpty("رمز عبور");

            try
            {
                var account = authController.Login(username, password);
                ConsoleUi.PrintSuccess($"خوش آمدید {account.Username}!");
                return account;
            }
            catch (AuthenticationException ex)
            {
                ConsoleUi.PrintError(ex.Message);
                if (!ConsoleUi.ReadYesNo("می خواهید دوباره تلاش کنید؟"))
                {
                    return null;
                }
            }
        }
    }
}
