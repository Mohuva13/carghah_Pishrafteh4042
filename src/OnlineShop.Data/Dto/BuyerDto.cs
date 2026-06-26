namespace OnlineShop.Data.Dto;

public sealed class CartItemDto
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }
}

public sealed class BuyerDto
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public bool IsApproved { get; set; }

    public decimal Credit { get; set; }

    public List<InvoiceDto> Invoices { get; set; } = new();

    public List<CartItemDto> CartItems { get; set; } = new();
}
