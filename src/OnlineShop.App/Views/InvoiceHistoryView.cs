using OnlineShop.Core.Entities;

namespace OnlineShop.App.Views;

/// <summary>نمایش سابقه خرید (فاکتورها)؛ برای هر فاکتور، مشخصات هر کالا و تعداد آن نمایش داده می شود.</summary>
public static class InvoiceHistoryView
{
    public static void Show(Buyer buyer)
    {
        ConsoleUi.PrintHeader("سابقه خرید (فاکتورها)");

        if (buyer.Invoices.Count == 0)
        {
            Console.WriteLine("تا کنون خریدی ثبت نشده است.");
        }
        else
        {
            foreach (var invoice in buyer.Invoices.OrderByDescending(i => i.Date))
            {
                Console.WriteLine(invoice);
                Console.WriteLine(new string('-', 40));
            }
        }

        ConsoleUi.Pause();
    }
}
