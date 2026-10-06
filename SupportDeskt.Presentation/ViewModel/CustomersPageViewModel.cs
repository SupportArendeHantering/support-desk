using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SupportDesk.Application.Customers;
using SupportDesk.Domain.Models;
using SupportDesk.Domain.ValueObjects;
using SupportDeskt.Presentation.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SupportDeskt.Presentation.ViewModels;

public partial class CustomersPageViewModel(ICustomerService customerService, INavigationService navigationService) : ObservableObject
{
    public ObservableCollection<Customer> Customers { get; } = [];

    [ObservableProperty]
    public partial string CustomerName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EmailAddress { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string EditCustomerName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EditEmailAddress { get; set; } = string.Empty;

    private Customer? _selectedCustomer;
    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set => SetProperty(ref _selectedCustomer, value);
    }

    [ObservableProperty]
    public partial string StatusMessage { get; private set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    public partial bool IsBusy { get; private set; }

    public bool IsNotBusy => !IsBusy;

    public async Task LoadAsync()
    {
        IsBusy = true;

        try
        {
            var customers = await customerService.GetAllCustomersAsync();

            Customers.Clear();

            foreach (var customer in customers)
                Customers.Add(customer);

            StatusMessage = $"{Customers.Count} customers.";
        }
        catch (Exception)
        {
            StatusMessage = "The customers could not be loaded. Please check the file and path in the settings.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            var newCustomer = await customerService.RegisterCustomerAsync(CustomerName, EmailAddress);
            Customers.Add(newCustomer);
            ResetInputFields();
        }
        catch (ArgumentException ex)
        {
            StatusMessage = ex.Message;
            return;
        }
        catch (Exception)
        {
            StatusMessage = "Customer could not be saved.";
            return;
        }
        finally
        {
            IsBusy = false;
        }   
    }

    [RelayCommand]
    private async Task UpdateCustomerAsync(Customer customer)
    {
        if (customer is null)
        {
            StatusMessage = "No customer provided.";
            return;
        }

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            var newName = string.IsNullOrWhiteSpace(EditCustomerName)
                ? customer.Name.Value
                : new CustomerName(EditCustomerName).Value;

            var newEmail = string.IsNullOrWhiteSpace(EditEmailAddress)
                ? customer.Email.Value
                : new Email(EditEmailAddress).Value;

            await customerService.UpdateCustomerAsync(customer, newName, newEmail);

            var updatedCustomer = new Customer(new CustomerName(newName), new Email(newEmail))
            {
                Id = customer.Id
            };

            var index = Customers.IndexOf(customer);
            Customers[index] = updatedCustomer;

            StatusMessage = "Customer updated successfully.";

            ResetInputFields();

        }
        catch (ArgumentException ex)
        {
            StatusMessage = ex.Message;
        }
        catch (Exception)
        {
            StatusMessage = "Customer could not be updated.";
        }
        finally
        {
            IsBusy = false;
        }
    }


    private void ResetInputFields()
    {
        CustomerName = string.Empty;
        EmailAddress = string.Empty;
        EditCustomerName = string.Empty;
        EditEmailAddress = string.Empty;
    }
}
