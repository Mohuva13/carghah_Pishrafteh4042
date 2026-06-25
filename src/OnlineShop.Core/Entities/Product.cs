using OnlineShop.Core.Enums;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.Core.Entities;

/// <summary>
/// کلاس پایه و abstract برای تمام کالاهای فروشگاه. امکان ساخت شیء مستقیم از این کلاس وجود ندارد؛
/// هر کالای واقعی باید از یکی از زیر دسته های تعریف شده (کالای دیجیتال، لوازم تحریر، وسیله نقلیه، خوراکی) مشتق شود.
/// </summary>
public abstract class Product
{
    private string _name = string.Empty;
    private decimal _price;
    private int _stockQuantity;
    private readonly List<Comment> _comments = new();
    private readonly List<Rating> _ratings = new();

    protected Product(int id, string name, decimal price, int stockQuantity, Category category)
    {
        Id = id;
        Name = name;
        Price = price;
        StockQuantity = stockQuantity;
        Category = category;
        IsDeleted = false;
    }

    public int Id { get; }

    /// <summary>قابل ویرایش توسط مدیر.</summary>
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new DomainException("نام کالا نمی تواند خالی باشد.");
            }

            _name = value;
        }
    }

    /// <summary>قابل ویرایش توسط مدیر.</summary>
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
            {
                throw new DomainException("قیمت کالا نمی تواند منفی باشد.");
            }

            _price = value;
        }
    }

    /// <summary>وضعیت موجودی (تعداد). قابل ویرایش توسط مدیر. صفر بودن به معنی ناموجود بودن است، نه حذف کالا.</summary>
    public int StockQuantity
    {
        get => _stockQuantity;
        set
        {
            if (value < 0)
            {
                throw new DomainException("موجودی کالا نمی تواند منفی باشد.");
            }

            _stockQuantity = value;
        }
    }

    public bool IsAvailable => StockQuantity > 0 && !IsDeleted;

    /// <summary>مدیر می تواند کالا را از لیست فروشگاه حذف کند؛ ولی رکورد آن (برای فاکتورهای قدیمی) باقی می ماند.</summary>
    public bool IsDeleted { get; set; }

    public Category Category { get; }

    public IReadOnlyList<Comment> Comments => _comments.AsReadOnly();

    public IReadOnlyList<Rating> Ratings => _ratings.AsReadOnly();

    /// <summary>میانگین نمره خریداران؛ در صورت نبود نمره، صفر بازگردانده می شود.</summary>
    public double AverageRating => _ratings.Count == 0 ? 0 : _ratings.Average(r => r.Score);

    public IEnumerable<Comment> ApprovedComments => _comments.Where(c => c.Status == CommentStatus.Approved);

    public void AddComment(Comment comment) => _comments.Add(comment);

    public void AddOrUpdateRating(Rating rating)
    {
        var existing = _ratings.FirstOrDefault(r => r.BuyerUsername == rating.BuyerUsername);
        if (existing is not null)
        {
            _ratings.Remove(existing);
        }

        _ratings.Add(rating);
    }

    /// <summary>هر زیر کلاس باید توضیح مختص ویژگی های خودش را برگرداند؛ برای استفاده در toString و نمایش جزئیات.</summary>
    public abstract string GetSpecificDetails();

    public override string ToString() =>
        $"#{Id} | {Name} | دسته: {CategoryDisplay()} | قیمت: {Price:N0} | موجودی: {StockQuantity} | میانگین نمره: {AverageRating:N1} | {GetSpecificDetails()}";

    private string CategoryDisplay() => Category switch
    {
        Category.DigitalGood => "کالای دیجیتال",
        Category.Stationery => "لوازم تحریر",
        Category.Vehicle => "وسایل نقلیه",
        Category.Food => "خوراکی",
        _ => Category.ToString()
    };
}
