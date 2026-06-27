using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Products;

namespace OnlineShop.Controllers.Catalog;

/// <summary>
/// مسئول مشاهده، جستجو، فیلتر و صفحه بندی کالاها. این عملیات نیازی به لاگین ندارد و
/// برای مهمان (کاربر لاگین نکرده) نیز در دسترس است.
/// </summary>
public sealed class ProductCatalogController
{
    public const int DefaultPageSize = 10;

    /// <summary>کالاهایی که مدیر حذفشان کرده در فروشگاه نمایش داده نمی شوند (ولی رکوردشان باقی می ماند).</summary>
    public IEnumerable<Product> GetStoreProducts() => Manager.Instance.Products.Where(p => !p.IsDeleted);

    public PagedResult<Product> Search(ProductFilter? filter, int pageNumber = 1, int pageSize = DefaultPageSize)
    {
        var query = GetStoreProducts();

        if (filter is not null)
        {
            query = ApplyFilter(query, filter);
        }

        var list = query.OrderBy(p => p.Id).ToList();
        var totalCount = list.Count;

        pageNumber = Math.Max(1, pageNumber);
        var pageItems = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResult<Product>(pageItems, pageNumber, pageSize, totalCount);
    }

    public Product? GetById(int productId) => Manager.Instance.FindProduct(productId) is { IsDeleted: false } p ? p : null;

    private static IEnumerable<Product> ApplyFilter(IEnumerable<Product> products, ProductFilter filter)
    {
        var query = products;

        if (!string.IsNullOrWhiteSpace(filter.NameContains))
        {
            query = query.Where(p => p.Name.Contains(filter.NameContains, StringComparison.OrdinalIgnoreCase));
        }

        if (filter.Category.HasValue)
        {
            query = query.Where(p => p.Category == filter.Category.Value);
        }

        if (filter.OnlyAvailable.HasValue)
        {
            query = query.Where(p => p.IsAvailable == filter.OnlyAvailable.Value);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);
        }

        if (filter.MinRating.HasValue)
        {
            query = query.Where(p => p.AverageRating >= filter.MinRating.Value);
        }

        if (filter.MaxRating.HasValue)
        {
            query = query.Where(p => p.AverageRating <= filter.MaxRating.Value);
        }

        if (filter.PencilType.HasValue)
        {
            query = query.Where(p => p is Pencil pencil && pencil.PencilType == filter.PencilType.Value);
        }

        if (filter.BicycleType.HasValue)
        {
            query = query.Where(p => p is Bicycle bicycle && bicycle.BicycleType == filter.BicycleType.Value);
        }

        if (filter.IsAutomatic.HasValue)
        {
            query = query.Where(p => p is Car car && car.IsAutomatic == filter.IsAutomatic.Value);
        }

        if (filter.MinCapacityGb.HasValue)
        {
            query = query.Where(p => p is StorageDevice storage && storage.CapacityGb >= filter.MinCapacityGb.Value);
        }

        if (filter.ExcludeExpiredFood == true)
        {
            query = query.Where(p => p is not Food food || !food.IsExpired);
        }

        if (!string.IsNullOrWhiteSpace(filter.ManufacturerContains))
        {
            query = query.Where(p => p is Vehicle vehicle &&
                                      vehicle.Manufacturer.Contains(filter.ManufacturerContains, StringComparison.OrdinalIgnoreCase));
        }

        return query;
    }
}
