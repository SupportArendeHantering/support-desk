using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using System;

namespace SupportDeskt.Presentation.Navigation;

public partial class NavigationService(IServiceProvider services) : ObservableObject, INavigationService
{
    [ObservableProperty] 
    public partial Page? CurrentPage { get; private set; }

    public void Navigate(AppPage page)
    {
       CurrentPage = page switch
       {
           _ => throw new ArgumentOutOfRangeException(nameof(page))
       };
    }
}