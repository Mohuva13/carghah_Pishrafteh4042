using OnlineShop.Core.Entities;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.Controllers;

/// <summary>مسئول نهایی کردن سبد خرید: بررسی اعتبار، کسر از حساب، بروزرسانی موجودی و ساخت فاکتور.</summary>
public sealed class OrderController
{
    private readonly ShopContext _context;

    public OrderController(ShopContext context)
    {
        _context = context;
    }

    public Invoice Checkout(Buyer buyer)
    {
        if (buyer.Cart.IsEmpty)
        {
            throw new DomainException("سبد خرید شما خالی است.");
        }

        // بررسی کافی بودن موجودی همه کالاها پیش از هرگونه تغییر در سیستم
        foreach (var item in buyer.Cart.Items)
        {
            var product = Manager.Instance.FindProduct(item.Product.Id) ?? throw new EntityNotFoundException("کالا", item.Product.Id);
            if (product.StockQuantity < item.Quantity)
            {
                throw new InsufficientStockException(product.Name, product.StockQuantity, item.Quantity);
            }
        }

        var totalAmount = buyer.Cart.TotalAmount;
        if (totalAmount > buyer.Credit)
        {
            throw new InsufficientCreditException(buyer.Credit, totalAmount);
        }

        var invoiceItems = buyer.Cart.Items
            .Select(i => new InvoiceItem(i.Product.Id, i.Product.Name, i.Product.Price, i.Quantity))
            .ToList();

        buyer.PayFromCredit(totalAmount);

        foreach (var item in buyer.Cart.Items)
        {
            item.Product.StockQuantity -= item.Quantity;
        }

        var invoice = new Invoice(_context.InvoiceIds.Next(), buyer.Username, invoiceItems);
        buyer.AddInvoice(invoice);
        buyer.Cart.Clear();

        _context.SaveChanges();
        return invoice;
    }
}
