using OnlineShop.Core.Entities.Requests;

namespace OnlineShop.Core.Entities;

/// <summary>
/// موجودیت مدیر (فروشنده و ادمین سایت). طبق صورت پروژه از الگوی طراحی Singleton پیروی می کند؛
/// در کل برنامه تنها یک نمونه از مدیر وجود دارد و نام کاربری/رمز عبور آن ثابت (admin/admin) است.
/// </summary>
public sealed class Manager : Account
{
    private static Manager? _instance;
    private static readonly object SyncRoot = new();

    private readonly List<Product> _products = new();
    private readonly List<RequestBase> _requests = new();
    private readonly List<Buyer> _allBuyers = new();

    public const string FixedUsername = "admin";
    public const string FixedPassword = "admin";

    private Manager()
        : base(FixedUsername, "admin@onlineshop.local", "0912" + "0000000", FixedPassword, isFixedSystemAccount: true)
    {
    }

    /// <summary>تنها راه دسترسی به نمونه یکتای مدیر در کل برنامه.</summary>
    public static Manager Instance
    {
        get
        {
            lock (SyncRoot)
            {
                return _instance ??= new Manager();
            }
        }
    }

    public override string RoleName => "مدیر";

    public IReadOnlyList<Product> Products => _products.AsReadOnly();

    public IReadOnlyList<RequestBase> Requests => _requests.AsReadOnly();

    public IReadOnlyList<Buyer> AllBuyers => _allBuyers.AsReadOnly();

    public void AddProduct(Product product) => _products.Add(product);

    public bool RemoveProduct(int productId) => _products.RemoveAll(p => p.Id == productId) > 0;

    public Product? FindProduct(int productId) => _products.FirstOrDefault(p => p.Id == productId);

    public void AddRequest(RequestBase request) => _requests.Add(request);

    public IEnumerable<RequestBase> PendingRequests =>
        _requests.Where(r => r.Status == Enums.RequestStatus.Pending);

    public void RegisterBuyer(Buyer buyer) => _allBuyers.Add(buyer);

    public void RemoveBuyer(string username) => _allBuyers.RemoveAll(b => b.Username == username);

    public Buyer? FindBuyer(string username) =>
        _allBuyers.FirstOrDefault(b => b.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

    /// <summary>فقط برای تست ها: بازنشانی وضعیت Singleton تا هر تست از نمونه تازه استفاده کند.</summary>
    public static void ResetForTests() => _instance = null;

    public override string ToString() =>
        $"{base.ToString()} | تعداد کالاها: {_products.Count} | درخواست های در انتظار: {PendingRequests.Count()} | تعداد کاربران: {_allBuyers.Count}";
}
