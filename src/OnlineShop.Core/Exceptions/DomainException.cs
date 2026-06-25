namespace OnlineShop.Core.Exceptions;

/// <summary>استثنای پایه برای تمام خطاهای منطق کسب و کار (Domain) پروژه.</summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}

/// <summary>زمانی پرتاب می شود که ورودی کاربر فرمت معتبری (طبق Regex) نداشته باشد.</summary>
public sealed class InvalidFormatException : DomainException
{
    public InvalidFormatException(string fieldName, string value)
        : base($"مقدار وارد شده برای «{fieldName}» ({value}) فرمت معتبری ندارد.")
    {
    }
}

/// <summary>زمانی پرتاب می شود که موجودی کالا برای برداشت درخواستی کافی نباشد.</summary>
public sealed class InsufficientStockException : DomainException
{
    public InsufficientStockException(string productName, int available, int requested)
        : base($"موجودی کالای «{productName}» کافی نیست. موجود: {available}, درخواستی: {requested}")
    {
    }
}

/// <summary>زمانی پرتاب می شود که اعتبار حساب کاربر برای پرداخت کافی نباشد.</summary>
public sealed class InsufficientCreditException : DomainException
{
    public InsufficientCreditException(decimal available, decimal required)
        : base($"اعتبار حساب کافی نیست. موجودی: {available:N0}, مبلغ لازم: {required:N0}")
    {
    }
}

/// <summary>زمانی پرتاب می شود که کاربری بدون خرید کالا، بخواهد به آن نظر/نمره ثبت کند.</summary>
public sealed class PurchaseRequiredException : DomainException
{
    public PurchaseRequiredException(string action)
        : base($"برای «{action}» ابتدا باید کالا را خریداری کرده باشید.")
    {
    }
}

/// <summary>زمانی پرتاب می شود که یک موجودیت با شناسه داده شده پیدا نشود.</summary>
public sealed class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object id)
        : base($"{entityName} با شناسه «{id}» یافت نشد.")
    {
    }
}

/// <summary>زمانی پرتاب می شود که دستور خط فرمان مدیر نامعتبر باشد.</summary>
public sealed class InvalidCommandException : DomainException
{
    public InvalidCommandException(string message) : base(message)
    {
    }
}

/// <summary>زمانی پرتاب می شود که نام کاربری یا رمز عبور در ورود اشتباه باشد.</summary>
public sealed class AuthenticationException : DomainException
{
    public AuthenticationException(string message) : base(message)
    {
    }
}
