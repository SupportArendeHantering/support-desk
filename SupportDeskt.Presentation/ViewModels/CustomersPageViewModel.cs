using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SupportDesk.Application.Customers;
using SupportDesk.Domain.Models;
using SupportDesk.Domain.ValueObjects;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SupportDeskt.Presentation.ViewModels;

public partial class CustomersPageViewModel(ICustomerService customerService) : ObservableObject
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

            StatusMessage = $"{Customers.Count} kunder.";
        }
        catch (Exception)
        {
            StatusMessage = "Kundlista kunde inte laddas";
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
        catch (InvalidOperationException ex)
        {
            StatusMessage = ex.Message;
            return;
        }
        catch (ArgumentException ex)
        {
            StatusMessage = ex.Message;
            return;
        }
        catch (Exception)
        {
            StatusMessage = "Kunden kunde inte sparas!";
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
            StatusMessage = "Det finns ingen kund.";
            return;
        }

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            if (string.IsNullOrEmpty(EditCustomerName) && string.IsNullOrEmpty(EditEmailAddress))
            {
                StatusMessage = "Inga ändringar att spara.";
                return;
            }

            var newName = string.IsNullOrWhiteSpace(EditCustomerName)
                ? customer.Name.Value
                : new CustomerName(EditCustomerName).Value;

            var newEmail = string.IsNullOrWhiteSpace(EditEmailAddress)
                ? customer.Email.Value
                : new Email(EditEmailAddress).Value;

            var updatedCustomer = await customerService.UpdateCustomerAsync(customer, newName, newEmail);

            var index = Customers.IndexOf(customer);
            Customers[index] = updatedCustomer;

            StatusMessage = "Kunden Updaterad.";

            ResetInputFields();
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = ex.Message;
        }
        catch (ArgumentException ex)
        {
            StatusMessage = ex.Message;
            return;
        }
        catch (Exception)
        {
            StatusMessage = "Kunden kunde inte updateras.";
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
