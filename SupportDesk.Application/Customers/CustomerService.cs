using SupportDesk.Application.Customers.Interfaces;
using SupportDesk.Domain.Models;

namespace SupportDesk.Application.Customers;

public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }


public Customer RegisterCustomer(string name, string email)
    {
        var customerName = new CustomerName(name);
        var customerEmail = new Email(email);

        var customer = new Customer(customerName, customerEmail)
        {
            Id = Guid.NewGuid()
        };

        _customerRepository.AddCustomer(customer);  

        return customer;
    }
}

