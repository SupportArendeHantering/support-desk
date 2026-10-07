using SupportDesk.Domain.Models;
using SupportDesk.Domain.ValueObjects;
using SupportDesk.Infrastructure.Customers;

namespace SupportDesk.Test.Infrastructure;

public class JsonCustomerRepositoryTests : IDisposable
{
    private readonly string _directory;
    private readonly string _filePath;
    private readonly JsonCustomerRepository _repository;

    public JsonCustomerRepositoryTests()
    {
        _directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        _filePath = Path.Combine(_directory, "nested", "customers.json");
        _repository = new JsonCustomerRepository(_filePath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, true);
    }

    private static Customer CreateCustomer(string name, string email)
    {
        var customer = new Customer(new CustomerName(name), new Email(email))
        {
            Id = Guid.NewGuid()
        };

        return customer;
    }


    [Fact]
    public async Task GetAllCustomersAsync_WhenFileDoesNotExist_ReturnsEmptyList()
    {
        var result = await _repository.GetAllCustomersAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllCustomersAsync_WhenFileContainsNull_ReturnsEmptyList()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        await File.WriteAllTextAsync(_filePath, "null");

        var result = await _repository.GetAllCustomersAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task AddCustomerAsync_CreatesFileAndDirectory()
    {
        await _repository.AddCustomerAsync(CreateCustomer("Anton", "atte@example.com"));

        Assert.True(File.Exists(_filePath));
    }

    [Fact]
    public async Task AddCustomerAsync_LeavesNoTemporaryFile()
    {
        await _repository.AddCustomerAsync(CreateCustomer("Printer Andersson", "printer@example.com"));

        Assert.False(File.Exists(_filePath + ".tmp"));
    }

    [Fact]
    public async Task AddCustomerAsync_ThenGetAll_ReturnsAddedCustomer()
    {
        var customer = CreateCustomer("Frankie", "frankie@example.com");

        await _repository.AddCustomerAsync(customer);
        var result = await _repository.GetAllCustomersAsync();

        var addedCustomer = Assert.Single(result);
        Assert.Equal(customer.Id, addedCustomer.Id);
        Assert.Equal("Frankie", addedCustomer.Name.Value);
        Assert.Equal("frankie@example.com", addedCustomer.Email.Value);
    }

    [Fact]
    public async Task AddCustomerAsync_MultipleTimes_PreservesAllCustomers()
    {
        var first = CreateCustomer("Homer Simpsons", "homer@example.com");
        var second = CreateCustomer("Bart Simpsons", "bart@example.com");

        await _repository.AddCustomerAsync(first);
        await _repository.AddCustomerAsync(second);
        var result = await _repository.GetAllCustomersAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, customer => customer.Id == first.Id);
        Assert.Contains(result, customer => customer.Id == second.Id);
    }

    [Fact]
    public async Task UpdateCustomerAsync_ExistingCustomer_UpdatesNameAndEmail()
    {
        var customer = CreateCustomer("Max Volym", "maxvolym@example.com");
        await _repository.AddCustomerAsync(customer);

        var updated = new Customer(new CustomerName("Min Volym"), new Email("maxvolym.s@example.com"))
        {
            Id = customer.Id
        };
        await _repository.UpdateCustomerAsync(updated);

        var result = await _repository.GetAllCustomersAsync();
        var single = Assert.Single(result);
        Assert.Equal("Min Volym", single.Name.Value);
        Assert.Equal("maxvolym.s@example.com", single.Email.Value);
    }

    [Fact]
    public async Task UpdateCustomerAsync_DoesNotAffectOtherCustomers()
    {
        var first = CreateCustomer("Anna Andersson", "anna@example.com");
        var second = CreateCustomer("Bo Berg", "bo@example.com");
        await _repository.AddCustomerAsync(first);
        await _repository.AddCustomerAsync(second);

        var updated = new Customer(new CustomerName("Bo Bergman"), new Email("bo@example.com"))
        {
            Id = second.Id
        };

        await _repository.UpdateCustomerAsync(updated);

        var result = await _repository.GetAllCustomersAsync();
        Assert.Equal("Anna Andersson", result.Single(customer => customer.Id == first.Id).Name.Value);
        Assert.Equal("Bo Bergman", result.Single(customer => customer.Id == second.Id).Name.Value);
    }

    [Fact]
    public async Task UpdateCustomerAsync_UnknownCustomer_ThrowsInvalidOperationException()
    {
        var customer = CreateCustomer("Chris Griffin", "cg@example.com");

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _repository.UpdateCustomerAsync(customer));

        Assert.Equal("Customer not found.", exception.Message);
    }
}