using OnlineShop.Core.Exceptions;
using OnlineShop.Core.Utils;

namespace OnlineShop.Core.Entities;

/// <summary>
/// موجودیت حساب کاربری. این کلاس abstract است و امکان ساخت شیء مستقیم از آن وجود ندارد؛
/// تنها نقش های مشتق شده (<see cref="Buyer"/> و <see cref="Manager"/>) قابل نمونه سازی هستند.
/// </summary>
public abstract class Account
{
    private string _email = string.Empty;
    private string _phoneNumber = string.Empty;
    private string _passwordHash = string.Empty;

    protected Account(string username, string email, string phoneNumber, string password)
    {
        if (!RegexValidators.IsValidUsername(username))
        {
            throw new InvalidFormatException("نام کاربری", username);
        }

        Username = username;
        Email = email;
        PhoneNumber = phoneNumber;
        Password = password;
    }

    /// <summary>
    /// سازنده مخصوص حساب ثابت مدیر (admin/admin) که طبق صورت پروژه نیازی به ساخت با
    /// همان قواعد Regex عمومی رمز عبور ندارد؛ رمز عبور به صورت مستقیم هش می شود.
    /// </summary>
    protected Account(string username, string email, string phoneNumber, string password, bool isFixedSystemAccount)
    {
        Username = username;
        Email = email;
        PhoneNumber = phoneNumber;
        if (isFixedSystemAccount)
        {
            _passwordHash = PasswordHasher.Hash(password);
        }
        else
        {
            Password = password;
        }
    }

    /// <summary>
    /// سازنده مخصوص بازیابی حساب از فایل داده ذخیره شده (JSON). چون در این حالت
    /// مقادیر (ایمیل/تلفن) از قبل معتبرسنجی شده اند، دوباره بررسی regex لازم نیست
    /// و رمز عبور هش شده مستقیما (بدون هش مجدد) نشانده می شود.
    /// </summary>
    protected Account(string username, string email, string phoneNumber)
    {
        Username = username;
        _email = email;
        _phoneNumber = phoneNumber;
    }

    /// <summary>هش رمز عبور، برای ذخیره سازی در فایل داده. خود رمز عبور هرگز در دسترس نیست.</summary>
    public string PasswordHash => _passwordHash;

    /// <summary>فقط برای لایه بازیابی داده (Data) استفاده می شود تا هش قبلا محاسبه شده را بدون هش مجدد بنشاند.</summary>
    protected void RestorePasswordHash(string hash) => _passwordHash = hash;

    /// <summary>نام کاربری، پس از ساخت حساب دیگر قابل تغییر نیست.</summary>
    public string Username { get; }

    public string Email
    {
        get => _email;
        set
        {
            if (!RegexValidators.IsValidEmail(value))
            {
                throw new InvalidFormatException("ایمیل", value);
            }

            _email = value;
        }
    }

    public string PhoneNumber
    {
        get => _phoneNumber;
        set
        {
            if (!RegexValidators.IsValidPhoneNumber(value))
            {
                throw new InvalidFormatException("شماره تلفن", value);
            }

            _phoneNumber = value;
        }
    }

    /// <summary>رمز عبور هرگز به صورت متن ساده نگهداری نمی شود؛ فقط هش آن ذخیره می شود.</summary>
    public string Password
    {
        set
        {
            if (!RegexValidators.IsValidPassword(value))
            {
                throw new InvalidFormatException("رمز عبور", value);
            }

            _passwordHash = PasswordHasher.Hash(value);
        }
    }

    public bool VerifyPassword(string plainTextPassword) => PasswordHasher.Verify(plainTextPassword, _passwordHash);

    /// <summary>نقش کاربر برای تصمیم گیری در ورود به ناحیه کاربری مناسب.</summary>
    public abstract string RoleName { get; }

    public override string ToString() =>
        $"نام کاربری: {Username} | ایمیل: {Email} | تلفن: {PhoneNumber} | نقش: {RoleName}";
}
