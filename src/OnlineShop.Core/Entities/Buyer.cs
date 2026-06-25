using OnlineShop.Core.Exceptions;

namespace OnlineShop.Core.Entities;

/// <summary>
/// نقش خریدار. هر خریدار دارای سبد خرید، سابقه خرید (فاکتورها) و اعتبار حساب کاربری است.
/// </summary>
public sealed class Buyer : Account
{
    private decimal _credit;
    private readonly List<Invoice> _invoices = new();

    public Buyer(string username, string email, string phoneNumber, string password)
        : base(username, email, phoneNumber, password)
    {
        Cart = new ShoppingCart();
    }

    public override string RoleName => "خریدار";

    /// <summary>پس از ثبت نام، تا زمان تایید درخواست توسط مدیر، حساب فعال نیست.</summary>
    public bool IsApproved { get; set; }

    public ShoppingCart Cart { get; }

    public decimal Credit
    {
        get => _credit;
        private set => _credit = value;
    }

    public IReadOnlyList<Invoice> Invoices => _invoices.AsReadOnly();

    public void IncreaseCredit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new DomainException("مبلغ شارژ باید بزرگتر از صفر باشد.");
        }

        Credit += amount;
    }

    public void PayFromCredit(decimal amount)
    {
        if (amount > Credit)
        {
            throw new InsufficientCreditException(Credit, amount);
        }

        Credit -= amount;
    }

    public void AddInvoice(Invoice invoice) => _invoices.Add(invoice);

    /// <summary>آیا این خریدار تا کنون کالای مورد نظر را (در هر یک از فاکتورهایش) خریداری کرده است.</summary>
    public bool HasPurchased(int productId) =>
        _invoices.Any(inv => inv.Items.Any(i => i.ProductId == productId));

    public override string ToString() =>
        $"{base.ToString()} | اعتبار: {Credit:N0} | تعداد سبد خرید: {Cart.Items.Count} | تعداد فاکتور: {_invoices.Count} | تایید شده: {(IsApproved ? "بله" : "خیر")}";
}
