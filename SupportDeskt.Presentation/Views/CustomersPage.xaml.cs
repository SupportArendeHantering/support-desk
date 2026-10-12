using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SupportDeskt.Presentation.ViewModels;

namespace SupportDeskt.Presentation.Views;

public sealed partial class CustomersPage : Page
{
    public CustomersPageViewModel ViewModel { get; }

    public CustomersPage(CustomersPageViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadAsync();
    }
}