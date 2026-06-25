using OnlineShop.Core.Enums;

namespace OnlineShop.Core.Entities;

/// <summary>نظر یک خریدار درباره یک کالا. تا زمانی که مدیر تایید نکند، در لیست نظرات کالا نمایش داده نمی شود.</summary>
public sealed class Comment
{
    public Comment(int id, string buyerUsername, int productId, string text, bool hasPurchasedProduct)
    {
        Id = id;
        BuyerUsername = buyerUsername;
        ProductId = productId;
        Text = text;
        HasPurchasedProduct = hasPurchasedProduct;
        Status = CommentStatus.Pending;
        CreatedAt = DateTime.Now;
    }

    public int Id { get; }

    public string BuyerUsername { get; }

    public int ProductId { get; }

    public string Text { get; }

    /// <summary>وضعیت نظر: در انتظار تایید / تایید شده / تایید نشده توسط مدیر.</summary>
    public CommentStatus Status { get; private set; }

    /// <summary>سامانه به صورت خودکار مشخص می کند نظر دهنده کالا را خریده است یا خیر.</summary>
    public bool HasPurchasedProduct { get; }

    public DateTime CreatedAt { get; }

    public void Approve() => Status = CommentStatus.Approved;

    public void Reject() => Status = CommentStatus.Rejected;

    public override string ToString() =>
        $"#{Id} | {BuyerUsername} {(HasPurchasedProduct ? "(خریدار تایید شده)" : "(بدون خرید)")} | وضعیت: {StatusDisplay()} | {Text}";

    private string StatusDisplay() => Status switch
    {
        CommentStatus.Pending => "در انتظار تایید",
        CommentStatus.Approved => "تایید شده",
        CommentStatus.Rejected => "تایید نشده",
        _ => Status.ToString()
    };
}
