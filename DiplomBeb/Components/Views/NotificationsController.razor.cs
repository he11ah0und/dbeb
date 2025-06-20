using System.Diagnostics;
using DiplomBeb.Services.Client;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Views;

public partial class NotificationsController : ComponentBase
{
    [Inject]
    public NotificationService NotificationService { get; set; }
    
    private List<(Guid, string)> ActiveNotifications { get; set; }
    private ulong MaxVisible { get; set; }
    private ulong MaxVisibleTime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ActiveNotifications = [];
        MaxVisible = 3;
        MaxVisibleTime = 3000;
        NotificationService.OnNewNotification = OnNewNotification;
    }
    
    private void OnNewNotification()
    {
        if (!NotificationService.Notifications.TryDequeue(out var not))
            return;

        if ((ulong)ActiveNotifications.Count >= MaxVisible)
            return;
        var guid = Guid.NewGuid();
        Debug.WriteLine($"Add {guid} and {not}");
        ActiveNotifications.Add((guid, not));
        StateHasChanged();
    }

    private void OnEnd(Guid guid)
    {
        Debug.WriteLine($"Del {guid} and {ActiveNotifications.SingleOrDefault(an => an.Item1 == guid).Item2}");
        var not = ActiveNotifications.SingleOrDefault(an => an.Item1 == guid, default);
        if (not == default)
            return;
        ActiveNotifications.Remove(not);
        StateHasChanged();

        OnNewNotification();
    }
}