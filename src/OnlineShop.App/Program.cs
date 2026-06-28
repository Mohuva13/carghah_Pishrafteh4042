using OnlineShop.App.Seed;
using OnlineShop.App.Views;
using OnlineShop.Controllers;
using OnlineShop.Core.Entities;
using OnlineShop.Data.Persistence;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

var dataFilePath = Path.Combine(FindRepositoryRoot(), "data", "store.json");
var dataStore = new DataStore(dataFilePath);
dataStore.Load();

var context = new ShopContext(dataStore);

if (Manager.Instance.Products.Count == 0)
{
    DemoDataSeeder.Seed(context);
}

HomeView.Run(context);

/// <summary>
/// فایل داده را در پوشه «data» کنار فایل sln نگه می داریم تا مستقل از پوشه اجرای dotnet
/// (bin/Debug/...) همیشه در یک مسیر ثابت و قابل پیش بینی ذخیره و بازیابی شود.
/// </summary>
static string FindRepositoryRoot()
{
    var directory = new DirectoryInfo(AppContext.BaseDirectory);
    while (directory is not null && !directory.GetFiles("*.slnx").Any() && !directory.GetFiles("*.sln").Any())
    {
        directory = directory.Parent;
    }

    return directory?.FullName ?? AppContext.BaseDirectory;
}
