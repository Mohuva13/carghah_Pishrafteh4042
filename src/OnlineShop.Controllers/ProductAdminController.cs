using OnlineShop.Core.Entities;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.Controllers;

/// <summary>مسئول افزودن، ویرایش و حذف کالا توسط مدیر.</summary>
public sealed class ProductAdminController
{
    private readonly ShopContext _context;

    public ProductAdminController(ShopContext context)
    {
        _context = context;
    }

    public void AddProduct(Product product)
    {
        Manager.Instance.AddProduct(product);
        _context.SaveChanges();
    }

    /// <summary>طبق صورت پروژه، فقط اسم، قیمت و وضعیت موجودی قابل ویرایش هستند.</summary>
    public void EditName(int productId, string newName)
    {
        var product = GetOrThrow(productId);
        product.Name = newName;
        _context.SaveChanges();
    }

    public void EditPrice(int productId, decimal newPrice)
    {
        var product = GetOrThrow(productId);
        product.Price = newPrice;
        _context.SaveChanges();
    }

    public void EditStock(int productId, int newStock)
    {
        var product = GetOrThrow(productId);
        product.StockQuantity = newStock;
        _context.SaveChanges();
    }

    /// <summary>حذف کامل کالا از لیست فروشگاه. موجودی صفر به تنهایی باعث حذف کالا نمی شود.</summary>
    public void DeleteProduct(int productId)
    {
        var product = GetOrThrow(productId);
        product.IsDeleted = true;
        _context.SaveChanges();
    }

    private static Product GetOrThrow(int productId) =>
        Manager.Instance.FindProduct(productId) ?? throw new EntityNotFoundException("کالا", productId);
}
