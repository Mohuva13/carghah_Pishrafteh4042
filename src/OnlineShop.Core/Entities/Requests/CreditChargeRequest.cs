using OnlineShop.Core.Enums;

namespace OnlineShop.Core.Entities.Requests;

/// <summary>
/// درخواست افزایش اعتبار حساب کاربری. فرمت شماره کارت، رمز و CVV2 پیش از ساخت این درخواست
/// توسط Regex بررسی شده است (<see cref="Utils.RegexValidators"/>). به دلایل امنیتی، فقط
/// ۴ رقم آخر شماره کارت نگه داری می شود و CVV2/رمز کارت اصلا ذخیره نمی گردد.
/// </summary>
public sealed class CreditChargeRequest : RequestBase
{
    public CreditChargeRequest(int id, Buyer buyer, decimal amount, string cardNumber)
        : base(id, RequestType.CreditCharge, buyer.Username)
    {
        Buyer = buyer;
        Amount = amount;
        MaskedCardNumber = "**** **** **** " + cardNumber[^4..];
    }

    /// <summary>سازنده مخصوص بازیابی از فایل داده؛ شماره کارت اصلی ذخیره نمی شد، فقط نسخه ماسک شده موجود است.</summary>
    public CreditChargeRequest(int id, Buyer buyer, decimal amount, string maskedCardNumber, RequestStatus status, DateTime createdAt)
        : base(id, RequestType.CreditCharge, buyer.Username, status, createdAt)
    {
        Buyer = buyer;
        Amount = amount;
        MaskedCardNumber = maskedCardNumber;
    }

    public Buyer Buyer { get; }

    public decimal Amount { get; }

    public string MaskedCardNumber { get; }

    protected override void OnApprove() => Buyer.IncreaseCredit(Amount);

    public override string GetSummary() =>
        $"افزایش اعتبار به مبلغ {Amount:N0} از کارت {MaskedCardNumber}";
}
