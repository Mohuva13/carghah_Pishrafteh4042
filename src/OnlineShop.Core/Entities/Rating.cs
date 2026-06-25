using OnlineShop.Core.Exceptions;

namespace OnlineShop.Core.Entities;

/// <summary>نمره ای که تنها خریدارِ واقعیِ یک کالا می تواند به آن بدهد (بازه ۱ تا ۵).</summary>
public sealed class Rating
{
    public Rating(string buyerUsername, int productId, int score)
    {
        if (score is < 1 or > 5)
        {
            throw new DomainException("امتیاز باید عددی بین ۱ تا ۵ باشد.");
        }

        BuyerUsername = buyerUsername;
        ProductId = productId;
        Score = score;
    }

    public string BuyerUsername { get; }

    public int ProductId { get; }

    public int Score { get; }

    public override string ToString() => $"{BuyerUsername} به کالای #{ProductId} امتیاز {Score} داد.";
}
