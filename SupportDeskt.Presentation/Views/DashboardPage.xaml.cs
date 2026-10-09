using Microsoft.UI.Xaml.Controls;
using SupportDeskt.Presentation.ViewModels;
using System.Threading.Tasks;

namespace SupportDeskt.Presentation.Views;

public sealed partial class DashboardPage : Page
{
    public DashboardPageViewModel ViewModel { get; }
    public DashboardPage(DashboardPageViewModel viewModel)
    {
        ViewModel = viewModel;

        InitializeComponent();

    }

    private async void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await ViewModel.LoadTicketsAsync();
    }
}