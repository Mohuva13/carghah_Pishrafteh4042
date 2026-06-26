namespace OnlineShop.Data.Dto;

public sealed class InvoiceItemDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }
}

public sealed class InvoiceDto
{
    public int Id { get; set; }

    public string BuyerUsername { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public List<InvoiceItemDto> Items { get; set; } = new();
}
