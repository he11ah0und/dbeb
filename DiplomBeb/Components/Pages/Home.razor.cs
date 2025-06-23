using System.Timers;
using DiplomBeb.Services.Client;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages;

public partial class Home
{
    [Inject] 
    private AccountService AccountService { get; set; }
    
    [Inject]
    private HomeService HomeService { get; set; }
    [Inject]
    private NotificationService NotificationService { get; set; }
    
    private System.Timers.Timer _libsTimer { get; set; }
    protected override async Task OnInitializedAsync()
    {
        _libsTimer = new System.Timers.Timer(15000);
        _libsTimer.Elapsed += LibsTimerOnElapsed;
        _libsTimer.Start();
    }

    private void LibsTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        InvokeAsync(async () =>
        {
            if (!await HomeService.RequestRandomLibs())
                NotificationService.AddNotification("Ошибка получения информации из библиотеки.");
        });
    }
    
    private void OnListUpdated()
    {
        StateHasChanged();
    }

    private async Task OnAccountState()
    {
        if (!await HomeService.TopUsersListView.First(1))
            NotificationService.AddNotification("Ошибка получения топа.");
        OnListUpdated();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) 
            return;
        
        HomeService.LibraryRandomStateChanged
            .Subscribe("home", OnListUpdated);

        AccountService.StateChanged.Subscribe("home", () => OnAccountState());
    }
}
