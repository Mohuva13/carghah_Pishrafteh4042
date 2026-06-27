using OnlineShop.Core.Entities;
using OnlineShop.Core.Utils;
using OnlineShop.Data.Persistence;

namespace OnlineShop.Controllers;

/// <summary>
/// نگه دارنده وضعیت مشترک بین تمام کنترلرها در طول اجرای برنامه: کاربر لاگین شده فعلی
/// و دسترسی به لایه ذخیره سازی (DataStore). این کلاس نقش «Session» ساده را در معماری MVC بازی می کند.
/// </summary>
public sealed class ShopContext
{
    public DataStore Store { get; }

    public Account? CurrentUser { get; private set; }

    public Buyer? CurrentBuyer => CurrentUser as Buyer;

    public Manager? CurrentManager => CurrentUser as Manager;

    public bool IsLoggedIn => CurrentUser is not null;

    public ShopContext(DataStore store)
    {
        Store = store;
    }

    public void SetCurrentUser(Account account) => CurrentUser = account;

    public void Logout() => CurrentUser = null;

    public IdGenerator ProductIds => Store.ProductIds;

    public IdGenerator InvoiceIds => Store.InvoiceIds;

    public IdGenerator CommentIds => Store.CommentIds;

    public IdGenerator RequestIds => Store.RequestIds;

    public void SaveChanges() => Store.Save();
}
