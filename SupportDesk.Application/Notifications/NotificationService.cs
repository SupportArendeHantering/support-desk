namespace SupportDesk.Application.Notifications;

public class NotificationService : INotificationService
{
    public event EventHandler<InfoBarMessageEventArgs> ShowInfoBarRequested = delegate { };

    public void Show(string message, InfoType type = InfoType.Info)
    {
        ShowInfoBarRequested?.Invoke(this, new InfoBarMessageEventArgs(message, type));
    }
}
