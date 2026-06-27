using OnlineShop.Core.Entities;

namespace OnlineShop.Controllers;

/// <summary>مسئول ویرایش اطلاعات شخصی حساب کاربری (به جز نام کاربری) و شارژ اعتبار.</summary>
public sealed class AccountController
{
    private readonly ShopContext _context;

    public AccountController(ShopContext context)
    {
        _context = context;
    }

    public void UpdateEmail(Buyer buyer, string newEmail)
    {
        buyer.Email = newEmail;
        _context.SaveChanges();
    }

    public void UpdatePhoneNumber(Buyer buyer, string newPhoneNumber)
    {
        buyer.PhoneNumber = newPhoneNumber;
        _context.SaveChanges();
    }

    public void UpdatePassword(Buyer buyer, string newPassword)
    {
        buyer.Password = newPassword;
        _context.SaveChanges();
    }
}
