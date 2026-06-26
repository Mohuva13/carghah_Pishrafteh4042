namespace OnlineShop.Data.Dto;

public sealed class RatingDto
{
    public string BuyerUsername { get; set; } = string.Empty;

    public int ProductId { get; set; }

    public int Score { get; set; }
}
