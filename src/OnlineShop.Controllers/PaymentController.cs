using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Requests;
using OnlineShop.Core.Exceptions;
using OnlineShop.Core.Utils;

namespace OnlineShop.Controllers;

/// <summary>
/// مسئول فرآیند شارژ اعتبار حساب کاربری. فرمت شماره کارت، رمز کارت و CVV2 با Regex بررسی می شود
/// و در صورت معتبر بودن، یک درخواست افزایش اعتبار برای مدیر ارسال می گردد.
/// </summary>
public sealed class PaymentController
{
    private readonly ShopContext _context;

    public PaymentController(ShopContext context)
    {
        _context = context;
    }

    public CreditChargeRequest RequestCreditCharge(Buyer buyer, string cardNumber, string cardPassword, string cvv2, decimal amount)
    {
        var normalizedCardNumber = cardNumber.Replace(" ", string.Empty).Replace("-", string.Empty);

        if (!RegexValidators.IsValidCardNumber(cardNumber))
        {
            throw new InvalidFormatException("شماره کارت", cardNumber);
        }

        if (!RegexValidators.IsValidCardPassword(cardPassword))
        {
            throw new InvalidFormatException("رمز کارت", cardPassword);
        }

        if (!RegexValidators.IsValidCvv2(cvv2))
        {
            throw new InvalidFormatException("CVV2", cvv2);
        }

        if (amount <= 0)
        {
            throw new DomainException("مبلغ شارژ باید بزرگتر از صفر باشد.");
        }

        var request = new CreditChargeRequest(_context.RequestIds.Next(), buyer, amount, normalizedCardNumber);
        Manager.Instance.AddRequest(request);
        _context.SaveChanges();
        return request;
    }
}
