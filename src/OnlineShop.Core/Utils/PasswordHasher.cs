using System.Security.Cryptography;
using System.Text;

namespace OnlineShop.Core.Utils;

/// <summary>
/// به جای نگهداری رمز عبور به صورت متن ساده، هش SHA-256 آن ذخیره می شود.
/// این کلاس امکان ساخت شیء از آن مجاز نیست چون تنها شامل متدهای کمکی ایستا است.
/// </summary>
public static class PasswordHasher
{
    public static string Hash(string plainText)
    {
        var bytes = Encoding.UTF8.GetBytes(plainText);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes);
    }

    public static bool Verify(string plainText, string storedHash) =>
        Hash(plainText).Equals(storedHash, StringComparison.OrdinalIgnoreCase);
}
