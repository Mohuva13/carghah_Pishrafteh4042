using OnlineShop.Controllers;
using OnlineShop.Controllers.Cli;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.App.Views;

/// <summary>
/// ناحیه کاربری مدیر: طبق صورت پروژه به صورت کامندلاینی (بدون منوهای تو در تو) عمل می کند.
/// برای راهنما دستور «Help» و برای خروج «Exit» یا «Logout» را وارد کنید.
/// </summary>
public static class ManagerAreaView
{
    public static void Show(ShopContext context)
    {
        var cli = new ManagerCliController(context);

        ConsoleUi.PrintHeader("ناحیه کاربری مدیر (خط فرمان)");
        Console.WriteLine("برای مشاهده لیست دستورات، «Help» را وارد کنید. برای خروج «Exit» را وارد کنید.");

        while (true)
        {
            Console.Write($"{Environment.NewLine}manager> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            var trimmed = input.Trim();
            if (trimmed.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("logout", StringComparison.OrdinalIgnoreCase))
            {
                new AuthController(context).Logout();
                return;
            }

            try
            {
                var output = cli.Execute(trimmed);
                Console.WriteLine(output);
            }
            catch (InvalidCommandException ex)
            {
                ConsoleUi.PrintError(ex.Message);
            }
            catch (DomainException ex)
            {
                ConsoleUi.PrintError(ex.Message);
            }
        }
    }
}
