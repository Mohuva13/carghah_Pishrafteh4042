using OnlineShop.Controllers;
using OnlineShop.Controllers.Catalog;
using OnlineShop.Core.Entities;
using OnlineShop.Core.Enums;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.App.Views;

/// <summary>
/// صفحه محصولات: نمایش صفحه بندی شده کالاها، جستجو، فیلتر و رفتن به صفحه جزئیات یک کالا.
/// این صفحه بدون ورود به حساب هم قابل دسترسی است (مهمان)، به همراه سبد خرید موقت مهمان.
/// </summary>
public static class ProductBrowseView
{
    public static void Show(ShopContext context, ShoppingCart guestCart)
    {
        var catalog = new ProductCatalogController();
        ProductFilter? filter = null;
        var page = 1;

        while (true)
        {
            var result = catalog.Search(filter, page, ProductCatalogController.DefaultPageSize);

            ConsoleUi.PrintHeader("صفحه محصولات فروشگاه");
            if (filter is not null)
            {
                ConsoleUi.PrintInfo("(فیلتر فعال است)");
            }

            if (result.TotalCount == 0)
            {
                Console.WriteLine("هیچ کالایی با این شرایط پیدا نشد.");
            }
            else
            {
                foreach (var product in result.Items)
                {
                    Console.WriteLine(product);
                }

                Console.WriteLine();
                Console.WriteLine($"صفحه {result.PageNumber} از {Math.Max(result.TotalPages, 1)} | مجموع {result.TotalCount} کالا | سبد خرید فعلی: {(context.CurrentBuyer?.Cart ?? guestCart).Items.Count} قلم");
            }

            Console.WriteLine();
            Console.WriteLine("1. صفحه بعد");
            Console.WriteLine("2. صفحه قبل");
            Console.WriteLine("3. جستجو بر اساس نام");
            Console.WriteLine("4. اعمال فیلتر");
            Console.WriteLine("5. حذف فیلترها");
            Console.WriteLine("6. مشاهده جزئیات یک کالا (خرید/نظر/نمره)");
            Console.WriteLine("7. مشاهده سبد خرید و نهایی کردن خرید");
            Console.WriteLine("0. بازگشت");
            Console.Write("انتخاب شما: ");

            switch (ConsoleUi.ReadMenuChoice())
            {
                case "1":
                    if (page < Math.Max(result.TotalPages, 1))
                    {
                        page++;
                    }

                    break;
                case "2":
                    if (page > 1)
                    {
                        page--;
                    }

                    break;
                case "3":
                    filter ??= new ProductFilter();
                    filter.NameContains = ConsoleUi.ReadOptional("عبارت جستجو در نام کالا");
                    page = 1;
                    break;
                case "4":
                    filter = BuildFilter();
                    page = 1;
                    break;
                case "5":
                    filter = null;
                    page = 1;
                    ConsoleUi.PrintInfo("فیلترها حذف شدند.");
                    break;
                case "6":
                    ShowProductDetail(context, guestCart, catalog);
                    break;
                case "7":
                    CartView.Show(context, guestCart);
                    break;
                case "0":
                    return;
                default:
                    ConsoleUi.PrintError("گزینه نامعتبر است.");
                    break;
            }
        }
    }

    private static ProductFilter BuildFilter()
    {
        var filter = new ProductFilter();

        ConsoleUi.PrintHeader("اعمال فیلتر روی کالاها");
        Console.WriteLine("دسته کالا: 1) کالای دیجیتال 2) لوازم تحریر 3) وسایل نقلیه 4) خوراکی 0) بدون فیلتر دسته");
        var categoryChoice = ConsoleUi.ReadOptionalInt("شماره دسته");
        filter.Category = categoryChoice switch
        {
            1 => Category.DigitalGood,
            2 => Category.Stationery,
            3 => Category.Vehicle,
            4 => Category.Food,
            _ => null
        };

        var availabilityChoice = ConsoleUi.ReadOptional("فقط کالاهای موجود؟ (y/n)");
        filter.OnlyAvailable = availabilityChoice switch
        {
            "y" or "Y" => true,
            "n" or "N" => false,
            _ => null
        };

        filter.MinPrice = ConsoleUi.ReadOptionalDecimal("حداقل قیمت");
        filter.MaxPrice = ConsoleUi.ReadOptionalDecimal("حداکثر قیمت");
        filter.MinRating = ConsoleUi.ReadOptionalDecimal("حداقل میانگین نمره (۰ تا ۵)") is { } minR ? (double)minR : null;
        filter.MaxRating = ConsoleUi.ReadOptionalDecimal("حداکثر میانگین نمره (۰ تا ۵)") is { } maxR ? (double)maxR : null;

        if (filter.Category == Category.Stationery)
        {
            var pencilChoice = ConsoleUi.ReadOptional("فیلتر نوع مداد (2H/H/F/B/HB)");
            if (pencilChoice is not null && TryParsePencilType(pencilChoice, out var pencilType))
            {
                filter.PencilType = pencilType;
            }
        }

        if (filter.Category == Category.Vehicle)
        {
            var bicycleChoice = ConsoleUi.ReadOptional("فیلتر نوع دوچرخه (Mountain/Road/Urban/Hybrid)");
            if (bicycleChoice is not null && Enum.TryParse<BicycleType>(bicycleChoice, true, out var bicycleType))
            {
                filter.BicycleType = bicycleType;
            }

            var automaticChoice = ConsoleUi.ReadOptional("فقط خودروهای اتومات؟ (y/n)");
            filter.IsAutomatic = automaticChoice switch { "y" or "Y" => true, "n" or "N" => false, _ => null };

            filter.ManufacturerContains = ConsoleUi.ReadOptional("فیلتر شرکت سازنده");
        }

        if (filter.Category == Category.DigitalGood)
        {
            var capacity = ConsoleUi.ReadOptionalInt("حداقل ظرفیت حافظه (GB) - فقط تجهیزات ذخیره سازی");
            filter.MinCapacityGb = capacity;
        }

        if (filter.Category == Category.Food)
        {
            filter.ExcludeExpiredFood = ConsoleUi.ReadYesNo("خوراکی های منقضی شده مخفی شوند؟");
        }

        return filter;
    }

    private static bool TryParsePencilType(string value, out PencilType pencilType)
    {
        switch (value.ToUpperInvariant())
        {
            case "2H": pencilType = PencilType.TwoH; return true;
            case "H": pencilType = PencilType.H; return true;
            case "F": pencilType = PencilType.F; return true;
            case "B": pencilType = PencilType.B; return true;
            case "HB": pencilType = PencilType.HB; return true;
            default: pencilType = default; return false;
        }
    }

    private static void ShowProductDetail(ShopContext context, ShoppingCart guestCart, ProductCatalogController catalog)
    {
        var id = ConsoleUi.ReadInt("شناسه کالا (Id)");
        var product = catalog.GetById(id);
        if (product is null)
        {
            ConsoleUi.PrintError("کالایی با این شناسه پیدا نشد.");
            ConsoleUi.Pause();
            return;
        }

        ProductDetailView.Show(context, guestCart, product);
    }
}
