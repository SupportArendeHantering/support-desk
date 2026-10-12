namespace SupportDesk.Application.Notifications;

public interface INotificationService
{
    event EventHandler<InfoBarMessageEventArgs> ShowInfoBarRequested;
    void Show(string message, InfoType type = InfoType.Info);
}
