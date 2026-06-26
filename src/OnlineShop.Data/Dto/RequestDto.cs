using OnlineShop.Core.Enums;

namespace OnlineShop.Data.Dto;

/// <summary>
/// نسخه قابل سریالایز (flat) از یک درخواست. فیلد <see cref="Type"/> مشخص می کند
/// این رکورد باید به کدام کلاس واقعی (RegistrationRequest, CommentApprovalRequest, CreditChargeRequest) تبدیل شود.
/// </summary>
public sealed class RequestDto
{
    public int Id { get; set; }

    public RequestType Type { get; set; }

    public string RequesterUsername { get; set; } = string.Empty;

    public RequestStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    // Registration -> RequesterUsername نقش نام کاربری خریدار در انتظار را دارد

    // CommentApproval
    public int? CommentId { get; set; }

    public int? CommentProductId { get; set; }

    // CreditCharge
    public decimal? Amount { get; set; }

    public string? MaskedCardNumber { get; set; }
}
