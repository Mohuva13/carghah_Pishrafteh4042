using OnlineShop.Core.Enums;

namespace OnlineShop.Core.Entities.Requests;

/// <summary>
/// کلاس پایه abstract برای مدیریت درخواست های ارسال شده به مدیر
/// (ثبت نام، تایید نظر، افزایش اعتبار). امکان ساخت شیء مستقیم از این کلاس وجود ندارد.
/// </summary>
public abstract class RequestBase
{
    protected RequestBase(int id, RequestType type, string requesterUsername)
    {
        Id = id;
        Type = type;
        RequesterUsername = requesterUsername;
        Status = RequestStatus.Pending;
        CreatedAt = DateTime.Now;
    }

    public int Id { get; }

    public RequestType Type { get; }

    public string RequesterUsername { get; }

    public RequestStatus Status { get; private set; }

    public DateTime CreatedAt { get; }

    /// <summary>عملیاتی که هنگام تایید درخواست توسط مدیر باید روی سیستم اعمال شود.</summary>
    protected abstract void OnApprove();

    /// <summary>عملیاتی که هنگام رد درخواست توسط مدیر باید روی سیستم اعمال شود (در صورت نیاز).</summary>
    protected virtual void OnReject()
    {
    }

    public void Approve()
    {
        EnsurePending();
        OnApprove();
        Status = RequestStatus.Approved;
    }

    public void Reject()
    {
        EnsurePending();
        OnReject();
        Status = RequestStatus.Rejected;
    }

    private void EnsurePending()
    {
        if (Status != RequestStatus.Pending)
        {
            throw new Exceptions.DomainException($"درخواست #{Id} قبلا بررسی شده است (وضعیت فعلی: {Status}).");
        }
    }

    /// <summary>توضیح مختص هر نوع درخواست، برای نمایش در لیست درخواست های مدیر.</summary>
    public abstract string GetSummary();

    public override string ToString() =>
        $"#{Id} | نوع: {TypeDisplay()} | درخواست دهنده: {RequesterUsername} | وضعیت: {StatusDisplay()} | تاریخ: {CreatedAt:yyyy/MM/dd HH:mm}{Environment.NewLine}    {GetSummary()}";

    private string TypeDisplay() => Type switch
    {
        RequestType.Registration => "ثبت نام",
        RequestType.CommentApproval => "تایید نظر",
        RequestType.CreditCharge => "افزایش اعتبار",
        _ => Type.ToString()
    };

    private string StatusDisplay() => Status switch
    {
        RequestStatus.Pending => "در انتظار",
        RequestStatus.Approved => "تایید شده",
        RequestStatus.Rejected => "رد شده",
        _ => Status.ToString()
    };
}
