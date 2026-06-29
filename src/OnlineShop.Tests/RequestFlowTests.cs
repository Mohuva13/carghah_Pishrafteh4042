using OnlineShop.Core.Entities;
using OnlineShop.Core.Entities.Requests;
using OnlineShop.Core.Enums;

namespace OnlineShop.Tests;

public class RequestFlowTests
{
    [Fact]
    public void RegistrationRequest_Approve_SetsBuyerApproved()
    {
        var buyer = new Buyer("newUser", "a@b.com", "09123456789", "Passw0rd1");
        var request = new RegistrationRequest(1, buyer);

        request.Approve();

        Assert.True(buyer.IsApproved);
        Assert.Equal(RequestStatus.Approved, request.Status);
    }

    [Fact]
    public void RegistrationRequest_Reject_DoesNotApproveBuyer()
    {
        var buyer = new Buyer("newUser", "a@b.com", "09123456789", "Passw0rd1");
        var request = new RegistrationRequest(1, buyer);

        request.Reject();

        Assert.False(buyer.IsApproved);
        Assert.Equal(RequestStatus.Rejected, request.Status);
    }

    [Fact]
    public void CommentApprovalRequest_Approve_ApprovesComment()
    {
        var comment = new Comment(1, "user1", 10, "متن نظر", true);
        var request = new CommentApprovalRequest(1, comment);

        request.Approve();

        Assert.Equal(CommentStatus.Approved, comment.Status);
    }

    [Fact]
    public void CommentApprovalRequest_Reject_RejectsComment()
    {
        var comment = new Comment(1, "user1", 10, "متن نظر", true);
        var request = new CommentApprovalRequest(1, comment);

        request.Reject();

        Assert.Equal(CommentStatus.Rejected, comment.Status);
    }

    [Fact]
    public void CreditChargeRequest_Approve_IncreasesBuyerCredit()
    {
        var buyer = new Buyer("newUser", "a@b.com", "09123456789", "Passw0rd1");
        var request = new CreditChargeRequest(1, buyer, 500_000, "1234567812345678");

        request.Approve();

        Assert.Equal(500_000, buyer.Credit);
        Assert.EndsWith("5678", request.MaskedCardNumber);
        Assert.DoesNotContain("1234567812345678", request.MaskedCardNumber);
    }

    [Fact]
    public void Request_CannotBeApprovedTwice()
    {
        var buyer = new Buyer("newUser", "a@b.com", "09123456789", "Passw0rd1");
        var request = new RegistrationRequest(1, buyer);
        request.Approve();

        Assert.Throws<Core.Exceptions.DomainException>(() => request.Approve());
    }
}
