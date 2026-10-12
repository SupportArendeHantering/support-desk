using SupportDesk.Domain.Models;
using SupportDesk.Domain.ValueObjects;

namespace SupportDesk.Test.Domain;

public class CustomerTests
{
    [Fact]
    public void Customer_WithValidNameAndEmail_CreatesCustomer()
    {
        // Arrange
        var name = new CustomerName("Test Company");
        var email = new Email("test@example.com");

        // Act
        var customer = new Customer(name, email)
        {
            Id = Guid.NewGuid()
        };

        // Assert
        Assert.Equal("Test Company", customer.Name.Value);
        Assert.Equal("test@example.com", customer.Email.Value);
        Assert.NotEqual(Guid.Empty, customer.Id);
    }

    [Fact]
    public void Rename_WithValidName_ChangesCustomerName()
    {
        // Arrange
        var customer = CreateCustomer();

        // Act
        customer.Rename(new CustomerName("New Company"));

        // Assert
        Assert.Equal("New Company", customer.Name.Value);
    }

    [Fact]
    public void ChangeEmail_WithValidEmail_ChangesCustomerEmail()
    {
        // Arrange
        var customer = CreateCustomer();

        // Act
        customer.ChangeEmail(new Email("new@example.com"));

        // Assert
        Assert.Equal("new@example.com", customer.Email.Value);
    }

    [Fact]
    public void CustomerName_WithOnlyWhitespace_ThrowsArgumentException()
    {
        // Act
        var act = () => new CustomerName("   ");

        // Assert
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Equal("value", exception.ParamName);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("1234567890123456789012345678901")]
    public void CustomerName_WithInvalidLength_ThrowsArgumentException(string name)
    {
        // Act
        var act = () => new CustomerName(name);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Email_WithInvalidFormat_ThrowsArgumentException()
    {
        // Act
        var act = () => new Email("not-an-email");

        // Assert
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Equal("value", exception.ParamName);
    }

    [Fact]
    public void Email_WithOnlyWhitespace_ThrowsArgumentException()
    {
        // Act
        var act = () => new Email("   ");

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Email_WithUppercase_NormalizesToLowercase()
    {
        // Act
        var email = new Email("TEST@EXAMPLE.COM");

        // Assert
        Assert.Equal("test@example.com", email.Value);
    }

    [Fact]
    public void CustomerName_WithWhitespace_TrimsValue()
    {
        // Act
        var name = new CustomerName("  Test Company  ");

        // Assert
        Assert.Equal("Test Company", name.Value);
    }

    private static Customer CreateCustomer()
    {
        return new Customer(
            new CustomerName("Test Company"),
            new Email("test@example.com"))
        {
            Id = Guid.NewGuid()
        };
    }
}