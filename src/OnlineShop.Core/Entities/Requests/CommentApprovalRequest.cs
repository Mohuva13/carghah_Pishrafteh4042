using OnlineShop.Core.Enums;

namespace OnlineShop.Core.Entities.Requests;

/// <summary>درخواست تایید یک نظر ثبت شده روی یک کالا.</summary>
public sealed class CommentApprovalRequest : RequestBase
{
    public CommentApprovalRequest(int id, Comment comment)
        : base(id, RequestType.CommentApproval, comment.BuyerUsername)
    {
        Comment = comment;
    }

    /// <summary>سازنده مخصوص بازیابی از فایل داده.</summary>
    public CommentApprovalRequest(int id, Comment comment, RequestStatus status, DateTime createdAt)
        : base(id, RequestType.CommentApproval, comment.BuyerUsername, status, createdAt)
    {
        Comment = comment;
    }

    public Comment Comment { get; }

    protected override void OnApprove() => Comment.Approve();

    protected override void OnReject() => Comment.Reject();

    public override string GetSummary() =>
        $"نظر روی کالای #{Comment.ProductId}: \"{Comment.Text}\"";
}
