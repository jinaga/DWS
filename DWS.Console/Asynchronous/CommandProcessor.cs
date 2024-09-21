using Notification.Wpf;

namespace DWS.Console.Asynchronous;

public class CommandProcessor
{
    private readonly NotificationManager notificationManager;

    public CommandProcessor(NotificationManager notificationManager)
    {
        this.notificationManager = notificationManager;
    }

    public async void Run(Func<Task> handle)
    {
        try
        {
            await handle();
        }
        catch (Exception ex)
        {
            notificationManager.Show(new NotificationContent
            {
                Title = "Error while saving",
                Message = ex.Message,
                Type = NotificationType.Error
            });
        }
    }
}
