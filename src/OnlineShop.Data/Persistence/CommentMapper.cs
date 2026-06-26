using OnlineShop.Core.Entities;
using OnlineShop.Data.Dto;

namespace OnlineShop.Data.Persistence;

public static class CommentMapper
{
    public static CommentDto ToDto(Comment comment) => new()
    {
        Id = comment.Id,
        BuyerUsername = comment.BuyerUsername,
        ProductId = comment.ProductId,
        Text = comment.Text,
        Status = comment.Status,
        HasPurchasedProduct = comment.HasPurchasedProduct,
        CreatedAt = comment.CreatedAt
    };

    public static Comment FromDto(CommentDto dto) => new(
        dto.Id, dto.BuyerUsername, dto.ProductId, dto.Text, dto.HasPurchasedProduct, dto.Status, dto.CreatedAt);
}
