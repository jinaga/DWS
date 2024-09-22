using Notification.Wpf;

namespace DWS.Console.Asynchronous;

public class CommandProcessor
{
    private readonly NotificationManager notificationManager;

    public CommandProcessor(NotificationManager notificationManager)
    {
        this.notificationManager = notificationManager;
    }

    public void Run(Func<Task> handle)
    {
        handle().ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                notificationManager.Show(new NotificationContent
                {
                    Title = "Error while saving",
                    Message = t.Exception?.Message,
                    Type = NotificationType.Error
                });
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}
