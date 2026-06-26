using OnlineShop.Core.Enums;

namespace OnlineShop.Core.Entities.Requests;

/// <summary>درخواست ثبت نام یک خریدار جدید که باید توسط مدیر تایید یا رد شود.</summary>
public sealed class RegistrationRequest : RequestBase
{
    public RegistrationRequest(int id, Buyer pendingBuyer)
        : base(id, RequestType.Registration, pendingBuyer.Username)
    {
        PendingBuyer = pendingBuyer;
    }

    /// <summary>سازنده مخصوص بازیابی از فایل داده.</summary>
    public RegistrationRequest(int id, Buyer pendingBuyer, RequestStatus status, DateTime createdAt)
        : base(id, RequestType.Registration, pendingBuyer.Username, status, createdAt)
    {
        PendingBuyer = pendingBuyer;
    }

    public Buyer PendingBuyer { get; }

    protected override void OnApprove()
    {
        PendingBuyer.IsApproved = true;
    }

    protected override void OnReject()
    {
        Manager.Instance.RemoveBuyer(PendingBuyer.Username);
    }

    public override string GetSummary() =>
        $"ثبت نام کاربر جدید «{PendingBuyer.Username}» ({PendingBuyer.Email})";
}
