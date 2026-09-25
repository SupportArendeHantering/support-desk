using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace SupportDeskt.Presentation.Navigation;

public interface INavigationService : INotifyPropertyChanged
{
    Page? CurrentPage { get; }

    void Navigate(AppPage page);
}
