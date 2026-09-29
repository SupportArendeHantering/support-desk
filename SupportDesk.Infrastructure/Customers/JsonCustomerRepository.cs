using SupportDesk.Application.Customers.Interfaces;
using SupportDesk.Domain.Models;
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
        List<CustomerData> customers;

        if (File.Exists(_filePath))
        {
            string json = await File.ReadAllTextAsync(_filePath);

            customers =
                JsonSerializer.Deserialize<List<CustomerData>>(
                    json,
                    JsonSettings.SerializerOptions)
                ?? new List<CustomerData>();
        }
        else
        {
            customers = new List<CustomerData>();
        }

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
}