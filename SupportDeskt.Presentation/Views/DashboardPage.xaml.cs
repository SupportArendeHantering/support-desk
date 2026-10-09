using Microsoft.UI.Xaml.Controls;
using SupportDeskt.Presentation.ViewModels;

namespace SupportDeskt.Presentation.Views;

public sealed partial class DashboardPage : Page
{
    public DashboardPageViewModel ViewModel { get; }
    public DashboardPage(DashboardPageViewModel viewModel)
    {
        ViewModel = viewModel;

        InitializeComponent();

    }
}