using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SupportDesk.Application.Comments;
using SupportDesk.Application.Customers;
using SupportDesk.Application.Extensions;
using SupportDesk.Application.Notifications;
using SupportDesk.Application.Tickets;
using SupportDesk.Domain.Models;
using SupportDeskt.Presentation.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SupportDeskt.Presentation.ViewModels;

public partial class TicketDetailsPageViewModel(
    INavigationService navigationService,
    ICustomerService customerService,
    ITicketService ticketService,
    ITicketAssigneeService assigneeService,
    ICommentService commentService,
    INotificationService notificationService) : ObservableObject
{
    [ObservableProperty]
    public partial SupportTicket? Ticket { get; private set; }

    public ObservableCollection<Customer> Customers { get; } = [];

    public ObservableCollection<TicketAssignee> Assignees { get; } = [];

    public ObservableCollection<TicketPriority> Priorities { get; } =
        new(Enum.GetValues<TicketPriority>());

    public ObservableCollection<TicketStatus> Statuses { get; } =
        new(Enum.GetValues<TicketStatus>());

    public ObservableCollection<CommentItem> Comments { get; } = [];

    [ObservableProperty]
    public partial string EditTitle { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EditDescription { get; set; } = string.Empty;

    [ObservableProperty]
    public partial Customer? SelectedCustomer { get; set; }

    [ObservableProperty]
    public partial TicketAssignee? SelectedAssignee { get; set; }

    [ObservableProperty]
    public partial TicketPriority SelectedPriority { get; set; }

    [ObservableProperty]
    public partial TicketStatus SelectedStatus { get; set; }

    [ObservableProperty]
    public partial string NewCommentText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string StatusMessage { get; private set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    public partial bool IsBusy { get; private set; }

    public bool IsNotBusy => !IsBusy;

    public async Task LoadAsync()
    {
        Ticket = navigationService.SelectedTicket;

        if (Ticket is null)
        {
            StatusMessage = "Inget ärende har valts.";
            notificationService.Show(StatusMessage, InfoType.Warning);
            return;
        }

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            EditTitle = Ticket.Title.Value;
            EditDescription = Ticket.Description.Value;
            SelectedPriority = Ticket.Priority;
            SelectedStatus = Ticket.Status;

            var customers = await customerService.GetAllCustomersAsync();

            Customers.Clear();

            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }

            SelectedCustomer = Customers.FirstOrDefault(customer => customer.Id == Ticket.AssignedCustomerId);

            Assignees.Clear();

            foreach (var assignee in assigneeService.GetAllAssignees())
            {
                Assignees.Add(assignee);
            }

            SelectedAssignee = null;

            if (Ticket.AssigneeId is int assigneeId)
            {
                SelectedAssignee =
                    assigneeService.GetAssigneeById(assigneeId);
            }
            await LoadCommentsAsync();
        }
        catch (Exception)
        {
            StatusMessage =
                "Alla uppgifter för ärendet kunde inte laddas.";
            notificationService.Show(StatusMessage, InfoType.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Ticket is null)
        {
            StatusMessage = "Inget ärende har valts.";
            notificationService.Show(StatusMessage, InfoType.Warning);
            return;
        }

        if (SelectedCustomer is null)
        {
            StatusMessage = "Välj en kund.";
            notificationService.Show(StatusMessage, InfoType.Warning);
            return;
        }

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            await ticketService.UpdateTicketDetailsAsync(
                Ticket,
                EditTitle,
                EditDescription);

            if (Ticket.AssignedCustomerId != SelectedCustomer.Id)
            {
                await ticketService.AssignCustomerToTicketAsync(
                    Ticket,
                    SelectedCustomer.Id);
            }

            if (Ticket.Priority != SelectedPriority)
            {
                await ticketService.ChangePriorityAsync(
                    Ticket,
                    SelectedPriority);
            }

            if (SelectedAssignee is not null &&
                Ticket.AssigneeId != SelectedAssignee.Id)
            {
                await ticketService.AssignAssigneeAsync(
                    Ticket,
                    SelectedAssignee.Id);
            }

            if (Ticket.Status != SelectedStatus)
            {
                await ticketService.ChangeStatusAsync(
                    Ticket,
                    SelectedStatus);
            }

            StatusMessage = "Ärendet uppdaterades.";
            notificationService.Show(StatusMessage, InfoType.Success);
        }
        catch (ArgumentException ex)
        {
            StatusMessage = ex.ToUiMessage();
            notificationService.Show(StatusMessage, InfoType.Warning);
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = ex.ToUiMessage();
            notificationService.Show(StatusMessage, InfoType.Warning);
        }
        catch (Exception)
        {
            StatusMessage = "Ärendet kunde inte uppdateras.";
            notificationService.Show(StatusMessage, InfoType.Warning);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadCommentsAsync()
    {
        if (Ticket is null)
            return;

        var comments =
            await commentService.GetCommentsByTicketIdAsync(Ticket.Id);

        Comments.Clear();

        foreach (var comment in comments
                     .OrderBy(comment => comment.CreatedAt))
        {
            var assignee =
                assigneeService.GetAssigneeById(comment.CreatedBy);

            Comments.Add(new CommentItem
            {
                AuthorName =
                    assignee?.Name ?? "Okänd handläggare",

                Text =
                    comment.CommentText.Value,

                CreatedAt =
                    comment.CreatedAt
            });
        }
    }

    [RelayCommand]
    private async Task AddCommentAsync()
    {
        if (Ticket is null)
        {
            StatusMessage = "Inget ärende har valts.";
            notificationService.Show(StatusMessage, InfoType.Warning);
            return;
        }

        if (Ticket.AssigneeId is not int assigneeId)
        {
            StatusMessage =
                "Tilldela en handläggare innan du lägger till en kommentar.";
            notificationService.Show(StatusMessage, InfoType.Warning);
            return;
        }

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            await commentService.AddCommentAsync(
                Ticket.Id,
                assigneeId,
                NewCommentText);

            NewCommentText = string.Empty;

            await LoadCommentsAsync();

            StatusMessage = "Kommentaren lades till.";
            notificationService.Show(StatusMessage, InfoType.Success);
        }
        catch (ArgumentException ex)
        {
            StatusMessage = ex.ToUiMessage();
            notificationService.Show(StatusMessage, InfoType.Warning);
        }
        catch (Exception)
        {
            StatusMessage =
                "Kommentaren kunde inte läggas till.";
            notificationService.Show(StatusMessage, InfoType.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Back()
    {
        navigationService.BackToTickets();
    }
}

public class CommentItem
{
    public string AuthorName { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}