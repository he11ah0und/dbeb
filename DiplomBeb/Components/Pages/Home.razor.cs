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
    
    protected override async Task OnInitializedAsync()
    {
        HomeService.TopUsersListView.OnListUpdated.
            Subscribe("home", OnListUpdated);
        HomeService.LibraryRandomStateChanged
            .Subscribe("home", OnListUpdated);
        
        AccountService.StateChanged.
            Subscribe("home", OnListUpdated);
        AccountService.MyListView.OnListUpdated
            .Subscribe("home", OnListUpdated);
        
        await HomeService.RequestRandomLibs();
    }
    
    private void OnListUpdated()
    {
        StateHasChanged();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) 
            return;
        await HomeService.TopUsersListView.First(1);
    }
}