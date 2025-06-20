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
    
    protected override async Task OnInitializedAsync()
    {
        
    }
    
    private void OnListUpdated()
    {
        StateHasChanged();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) 
            return;
        
        HomeService.LibraryRandomStateChanged
            .Subscribe("home", OnListUpdated);
        
        AccountService.StateChanged.
            Subscribe("home", OnListUpdated);
        
        if (!await HomeService.TopUsersListView.First(1))
            NotificationService.AddNotification("Ошибка получения топа.");
        
        if (!await HomeService.RequestRandomLibs())
            NotificationService.AddNotification("Ошибка получения информации из библиотеки.");
    }
}