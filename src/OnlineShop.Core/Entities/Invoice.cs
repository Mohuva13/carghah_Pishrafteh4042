namespace OnlineShop.Core.Entities;

/// <summary>فاکتور خرید یک خریدار پس از نهایی شدن سبد خرید.</summary>
public sealed class Invoice
{
    public Invoice(int id, string buyerUsername, IEnumerable<InvoiceItem> items)
    {
        Id = id;
        BuyerUsername = buyerUsername;
        Date = DateTime.Now;
        Items = items.ToList();
        PaidAmount = Items.Sum(i => i.LineTotal);
    }

    public int Id { get; }

    public string BuyerUsername { get; }

    public DateTime Date { get; }

    public decimal PaidAmount { get; }

    public IReadOnlyList<InvoiceItem> Items { get; }

    public override string ToString()
    {
        var itemsText = string.Join(Environment.NewLine + "    ", Items);
        return $"فاکتور #{Id} | تاریخ: {Date:yyyy/MM/dd HH:mm} | مبلغ پرداختی: {PaidAmount:N0}{Environment.NewLine}    {itemsText}";
    }
}
