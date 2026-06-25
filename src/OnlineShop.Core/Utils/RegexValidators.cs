using System.Text.RegularExpressions;

namespace OnlineShop.Core.Utils;

/// <summary>
/// تمام قوانین اعتبارسنجی فرمتی پروژه (ایمیل، شماره تلفن، رمز عبور، اطلاعات کارت بانکی)
/// با استفاده از عبارت باقاعده (Regex) در یک کلاس مرکزی نگه داری می شوند.
/// </summary>
public static class RegexValidators
{
    // example: name@example.com
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
        RegexOptions.Compiled);

    // Iranian mobile number, e.g. 09123456789
    private static readonly Regex PhoneRegex = new(
        @"^09\d{9}$",
        RegexOptions.Compiled);

    // at least 8 characters, at least one letter and one digit
    private static readonly Regex PasswordRegex = new(
        @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@#$%^&+=!_.-]{8,32}$",
        RegexOptions.Compiled);

    // username: letters, digits, underscore, 3-20 chars, must start with a letter
    private static readonly Regex UsernameRegex = new(
        @"^[A-Za-z][A-Za-z0-9_]{2,19}$",
        RegexOptions.Compiled);

    // 16 digit card number, optionally separated by spaces or dashes in groups of 4
    private static readonly Regex CardNumberRegex = new(
        @"^\d{4}[- ]?\d{4}[- ]?\d{4}[- ]?\d{4}$",
        RegexOptions.Compiled);

    // 4 to 6 digit dynamic/second password for the card
    private static readonly Regex CardPasswordRegex = new(
        @"^\d{4,6}$",
        RegexOptions.Compiled);

    // 3 or 4 digit CVV2
    private static readonly Regex Cvv2Regex = new(
        @"^\d{3,4}$",
        RegexOptions.Compiled);

    public static bool IsValidEmail(string? value) => value is not null && EmailRegex.IsMatch(value);

    public static bool IsValidPhoneNumber(string? value) => value is not null && PhoneRegex.IsMatch(value);

    public static bool IsValidPassword(string? value) => value is not null && PasswordRegex.IsMatch(value);

    public static bool IsValidUsername(string? value) => value is not null && UsernameRegex.IsMatch(value);

    public static bool IsValidCardNumber(string? value) => value is not null && CardNumberRegex.IsMatch(value);

    public static bool IsValidCardPassword(string? value) => value is not null && CardPasswordRegex.IsMatch(value);

    public static bool IsValidCvv2(string? value) => value is not null && Cvv2Regex.IsMatch(value);
}
