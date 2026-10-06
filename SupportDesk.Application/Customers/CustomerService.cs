using SupportDesk.Application.Customers.Interfaces;
using SupportDesk.Domain.Models;
using SupportDesk.Domain.ValueObjects;

namespace SupportDesk.Application.Customers;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Customer> RegisterCustomerAsync(string name, string email)
    {
        var customerName = new CustomerName(name);
        var customerEmail = new Email(email);

        var customer = new Customer(customerName, customerEmail)
        {
            Id = Guid.NewGuid()
        };

        await _customerRepository.AddCustomerAsync(customer);

        return customer;
    }

    public async Task<Customer> GetCustomerByIdAsync(Guid customerId)
    {
        var customers = await _customerRepository.GetAllCustomersAsync();
        var customer = customers.FirstOrDefault(customer => customer.Id == customerId);

        if (customer is null)
        {
            throw new InvalidOperationException("The customer with the specified ID does not exist.");
        }

        return customer;
    }

    public async Task<IReadOnlyList<Customer>> GetAllCustomersAsync()
    {
        return await _customerRepository.GetAllCustomersAsync();
    }

    public async Task<Customer> UpdateCustomerAsync(
        Customer customer,
        string name,
        string email)
    {
        customer.Rename(new CustomerName(name));
        customer.ChangeEmail(new Email(email));

        await _customerRepository.UpdateCustomerAsync(customer);

        return customer;
    }
}
