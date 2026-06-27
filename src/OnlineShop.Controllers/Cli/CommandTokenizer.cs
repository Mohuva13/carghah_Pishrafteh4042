using System.Text;

namespace OnlineShop.Controllers.Cli;

/// <summary>
/// یک تجزیه کننده ساده خط فرمان که ورودی را بر اساس فاصله جدا می کند،
/// اما رشته های داخل گیومه (" ") را به عنوان یک توکن واحد در نظر می گیرد
/// تا مقادیری مانند نام یا ابعاد که شامل چند کلمه هستند هم قابل استفاده باشند.
/// </summary>
public static class CommandTokenizer
{
    public static string[] Tokenize(string input)
    {
        var tokens = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        foreach (var ch in input)
        {
            if (ch == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (char.IsWhiteSpace(ch) && !inQuotes)
            {
                if (current.Length > 0)
                {
                    tokens.Add(current.ToString());
                    current.Clear();
                }

                continue;
            }

            current.Append(ch);
        }

        if (current.Length > 0)
        {
            tokens.Add(current.ToString());
        }

        return tokens.ToArray();
    }
}
