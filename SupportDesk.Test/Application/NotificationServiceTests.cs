using SupportDesk.Application.Notifications;

namespace SupportDesk.Test.Application
{
    public class NotificationServiceTests
    {
        [Fact]
        public void Show_RaisesEvent_WithMessageAndType()
        {
            var service = new NotificationService();
            InfoBarMessageEventArgs? received = null;
            service.ShowInfoBarRequested += (_, args) => received = args;

            service.Show("Saved", InfoType.Warning);

            Assert.NotNull(received);
            Assert.Equal("Saved", received.Message);
            Assert.Equal(InfoType.Warning, received.Type);
        }

        [Fact]
        public void Show_UsesInfoAsDefaultType()
        {
            var service = new NotificationService();
            InfoBarMessageEventArgs? received = null;
            service.ShowInfoBarRequested += (_, args) => received = args;

            service.Show("Hello");

            Assert.NotNull(received);
            Assert.Equal(InfoType.Info, received.Type);
        }

        [Fact]
        public void Show_PassesServiceAsSender()
        {
            var service = new NotificationService();
            object? sender = null;
            service.ShowInfoBarRequested += (s, _) => sender = s;

            service.Show("Hello");

            Assert.Same(service, sender);
        }

        [Fact]
        public void Show_WithNoSubscribers_DoesNotThrow()
        {
            var service = new NotificationService();

            var exception = Record.Exception(() => service.Show("Hello"));

            Assert.Null(exception);
        }

        [Fact]
        public void Show_WithMultipleSubscribers_NotifiesAll()
        {
            var service = new NotificationService();
            var count = 0;
            service.ShowInfoBarRequested += (_, _) => count++;
            service.ShowInfoBarRequested += (_, _) => count++;

            service.Show("Hello");

            Assert.Equal(2, count);
        }

        [Fact]
        public void Show_AfterUnsubscribe_DoesNotNotifyHandler()
        {
            var service = new NotificationService();
            var count = 0;
            EventHandler<InfoBarMessageEventArgs> handler = (_, _) => count++;
            service.ShowInfoBarRequested += handler;
            service.ShowInfoBarRequested -= handler;

            service.Show("Hello");

            Assert.Equal(0, count);
        }
    }
}
