using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Requests;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.Controllers;

/// <summary>مسئول فرآیندهای ثبت نام و ورود به سامانه.</summary>
public sealed class AuthController
{
    private readonly ShopContext _context;

    public AuthController(ShopContext context)
    {
        _context = context;
    }

    /// <summary>
    /// ثبت نام یک خریدار جدید. طبق صورت پروژه ثبت نام فقط برای خریدار ممکن است و
    /// پس از آن یک درخواست تایید برای مدیر ارسال می شود؛ خریدار تا زمان تایید نمی تواند وارد شود.
    /// </summary>
    public RegistrationRequest Register(string username, string email, string phoneNumber, string password)
    {
        var manager = Manager.Instance;

        if (username.Equals(Manager.FixedUsername, StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainException("این نام کاربری قابل استفاده نیست.");
        }

        if (manager.FindBuyer(username) is not null)
        {
            throw new DomainException($"نام کاربری «{username}» قبلا استفاده شده است.");
        }

        var buyer = new Buyer(username, email, phoneNumber, password);
        manager.RegisterBuyer(buyer);

        var request = new RegistrationRequest(_context.RequestIds.Next(), buyer);
        manager.AddRequest(request);
        _context.SaveChanges();
        return request;
    }

    /// <summary>
    /// ورود بر اساس نام کاربری و رمز عبور. اگر نام کاربری/رمز برابر با admin/admin باشد،
    /// کاربر به عنوان مدیر وارد می شود؛ در غیر این صورت به دنبال خریدار تایید شده می گردد.
    /// </summary>
    public Account Login(string username, string password)
    {
        if (username.Equals(Manager.FixedUsername, StringComparison.OrdinalIgnoreCase))
        {
            if (password == Manager.FixedPassword)
            {
                _context.SetCurrentUser(Manager.Instance);
                return Manager.Instance;
            }

            throw new AuthenticationException("رمز عبور مدیر اشتباه است.");
        }

        var buyer = Manager.Instance.FindBuyer(username);
        if (buyer is null || !buyer.VerifyPassword(password))
        {
            throw new AuthenticationException("نام کاربری یا رمز عبور اشتباه است.");
        }

        if (!buyer.IsApproved)
        {
            throw new AuthenticationException("ثبت نام شما هنوز توسط مدیر تایید نشده است.");
        }

        _context.SetCurrentUser(buyer);
        return buyer;
    }

    public void Logout() => _context.Logout();
}
