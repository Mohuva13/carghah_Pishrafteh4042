using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Requests;
using OnlineShop.Core.Exceptions;

namespace OnlineShop.Controllers;

/// <summary>مسئول مشاهده و تایید/رد درخواست های ارسال شده به مدیر (ثبت نام، تایید نظر، افزایش اعتبار).</summary>
public sealed class RequestController
{
    private readonly ShopContext _context;

    public RequestController(ShopContext context)
    {
        _context = context;
    }

    public IEnumerable<RequestBase> GetPendingRequests() => Manager.Instance.PendingRequests;

    public IEnumerable<RequestBase> GetAllRequests() => Manager.Instance.Requests;

    public void Approve(int requestId)
    {
        var request = GetOrThrow(requestId);
        request.Approve();
        _context.SaveChanges();
    }

    public void Reject(int requestId)
    {
        var request = GetOrThrow(requestId);
        request.Reject();
        _context.SaveChanges();
    }

    private static RequestBase GetOrThrow(int requestId) =>
        Manager.Instance.Requests.FirstOrDefault(r => r.Id == requestId)
        ?? throw new EntityNotFoundException("درخواست", requestId);
}
