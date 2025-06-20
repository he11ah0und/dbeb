using System.Diagnostics;
using DiplomBeb.Services.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DiplomBeb.Components.Views;

public partial class NotificationsController : ComponentBase
{
    [Inject]
    public NotificationService NotificationService { get; set; }
    [Inject]
    public IJSRuntime _JsRuntime { get; set; }
    
    private ulong MaxVisibleTime { get; set; }
    private string CurrentValue { get; set; }
    private Timer? _endTimer;
    protected override async Task OnInitializedAsync()
    {
        MaxVisibleTime = 3000;
        NotificationService.OnNewNotification = OnNewNotification;
    }

    private async Task OnNewNotification()
    {
        while (true)
        {
            if (!NotificationService.Notifications.TryDequeue(out var not))
                return;
            CurrentValue = not;
            StateHasChanged();
            if (_endTimer is not null)
                await _endTimer.DisposeAsync();
            _endTimer = new Timer(async _ =>
            {
                await InvokeAsync(async () =>
                {
                    // fade in
                    //await _JsRuntime.InvokeVoidAsync("inAnimation");
                    await Task.Delay(600); // 0.6

                    // wait
                    await Task.Delay(TimeSpan.FromMilliseconds(MaxVisibleTime));

                    // fade out
                    //await _JsRuntime.InvokeVoidAsync("outAnimation");
                    await Task.Delay(600); // 0.6
                    CurrentValue = "";
                    StateHasChanged();
                });
            }, null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);
        }
    }
}