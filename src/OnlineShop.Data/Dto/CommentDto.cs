using OnlineShop.Core.Enums;

namespace OnlineShop.Data.Dto;

public sealed class CommentDto
{
    public int Id { get; set; }

    public string BuyerUsername { get; set; } = string.Empty;

    public int ProductId { get; set; }

    public string Text { get; set; } = string.Empty;

    public CommentStatus Status { get; set; }

    public bool HasPurchasedProduct { get; set; }

    public DateTime CreatedAt { get; set; }
}
