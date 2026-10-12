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

        await WriteJsonToDisk(customers);
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

    public async Task UpdateCustomerAsync(Customer customer)
    {
        List<CustomerData> customers =
            await GetAllCustomerDataListAsync();

        CustomerData? existingCustomer =
            customers.FirstOrDefault(c => c.Id == customer.Id);

        if (existingCustomer is null)
            throw new InvalidOperationException("Customer not found.");

        existingCustomer.Name = customer.Name.Value;
        existingCustomer.Email = customer.Email.Value;

        await WriteJsonToDisk(customers);
    }

    private async Task WriteJsonToDisk(List<CustomerData> customers)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);

        var json = JsonSerializer.Serialize(customers, JsonSettings.SerializerOptions);
        var temporaryPath = _filePath + ".tmp";

        await File.WriteAllTextAsync(temporaryPath, json);
        File.Move(temporaryPath, _filePath, overwrite: true);
    }
}