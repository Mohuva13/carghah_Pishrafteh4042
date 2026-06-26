namespace OnlineShop.Data.Dto;

/// <summary>ریشه سند JSON که کل وضعیت فروشگاه (کالاها، کاربران، درخواست ها) را در بر می گیرد.</summary>
public sealed class RootDto
{
    public int NextProductId { get; set; }

    public int NextInvoiceId { get; set; }

    public int NextCommentId { get; set; }

    public int NextRequestId { get; set; }

    public List<ProductDto> Products { get; set; } = new();

    public List<BuyerDto> Buyers { get; set; } = new();

    public List<RequestDto> Requests { get; set; } = new();
}
