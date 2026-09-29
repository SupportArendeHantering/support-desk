using SupportDesk.Application.Customers.Interfaces;
using SupportDesk.Domain.Models;
using SupportDesk.Domain.ValueObjects;
using SupportDesk.Infrastructure.Customers.Models;
using SupportDesk.Infrastructure.Settings;
using System.Text.Json;

namespace SupportDesk.Infrastructure.Customers;

public class JsonCustomerRepository : ICustomerRepository
{
    private readonly string _filePath;

    public JsonCustomerRepository(string filePath)
    {
        _filePath = filePath;
    }

    public async Task AddCustomerAsync(Customer customer)
    {
        List<CustomerData> customers = await GetAllCustomerDataListAsync();

        var customerData = new CustomerData
        {
            Id = customer.Id,
            Name = customer.Name.Value,
            Email = customer.Email.Value
        };

        customers.Add(customerData);

        string updatedJson = JsonSerializer.Serialize(
            customers,
            JsonSettings.SerializerOptions);

        await File.WriteAllTextAsync(_filePath, updatedJson);
    }

    private async Task<List<CustomerData>> GetAllCustomerDataListAsync()
    {
        if (!File.Exists(_filePath))
            return [];

        string json = await File.ReadAllTextAsync(_filePath);

        return JsonSerializer.Deserialize<List<CustomerData>>(
                   json,
                   JsonSettings.SerializerOptions)
               ?? [];
    }

    public async Task<IReadOnlyList<Customer>> GetAllCustomersAsync()
    {
        List<CustomerData> customerDataList = await GetAllCustomerDataListAsync();

        var customers = customerDataList.Select(data =>
            new Customer(new CustomerName(data.Name), new Email(data.Email))
            {
                Id = data.Id
            }).ToList();

        return customers;
    }
}