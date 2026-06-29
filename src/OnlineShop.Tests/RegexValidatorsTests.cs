using OnlineShop.Core.Utils;

namespace OnlineShop.Tests;

public class RegexValidatorsTests
{
    [Theory]
    [InlineData("ali@example.com", true)]
    [InlineData("ali.reza@sub.example.co", true)]
    [InlineData("not-an-email", false)]
    [InlineData("missing@domain", false)]
    [InlineData("@nodomain.com", false)]
    public void IsValidEmail_ReturnsExpected(string email, bool expected)
    {
        Assert.Equal(expected, RegexValidators.IsValidEmail(email));
    }

    [Theory]
    [InlineData("09123456789", true)]
    [InlineData("09351112233", true)]
    [InlineData("0912345678", false)]
    [InlineData("9123456789", false)]
    [InlineData("091234567890", false)]
    public void IsValidPhoneNumber_ReturnsExpected(string phone, bool expected)
    {
        Assert.Equal(expected, RegexValidators.IsValidPhoneNumber(phone));
    }

    [Theory]
    [InlineData("Passw0rd", true)]
    [InlineData("abcdefg1", true)]
    [InlineData("short1", false)]
    [InlineData("onlyletters", false)]
    [InlineData("12345678", false)]
    public void IsValidPassword_ReturnsExpected(string password, bool expected)
    {
        Assert.Equal(expected, RegexValidators.IsValidPassword(password));
    }

    [Theory]
    [InlineData("1234567812345678", true)]
    [InlineData("1234 5678 1234 5678", true)]
    [InlineData("1234-5678-1234-5678", true)]
    [InlineData("123456781234567", false)]
    [InlineData("abcd5678123456780", false)]
    public void IsValidCardNumber_ReturnsExpected(string cardNumber, bool expected)
    {
        Assert.Equal(expected, RegexValidators.IsValidCardNumber(cardNumber));
    }

    [Theory]
    [InlineData("1234", true)]
    [InlineData("123456", true)]
    [InlineData("123", false)]
    [InlineData("1234567", false)]
    public void IsValidCardPassword_ReturnsExpected(string cardPassword, bool expected)
    {
        Assert.Equal(expected, RegexValidators.IsValidCardPassword(cardPassword));
    }

    [Theory]
    [InlineData("123", true)]
    [InlineData("1234", true)]
    [InlineData("12", false)]
    [InlineData("12345", false)]
    public void IsValidCvv2_ReturnsExpected(string cvv2, bool expected)
    {
        Assert.Equal(expected, RegexValidators.IsValidCvv2(cvv2));
    }
}
