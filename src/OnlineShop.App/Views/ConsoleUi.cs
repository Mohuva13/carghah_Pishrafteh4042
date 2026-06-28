using System.Globalization;

namespace OnlineShop.App.Views;

/// <summary>
/// مجموعه ای از متدهای کمکی برای گرفتن ورودی و نمایش خروجی در کنسول،
/// تا کد View ها تمیز و بدون تکرار باقی بماند.
/// </summary>
public static class ConsoleUi
{
    public static void PrintHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 60));
        Console.WriteLine($"  {title}");
        Console.WriteLine(new string('=', 60));
    }

    public static void PrintError(string message)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"خطا: {message}");
        Console.ForegroundColor = previous;
    }

    public static void PrintSuccess(string message)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ForegroundColor = previous;
    }

    public static void PrintInfo(string message)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(message);
        Console.ForegroundColor = previous;
    }

    public static string ReadNonEmpty(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var value = Console.ReadLine()?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            PrintError("این مقدار نمی تواند خالی باشد.");
        }
    }

    /// <summary>رشته ای که می تواند خالی باشد (برای فیلترهای اختیاری).</summary>
    public static string? ReadOptional(string prompt)
    {
        Console.Write($"{prompt} (برای رد شدن Enter بزنید): ");
        var value = Console.ReadLine()?.Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var raw = Console.ReadLine();
            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            PrintError("لطفا یک عدد صحیح معتبر وارد کنید.");
        }
    }

    public static int? ReadOptionalInt(string prompt)
    {
        Console.Write($"{prompt} (برای رد شدن Enter بزنید): ");
        var raw = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : null;
    }

    public static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var raw = Console.ReadLine();
            if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var value) && value >= 0)
            {
                return value;
            }

            PrintError("لطفا یک عدد معتبر و غیر منفی وارد کنید.");
        }
    }

    public static decimal? ReadOptionalDecimal(string prompt)
    {
        Console.Write($"{prompt} (برای رد شدن Enter بزنید): ");
        var raw = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        return decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var value) ? value : null;
    }

    public static bool ReadYesNo(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (y/n): ");
            var raw = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (raw is "y" or "yes")
            {
                return true;
            }

            if (raw is "n" or "no")
            {
                return false;
            }

            PrintError("لطفا فقط y یا n وارد کنید.");
        }
    }

    public static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("برای ادامه، کلیدی را فشار دهید...");
        Console.ReadKey(true);
    }
}
