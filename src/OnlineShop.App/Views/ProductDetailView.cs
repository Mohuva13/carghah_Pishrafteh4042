using OnlineShop.Controllers;
using OnlineShop.Core.Entities;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.App.Views;

/// <summary>صفحه جزئیات کامل یک کالا: مشخصات، نظرات تایید شده، میانگین نمره، افزودن به سبد، ثبت نظر و نمره.</summary>
public static class ProductDetailView
{
    public static void Show(ShopContext context, ShoppingCart guestCart, Product product)
    {
        while (true)
        {
            ConsoleUi.PrintHeader($"جزئیات کالا: {product.Name}");
            Console.WriteLine(product);
            Console.WriteLine();
            Console.WriteLine("نظرات تایید شده:");
            var approved = product.ApprovedComments.ToList();
            if (approved.Count == 0)
            {
                Console.WriteLine("  (نظری ثبت نشده است)");
            }
            else
            {
                foreach (var comment in approved)
                {
                    Console.WriteLine($"  - {comment}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("1. افزودن به سبد خرید");
            Console.WriteLine("2. ثبت نظر برای این کالا");
            Console.WriteLine("3. ثبت نمره برای این کالا");
            Console.WriteLine("0. بازگشت");
            Console.Write("انتخاب شما: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    AddToCart(context, guestCart, product);
                    break;
                case "2":
                    AddComment(context, product);
                    break;
                case "3":
                    AddRating(context, product);
                    break;
                case "0":
                    return;
                default:
                    ConsoleUi.PrintError("گزینه نامعتبر است.");
                    break;
            }
        }
    }

    private static void AddToCart(ShopContext context, ShoppingCart guestCart, Product product)
    {
        var quantity = ConsoleUi.ReadInt("تعداد");

        try
        {
            var buyer = context.CurrentBuyer;
            if (buyer is null)
            {
                // امکان امتیازی: مهمان هم می تواند به سبد موقت اضافه کند، اما برای نهایی کردن خرید باید لاگین کند.
                guestCart.AddProduct(product, quantity);
                ConsoleUi.PrintSuccess("کالا به سبد خرید موقت شما اضافه شد. برای نهایی کردن خرید باید ابتدا وارد سامانه شوید.");
            }
            else
            {
                new CartController(context).AddToCart(buyer, product.Id, quantity);
                ConsoleUi.PrintSuccess("کالا به سبد خرید شما اضافه شد.");
            }
        }
        catch (DomainException ex)
        {
            ConsoleUi.PrintError(ex.Message);
        }

        ConsoleUi.Pause();
    }

    private static void AddComment(ShopContext context, Product product)
    {
        var buyer = context.CurrentBuyer;
        if (buyer is null)
        {
            ConsoleUi.PrintError("برای ثبت نظر ابتدا باید وارد سامانه شوید.");
            ConsoleUi.Pause();
            return;
        }

        var text = ConsoleUi.ReadNonEmpty("متن نظر شما");

        try
        {
            new CommentController(context).AddComment(buyer, product.Id, text);
            ConsoleUi.PrintSuccess("نظر شما ثبت شد و پس از تایید مدیر در لیست نظرات نمایش داده می شود.");
        }
        catch (DomainException ex)
        {
            ConsoleUi.PrintError(ex.Message);
        }

        ConsoleUi.Pause();
    }

    private static void AddRating(ShopContext context, Product product)
    {
        var buyer = context.CurrentBuyer;
        if (buyer is null)
        {
            ConsoleUi.PrintError("برای نمره دهی ابتدا باید وارد سامانه شوید.");
            ConsoleUi.Pause();
            return;
        }

        var score = ConsoleUi.ReadInt("نمره شما به این کالا (۱ تا ۵)");

        try
        {
            new RatingController(context).RateProduct(buyer, product.Id, score);
            ConsoleUi.PrintSuccess("نمره شما با موفقیت ثبت شد.");
        }
        catch (DomainException ex)
        {
            ConsoleUi.PrintError(ex.Message);
        }

        ConsoleUi.Pause();
    }
}
