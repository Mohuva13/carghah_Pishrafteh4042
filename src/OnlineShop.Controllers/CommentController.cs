using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Requests;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.Controllers;

/// <summary>
/// مسئول ثبت نظر برای یک کالا. سامانه به صورت خودکار مشخص می کند نظر دهنده کیست و آیا
/// کالا را خریده است یا خیر؛ سپس درخواست تایید نظر برای مدیر ارسال می شود.
/// </summary>
public sealed class CommentController
{
    private readonly ShopContext _context;

    public CommentController(ShopContext context)
    {
        _context = context;
    }

    public Comment AddComment(Buyer buyer, int productId, string text)
    {
        var product = Manager.Instance.FindProduct(productId) ?? throw new EntityNotFoundException("کالا", productId);

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new DomainException("متن نظر نمی تواند خالی باشد.");
        }

        var hasPurchased = buyer.HasPurchased(productId);
        var comment = new Comment(_context.CommentIds.Next(), buyer.Username, productId, text, hasPurchased);
        product.AddComment(comment);

        var request = new CommentApprovalRequest(_context.RequestIds.Next(), comment);
        Manager.Instance.AddRequest(request);

        _context.SaveChanges();
        return comment;
    }
}
