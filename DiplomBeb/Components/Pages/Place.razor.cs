using DiplomBeb.Services;
using DiplomBeb.Services.Client;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages;

public partial class Place
{
    [Parameter]
    public long? PlaceId { get; set; }
    
    [Inject]
    private UserService UserService { get; set; }
    
    [Inject]
    private AccountService AccountService { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    [Inject]
    private NotificationService NotificationService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AccountService.StateChanged.Subscribe("404", OnLoaded);
        
    }
    private void OnLoaded()
    {
        if (ulong.TryParse(AccountService.Account?.Get("apirole_id"), out var val)
            && val >= 1)
            return;
        Navigation.NavigateTo($"/error/403");
    }
    
    private void OnListUpdated()
    {
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        
        if (!await UserService.Place.SelectPlace((ulong)PlaceId.Value))
            NotificationService.AddNotification("Ошибка получения информации о месте.");
            
        UserService.Place.ListView.OnListUpdated.Subscribe("place", OnListUpdated);
        
        if (!await UserService.Place.ListView.First(1))
            NotificationService.AddNotification("Ошибка загрузки информации о месте.");
    }
}