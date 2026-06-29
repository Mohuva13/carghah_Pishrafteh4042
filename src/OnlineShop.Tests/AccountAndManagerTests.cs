using OnlineShop.Core.Entities;
using OnlineShop.Core.Exceptions;
using OnlineShop.Tests.TestHelpers;

namespace OnlineShop.Tests;

public class AccountAndManagerTests
{
    [Fact]
    public void Buyer_Constructor_ThrowsOnInvalidEmail()
    {
        Assert.Throws<InvalidFormatException>(() => new Buyer("validUser", "invalid-email", "09123456789", "Passw0rd1"));
    }

    [Fact]
    public void Buyer_Constructor_ThrowsOnInvalidPhone()
    {
        Assert.Throws<InvalidFormatException>(() => new Buyer("validUser", "a@b.com", "12345", "Passw0rd1"));
    }

    [Fact]
    public void Buyer_Constructor_ThrowsOnWeakPassword()
    {
        Assert.Throws<InvalidFormatException>(() => new Buyer("validUser", "a@b.com", "09123456789", "weak"));
    }

    [Fact]
    public void Buyer_VerifyPassword_DoesNotStorePlainTextPassword()
    {
        var buyer = new Buyer("validUser", "a@b.com", "09123456789", "Passw0rd1");

        Assert.True(buyer.VerifyPassword("Passw0rd1"));
        Assert.False(buyer.VerifyPassword("WrongPass1"));
        Assert.NotEqual("Passw0rd1", buyer.PasswordHash);
    }

    [Fact]
    public void Manager_Instance_IsSingleton()
    {
        TestContextFactory.CreateFreshContext();
        var first = Manager.Instance;
        var second = Manager.Instance;

        Assert.Same(first, second);
    }

    [Fact]
    public void Manager_HasFixedAdminCredentials()
    {
        TestContextFactory.CreateFreshContext();
        var manager = Manager.Instance;

        Assert.Equal("admin", manager.Username);
        Assert.True(manager.VerifyPassword("admin"));
    }

    [Fact]
    public void Manager_RoleName_IsManager()
    {
        TestContextFactory.CreateFreshContext();
        Assert.Equal("مدیر", Manager.Instance.RoleName);
    }

    [Fact]
    public void Account_ToString_ContainsUsernameAndRole()
    {
        var buyer = new Buyer("validUser", "a@b.com", "09123456789", "Passw0rd1");
        var text = buyer.ToString();

        Assert.Contains("validUser", text);
        Assert.Contains("خریدار", text);
    }
}
