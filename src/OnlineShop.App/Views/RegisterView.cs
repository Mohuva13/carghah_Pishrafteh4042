using OnlineShop.Controllers;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.App.Views;

/// <summary>صفحه ثبت نام؛ فقط برای خریدار قابل استفاده است.</summary>
public static class RegisterView
{
    public static void Show(ShopContext context)
    {
        ConsoleUi.PrintHeader("ثبت نام خریدار جدید");
        var authController = new AuthController(context);

        while (true)
        {
            try
            {
                var username = ConsoleUi.ReadNonEmpty("نام کاربری (حداقل ۳ حرف، با حرف شروع شود)");
                var email = ConsoleUi.ReadNonEmpty("ایمیل");
                var phone = ConsoleUi.ReadNonEmpty("شماره تلفن (مثال: 09123456789)");
                var password = ConsoleUi.ReadNonEmpty("رمز عبور (حداقل ۸ کاراکتر، شامل حرف و عدد)");

                var request = authController.Register(username, email, phone, password);
                ConsoleUi.PrintSuccess("ثبت نام شما با موفقیت انجام شد.");
                ConsoleUi.PrintInfo($"درخواست ثبت نام شما (شماره #{request.Id}) برای مدیر ارسال شد. پس از تایید می توانید وارد شوید.");
                break;
            }
            catch (DomainException ex)
            {
                ConsoleUi.PrintError(ex.Message);
                if (!ConsoleUi.ReadYesNo("می خواهید دوباره تلاش کنید؟"))
                {
                    break;
                }
            }
        }

        ConsoleUi.Pause();
    }
}
