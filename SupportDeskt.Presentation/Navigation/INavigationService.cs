using Microsoft.UI.Xaml.Controls;
using SupportDesk.Domain.Models;
using System.ComponentModel;

namespace SupportDeskt.Presentation.Navigation;

public interface INavigationService : INotifyPropertyChanged
{
    Page? CurrentPage { get; }

    SupportTicket? SelectedTicket { get; }

    void Navigate(AppPage page);

    void NavigateToTicketDetails(SupportTicket ticket);

    void BackToTickets();
}