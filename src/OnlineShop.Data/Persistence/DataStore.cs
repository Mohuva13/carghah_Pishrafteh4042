using System.Text.Json;
using System.Text.Json.Serialization;
using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Requests;
using OnlineShop.Core.Utils;
using OnlineShop.Data.Dto;

namespace OnlineShop.Data.Persistence;

/// <summary>
/// نقطه مرکزی ارتباط با فایل JSON برای ذخیره و بازیابی کل وضعیت فروشگاه (Manager Singleton).
/// همچنین شمارنده های شناسه یکتا (Id) برای کالا، فاکتور، نظر و درخواست را نگه می دارد
/// تا بعد از بارگذاری مجدد داده، شناسه تکراری تولید نشود.
/// </summary>
public sealed class DataStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public string FilePath { get; }

    public IdGenerator ProductIds { get; } = new();

    public IdGenerator InvoiceIds { get; } = new();

    public IdGenerator CommentIds { get; } = new();

    public IdGenerator RequestIds { get; } = new();

    public DataStore(string filePath)
    {
        FilePath = filePath;
    }

    public bool DataFileExists() => File.Exists(FilePath);

    public void Save()
    {
        var manager = Manager.Instance;
        var root = new RootDto
        {
            NextProductId = ProductIds.Current,
            NextInvoiceId = InvoiceIds.Current,
            NextCommentId = CommentIds.Current,
            NextRequestId = RequestIds.Current,
            Products = manager.Products.Select(ProductMapper.ToDto).ToList(),
            Buyers = manager.AllBuyers.Select(ToBuyerDto).ToList(),
            Requests = manager.Requests.Select(ToRequestDto).ToList()
        };

        var directory = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(FilePath, JsonSerializer.Serialize(root, JsonOptions));
    }

    public void Load()
    {
        if (!DataFileExists())
        {
            return;
        }

        var json = File.ReadAllText(FilePath);
        var root = JsonSerializer.Deserialize<RootDto>(json, JsonOptions);
        if (root is null)
        {
            return;
        }

        var manager = Manager.Instance;

        var products = root.Products.Select(ProductMapper.FromDto).ToList();
        foreach (var product in products)
        {
            manager.AddProduct(product);
        }

        var buyers = root.Buyers.Select(dto => ToBuyerEntity(dto, products)).ToList();
        foreach (var buyer in buyers)
        {
            manager.RegisterBuyer(buyer);
        }

        foreach (var requestDto in root.Requests)
        {
            manager.AddRequest(ToRequestEntity(requestDto, buyers, products));
        }

        ProductIds.EnsureAtLeast(root.NextProductId);
        InvoiceIds.EnsureAtLeast(root.NextInvoiceId);
        CommentIds.EnsureAtLeast(root.NextCommentId);
        RequestIds.EnsureAtLeast(root.NextRequestId);
    }

    private static BuyerDto ToBuyerDto(Buyer buyer) => new()
    {
        Username = buyer.Username,
        Email = buyer.Email,
        PhoneNumber = buyer.PhoneNumber,
        PasswordHash = buyer.PasswordHash,
        IsApproved = buyer.IsApproved,
        Credit = buyer.Credit,
        Invoices = buyer.Invoices.Select(inv => new InvoiceDto
        {
            Id = inv.Id,
            BuyerUsername = inv.BuyerUsername,
            Date = inv.Date,
            Items = inv.Items.Select(i => new InvoiceItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        }).ToList(),
        CartItems = buyer.Cart.Items.Select(ci => new CartItemDto { ProductId = ci.Product.Id, Quantity = ci.Quantity }).ToList()
    };

    private static Buyer ToBuyerEntity(BuyerDto dto, List<Product> allProducts)
    {
        var buyer = new Buyer(dto.Username, dto.Email, dto.PhoneNumber, dto.PasswordHash, dto.IsApproved, dto.Credit);

        foreach (var invoiceDto in dto.Invoices)
        {
            var items = invoiceDto.Items.Select(i => new InvoiceItem(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity));
            buyer.AddInvoice(new Invoice(invoiceDto.Id, invoiceDto.BuyerUsername, invoiceDto.Date, items));
        }

        foreach (var cartItemDto in dto.CartItems)
        {
            var product = allProducts.FirstOrDefault(p => p.Id == cartItemDto.ProductId);
            if (product is not null)
            {
                buyer.Cart.AddProduct(product, cartItemDto.Quantity);
            }
        }

        return buyer;
    }

    private static RequestDto ToRequestDto(RequestBase request)
    {
        var dto = new RequestDto
        {
            Id = request.Id,
            Type = request.Type,
            RequesterUsername = request.RequesterUsername,
            Status = request.Status,
            CreatedAt = request.CreatedAt
        };

        switch (request)
        {
            case CommentApprovalRequest commentRequest:
                dto.CommentId = commentRequest.Comment.Id;
                dto.CommentProductId = commentRequest.Comment.ProductId;
                break;
            case CreditChargeRequest creditRequest:
                dto.Amount = creditRequest.Amount;
                dto.MaskedCardNumber = creditRequest.MaskedCardNumber;
                break;
        }

        return dto;
    }

    private static RequestBase ToRequestEntity(RequestDto dto, List<Buyer> allBuyers, List<Product> allProducts)
    {
        return dto.Type switch
        {
            Core.Enums.RequestType.Registration => CreateRegistrationRequest(dto, allBuyers),
            Core.Enums.RequestType.CommentApproval => CreateCommentApprovalRequest(dto, allProducts),
            Core.Enums.RequestType.CreditCharge => CreateCreditChargeRequest(dto, allBuyers),
            _ => throw new NotSupportedException($"نوع درخواست «{dto.Type}» پشتیبانی نمی شود.")
        };
    }

    private static RequestBase CreateRegistrationRequest(RequestDto dto, List<Buyer> allBuyers)
    {
        var buyer = allBuyers.First(b => b.Username == dto.RequesterUsername);
        return new RegistrationRequest(dto.Id, buyer, dto.Status, dto.CreatedAt);
    }

    private static RequestBase CreateCommentApprovalRequest(RequestDto dto, List<Product> allProducts)
    {
        var product = allProducts.First(p => p.Id == dto.CommentProductId);
        var comment = product.Comments.First(c => c.Id == dto.CommentId);
        return new CommentApprovalRequest(dto.Id, comment, dto.Status, dto.CreatedAt);
    }

    private static RequestBase CreateCreditChargeRequest(RequestDto dto, List<Buyer> allBuyers)
    {
        var buyer = allBuyers.First(b => b.Username == dto.RequesterUsername);
        return new CreditChargeRequest(dto.Id, buyer, dto.Amount ?? 0, dto.MaskedCardNumber ?? string.Empty, dto.Status, dto.CreatedAt);
    }
}
