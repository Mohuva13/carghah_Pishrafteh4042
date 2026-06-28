using OnlineShop.Controllers;
using OnlineShop.Core.Entities;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.App.Views;

/// <summary>
/// صفحه پرداخت برای افزایش اعتبار حساب. کاربر شماره کارت، رمز و CVV2 را وارد می کند
/// که فرمت هر سه با Regex بررسی می شود؛ سپس درخواستی برای مدیر جهت تایید ارسال می شود.
/// </summary>
public static class PaymentView
{
    public static void Show(ShopContext context, Buyer buyer)
    {
        ConsoleUi.PrintHeader("شارژ اعتبار حساب کاربری");
        Console.WriteLine($"اعتبار فعلی شما: {buyer.Credit:N0}");

        var cardNumber = ConsoleUi.ReadNonEmpty("شماره کارت (۱۶ رقم)");
        var cardPassword = ConsoleUi.ReadNonEmpty("رمز دوم کارت (۴ تا ۶ رقم)");
        var cvv2 = ConsoleUi.ReadNonEmpty("CVV2 (۳ یا ۴ رقم)");
        var amount = ConsoleUi.ReadDecimal("مبلغ شارژ");

        try
        {
            var request = new PaymentController(context).RequestCreditCharge(buyer, cardNumber, cardPassword, cvv2, amount);
            ConsoleUi.PrintSuccess("اطلاعات پرداخت معتبر است.");
            ConsoleUi.PrintInfo($"درخواست افزایش اعتبار شما (شماره #{request.Id}) برای مدیر ارسال شد. پس از تایید، اعتبار حساب شما افزایش می یابد.");
        }
        catch (DomainException ex)
        {
            ConsoleUi.PrintError(ex.Message);
        }

        ConsoleUi.Pause();
    }
}
