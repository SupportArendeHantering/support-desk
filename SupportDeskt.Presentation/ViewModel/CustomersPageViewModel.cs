using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SupportDesk.Application.Customers;
using SupportDesk.Domain.Models;
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
            await customerService.RegisterCustomerAsync(CustomerName, EmailAddress);
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
            StatusMessage = "Customer saved successfully.";
        }

       // navigationService.Navigate(AppPage.Customer);
    }
}
