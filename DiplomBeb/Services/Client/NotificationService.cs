namespace DiplomBeb.Services.Client;

public class NotificationService
{
    public Queue<string> Notifications { get; private set; }
    public Action OnNewNotification { get; set; }
    
    public NotificationService()
    {
        Notifications = [];
    }

    public void AddNotification(string? text)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
            return;
        Notifications.Enqueue(text);
        OnNewNotification?.Invoke();
    }
}