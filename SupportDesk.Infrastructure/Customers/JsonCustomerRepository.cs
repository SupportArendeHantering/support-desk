using System.Text.Json;
using SupportDesk.Application.Customers.Interfaces;
using SupportDesk.Domain.Models;
using SupportDesk.Infrastructure.Customers.Models;

namespace SupportDesk.Infrastructure.Customers;

public class JsonCustomerRepository : ICustomerRepository
{
    private readonly string _filePath;

    public JsonCustomerRepository(string filePath)
    {
        _filePath = filePath;
    }

    public void AddCustomer(Customer customer)
    {
        List<CustomerData> customers;

        if (File.Exists(_filePath))
        {
            string json = File.ReadAllText(_filePath);

            customers = JsonSerializer.Deserialize<List<CustomerData>>(json)
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
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(_filePath, updatedJson);
    }
}