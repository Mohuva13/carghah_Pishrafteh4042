using System.Globalization;
using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Products;
using OnlineShop.Core.Enums;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.Controllers.Cli;

/// <summary>
/// مسئول تفسیر دستورات خط فرمانی مدیر (مثل «Add Vehicle 4300 true BMW X5 66000 3»)
/// و اجرای عملیات متناظر با آن روی لیست کالاها/درخواست ها/کاربران.
/// در صورت نامعتبر بودن دستور، <see cref="InvalidCommandException"/> پرتاب می شود.
/// </summary>
public sealed class ManagerCliController
{
    private readonly ShopContext _context;
    private readonly ProductAdminController _productAdmin;
    private readonly RequestController _requests;

    public ManagerCliController(ShopContext context)
    {
        _context = context;
        _productAdmin = new ProductAdminController(context);
        _requests = new RequestController(context);
    }

    public const string HelpText =
        """
        دستورات قابل استفاده مدیر:
          Add FlashMemory   <UsbVersion> <CapacityGb> <Weight> <Dimensions> <Name> <Price> <Stock>
          Add Ssd           <ReadSpeedMbps> <WriteSpeedMbps> <CapacityGb> <Weight> <Dimensions> <Name> <Price> <Stock>
          Add PC            <CpuModel> <RamCapacityGb> <Weight> <Dimensions> <Name> <Price> <Stock>
          Add Pencil        <PencilType(2H|H|F|B|HB)> <CountryOfOrigin> <Name> <Price> <Stock>
          Add Pen           <Color> <CountryOfOrigin> <Name> <Price> <Stock>
          Add Notebook      <PageCount> <PaperType> <CountryOfOrigin> <Name> <Price> <Stock>
          Add Bicycle       <BicycleType(Mountain|Road|Urban|Hybrid)> <Manufacturer> <Name> <Price> <Stock>
          Add Car (Vehicle) <EngineVolume> <IsAutomatic(true|false)> <Manufacturer> <Name> <Price> <Stock>
          Add Food          <ProductionDate(yyyy-MM-dd)> <ExpirationDate(yyyy-MM-dd)> <Name> <Price> <Stock>
          Edit <ProductId> Name <NewName>
          Edit <ProductId> Price <NewPrice>
          Edit <ProductId> Stock <NewStock>
          Remove <ProductId>
          List [PageNumber]
          Requests
          Approve <RequestId>
          Reject <RequestId>
          Users
          Help
        نکته: مقادیری که شامل فاصله هستند را داخل گیومه ("...") بنویسید.
        """;

    public string Execute(string rawInput)
    {
        var tokens = CommandTokenizer.Tokenize(rawInput);
        if (tokens.Length == 0)
        {
            throw new InvalidCommandException("دستور نمی تواند خالی باشد. برای راهنما «Help» را وارد کنید.");
        }

        var command = tokens[0].ToLowerInvariant();
        var args = tokens.Skip(1).ToArray();

        return command switch
        {
            "help" => HelpText,
            "add" => ExecuteAdd(args),
            "edit" => ExecuteEdit(args),
            "remove" or "delete" => ExecuteRemove(args),
            "list" => ExecuteList(args),
            "requests" => ExecuteListRequests(),
            "approve" => ExecuteApprove(args),
            "reject" => ExecuteReject(args),
            "users" => ExecuteListUsers(),
            _ => throw new InvalidCommandException($"دستور «{tokens[0]}» شناخته شده نیست. برای راهنما «Help» را وارد کنید.")
        };
    }

    private string ExecuteAdd(string[] args)
    {
        if (args.Length == 0)
        {
            throw new InvalidCommandException("نوع کالا برای دستور Add مشخص نشده است.");
        }

        var typeKeyword = args[0].ToLowerInvariant();
        var rest = args.Skip(1).ToArray();
        var id = _context.ProductIds.Next();

        Product product = typeKeyword switch
        {
            "flashmemory" => BuildFlashMemory(id, rest),
            "ssd" => BuildSsd(id, rest),
            "pc" or "personalcomputer" => BuildPersonalComputer(id, rest),
            "pencil" => BuildPencil(id, rest),
            "pen" => BuildPen(id, rest),
            "notebook" => BuildNotebook(id, rest),
            "bicycle" => BuildBicycle(id, rest),
            "car" or "vehicle" => BuildCar(id, rest),
            "food" => BuildFood(id, rest),
            _ => throw new InvalidCommandException($"نوع کالای «{args[0]}» شناخته شده نیست.")
        };

        _productAdmin.AddProduct(product);
        return $"کالای جدید با موفقیت اضافه شد:{Environment.NewLine}{product}";
    }

    private static Product BuildFlashMemory(int id, string[] a)
    {
        RequireCount(a, 7, "Add FlashMemory <UsbVersion> <CapacityGb> <Weight> <Dimensions> <Name> <Price> <Stock>");
        return new FlashMemory(id, name: a[4], price: ParseDecimal(a[5], "قیمت"), stockQuantity: ParseStock(a, 6),
            weight: ParseDouble(a[2], "وزن"), dimensions: a[3], capacityGb: ParseInt(a[1], "ظرفیت"), usbVersion: a[0]);
    }

    private static Product BuildSsd(int id, string[] a)
    {
        RequireCount(a, 8, "Add Ssd <ReadSpeedMbps> <WriteSpeedMbps> <CapacityGb> <Weight> <Dimensions> <Name> <Price> <Stock>");
        return new Ssd(id, name: a[5], price: ParseDecimal(a[6], "قیمت"), stockQuantity: ParseStock(a, 7),
            weight: ParseDouble(a[3], "وزن"), dimensions: a[4], capacityGb: ParseInt(a[2], "ظرفیت"),
            readSpeedMbps: ParseInt(a[0], "سرعت خواندن"), writeSpeedMbps: ParseInt(a[1], "سرعت نوشتن"));
    }

    private static Product BuildPersonalComputer(int id, string[] a)
    {
        RequireCount(a, 7, "Add PC <CpuModel> <RamCapacityGb> <Weight> <Dimensions> <Name> <Price> <Stock>");
        return new PersonalComputer(id, name: a[4], price: ParseDecimal(a[5], "قیمت"), stockQuantity: ParseStock(a, 6),
            weight: ParseDouble(a[2], "وزن"), dimensions: a[3], cpuModel: a[0], ramCapacityGb: ParseInt(a[1], "ظرفیت رم"));
    }

    private static Product BuildPencil(int id, string[] a)
    {
        RequireCount(a, 5, "Add Pencil <PencilType> <CountryOfOrigin> <Name> <Price> <Stock>");
        return new Pencil(id, name: a[2], price: ParseDecimal(a[3], "قیمت"), stockQuantity: ParseStock(a, 4),
            countryOfOrigin: a[1], pencilType: ParsePencilType(a[0]));
    }

    private static Product BuildPen(int id, string[] a)
    {
        RequireCount(a, 5, "Add Pen <Color> <CountryOfOrigin> <Name> <Price> <Stock>");
        return new Pen(id, name: a[2], price: ParseDecimal(a[3], "قیمت"), stockQuantity: ParseStock(a, 4),
            countryOfOrigin: a[1], color: a[0]);
    }

    private static Product BuildNotebook(int id, string[] a)
    {
        RequireCount(a, 6, "Add Notebook <PageCount> <PaperType> <CountryOfOrigin> <Name> <Price> <Stock>");
        return new Notebook(id, name: a[3], price: ParseDecimal(a[4], "قیمت"), stockQuantity: ParseStock(a, 5),
            countryOfOrigin: a[2], pageCount: ParseInt(a[0], "تعداد برگ"), paperType: a[1]);
    }

    private static Product BuildBicycle(int id, string[] a)
    {
        RequireCount(a, 5, "Add Bicycle <BicycleType> <Manufacturer> <Name> <Price> <Stock>");
        return new Bicycle(id, name: a[2], price: ParseDecimal(a[3], "قیمت"), stockQuantity: ParseStock(a, 4),
            manufacturer: a[1], bicycleType: ParseBicycleType(a[0]));
    }

    private static Product BuildCar(int id, string[] a)
    {
        RequireCount(a, 6, "Add Car <EngineVolume> <IsAutomatic> <Manufacturer> <Name> <Price> <Stock>");
        return new Car(id, name: a[3], price: ParseDecimal(a[4], "قیمت"), stockQuantity: ParseStock(a, 5),
            manufacturer: a[2], engineVolume: ParseInt(a[0], "حجم موتور"), isAutomatic: ParseBool(a[1], "اتومات بودن"));
    }

    private static Product BuildFood(int id, string[] a)
    {
        RequireCount(a, 5, "Add Food <ProductionDate> <ExpirationDate> <Name> <Price> <Stock>");
        return new Food(id, name: a[2], price: ParseDecimal(a[3], "قیمت"), stockQuantity: ParseStock(a, 4),
            productionDate: ParseDate(a[0], "تاریخ تولید"), expirationDate: ParseDate(a[1], "تاریخ انقضا"));
    }

    private string ExecuteEdit(string[] a)
    {
        if (a.Length != 3)
        {
            throw new InvalidCommandException("فرمت صحیح: Edit <ProductId> <Name|Price|Stock> <NewValue>");
        }

        var id = ParseInt(a[0], "شناسه کالا");
        var field = a[1].ToLowerInvariant();

        switch (field)
        {
            case "name":
                _productAdmin.EditName(id, a[2]);
                break;
            case "price":
                _productAdmin.EditPrice(id, ParseDecimal(a[2], "قیمت"));
                break;
            case "stock":
                _productAdmin.EditStock(id, ParseInt(a[2], "موجودی"));
                break;
            default:
                throw new InvalidCommandException("فقط ویژگی های Name، Price و Stock قابل ویرایش هستند.");
        }

        return $"کالای #{id} با موفقیت ویرایش شد.";
    }

    private string ExecuteRemove(string[] a)
    {
        if (a.Length != 1)
        {
            throw new InvalidCommandException("فرمت صحیح: Remove <ProductId>");
        }

        var id = ParseInt(a[0], "شناسه کالا");
        _productAdmin.DeleteProduct(id);
        return $"کالای #{id} از لیست فروشگاه حذف شد.";
    }

    private string ExecuteList(string[] a)
    {
        var page = a.Length > 0 ? ParseInt(a[0], "شماره صفحه") : 1;
        var catalog = new Catalog.ProductCatalogController();
        var result = catalog.Search(null, page, Catalog.ProductCatalogController.DefaultPageSize);

        if (result.TotalCount == 0)
        {
            return "هیچ کالایی در فروشگاه ثبت نشده است.";
        }

        var lines = result.Items.Select(p => p.ToString());
        return string.Join(Environment.NewLine, lines) +
               $"{Environment.NewLine}-- صفحه {result.PageNumber} از {result.TotalPages} (مجموع {result.TotalCount} کالا) --";
    }

    private string ExecuteListRequests()
    {
        var pending = _requests.GetPendingRequests().ToList();
        if (pending.Count == 0)
        {
            return "هیچ درخواست در انتظاری وجود ندارد.";
        }

        return string.Join(Environment.NewLine, pending);
    }

    private string ExecuteApprove(string[] a)
    {
        if (a.Length != 1)
        {
            throw new InvalidCommandException("فرمت صحیح: Approve <RequestId>");
        }

        var id = ParseInt(a[0], "شناسه درخواست");
        _requests.Approve(id);
        return $"درخواست #{id} تایید شد.";
    }

    private string ExecuteReject(string[] a)
    {
        if (a.Length != 1)
        {
            throw new InvalidCommandException("فرمت صحیح: Reject <RequestId>");
        }

        var id = ParseInt(a[0], "شناسه درخواست");
        _requests.Reject(id);
        return $"درخواست #{id} رد شد.";
    }

    private string ExecuteListUsers()
    {
        var buyers = Manager.Instance.AllBuyers;
        if (buyers.Count == 0)
        {
            return "هیچ کاربری ثبت نام نکرده است.";
        }

        return string.Join(Environment.NewLine, buyers);
    }

    private static void RequireCount(string[] args, int expected, string usage)
    {
        if (args.Length != expected)
        {
            throw new InvalidCommandException($"تعداد آرگومان ها نادرست است. فرمت صحیح: {usage}");
        }
    }

    private static int ParseStock(string[] a, int index) => ParseInt(a[index], "موجودی");

    private static int ParseInt(string value, string fieldName)
    {
        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
        {
            throw new InvalidCommandException($"مقدار «{value}» برای «{fieldName}» باید عدد صحیح باشد.");
        }

        return result;
    }

    private static double ParseDouble(string value, string fieldName)
    {
        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
        {
            throw new InvalidCommandException($"مقدار «{value}» برای «{fieldName}» باید عدد باشد.");
        }

        return result;
    }

    private static decimal ParseDecimal(string value, string fieldName)
    {
        if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
        {
            throw new InvalidCommandException($"مقدار «{value}» برای «{fieldName}» باید عدد باشد.");
        }

        return result;
    }

    private static bool ParseBool(string value, string fieldName)
    {
        if (!bool.TryParse(value, out var result))
        {
            throw new InvalidCommandException($"مقدار «{value}» برای «{fieldName}» باید true یا false باشد.");
        }

        return result;
    }

    private static DateTime ParseDate(string value, string fieldName)
    {
        if (!DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
        {
            throw new InvalidCommandException($"مقدار «{value}» برای «{fieldName}» باید به فرمت yyyy-MM-dd باشد.");
        }

        return result;
    }

    private static PencilType ParsePencilType(string value) => value.ToUpperInvariant() switch
    {
        "2H" => Core.Enums.PencilType.TwoH,
        "H" => Core.Enums.PencilType.H,
        "F" => Core.Enums.PencilType.F,
        "B" => Core.Enums.PencilType.B,
        "HB" => Core.Enums.PencilType.HB,
        _ => throw new InvalidCommandException($"نوع مداد «{value}» نامعتبر است. مقادیر مجاز: 2H, H, F, B, HB")
    };

    private static BicycleType ParseBicycleType(string value)
    {
        if (Enum.TryParse<BicycleType>(value, true, out var result))
        {
            return result;
        }

        throw new InvalidCommandException($"نوع دوچرخه «{value}» نامعتبر است. مقادیر مجاز: Mountain, Road, Urban, Hybrid");
    }
}
