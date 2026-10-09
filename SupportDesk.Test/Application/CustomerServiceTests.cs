using NSubstitute;
using SupportDesk.Application.Customers;
using SupportDesk.Application.Customers.Interfaces;
using SupportDesk.Domain.Models;
using SupportDesk.Domain.ValueObjects;

namespace SupportDesk.Test.Application;

public class CustomerServiceTests
{
    [Fact]
    public async Task RegisterCustomerAsync_WithValidInput_AddsAndReturnsCustomer()
    {
        // Arrange
        var repository = Substitute.For<ICustomerRepository>();
        var service = new CustomerService(repository);

        // Act
        var result = await service.RegisterCustomerAsync(
            "  Test Company  ",
            "TEST@EXAMPLE.COM");

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Test Company", result.Name.Value);
        Assert.Equal("test@example.com", result.Email.Value);

        await repository.Received(1).AddCustomerAsync(result);
    }

    [Fact]
    public async Task GetAllCustomersAsync_ReturnsCustomersFromRepository()
    {
        // Arrange
        var repository = Substitute.For<ICustomerRepository>();
        var service = new CustomerService(repository);

        IReadOnlyList<Customer> customers =
        [
            CreateCustomer("Anna", "anna@example.com"),
            CreateCustomer("Bo", "bo@example.com")
        ];

        repository.GetAllCustomersAsync().Returns(customers);

        // Act
        var result = await service.GetAllCustomersAsync();

        // Assert
        Assert.Same(customers, result);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WhenCustomerExists_ReturnsCustomer()
    {
        // Arrange
        var repository = Substitute.For<ICustomerRepository>();
        var service = new CustomerService(repository);

        var expectedCustomer = CreateCustomer(
            "Anna",
            "anna@example.com");

        repository.GetAllCustomersAsync()
            .Returns([expectedCustomer]);

        // Act
        var result =
            await service.GetCustomerByIdAsync(expectedCustomer.Id);

        // Assert
        Assert.Same(expectedCustomer, result);
    }

    [Fact]
    public async Task UpdateCustomerAsync_ChangesCustomerAndUpdatesRepository()
    {
        // Arrange
        var repository = Substitute.For<ICustomerRepository>();
        var service = new CustomerService(repository);

        var customer = CreateCustomer(
            "Old Name",
            "old@example.com");

        // Act
        var result = await service.UpdateCustomerAsync(
            customer,
            "New Name",
            "new@example.com");

        // Assert
        Assert.Same(customer, result);
        Assert.Equal("New Name", customer.Name.Value);
        Assert.Equal("new@example.com", customer.Email.Value);

        await repository
            .Received(1)
            .UpdateCustomerAsync(customer);
    }

    private static Customer CreateCustomer(
        string name,
        string email)
    {
        return new Customer(
            new CustomerName(name),
            new Email(email))
        {
            Id = Guid.NewGuid()
        };
    }
}