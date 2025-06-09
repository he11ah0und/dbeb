using DiplomBeb.Services.Client;
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
    private SyncRenderService SyncRenderService { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        SyncRenderService.Max = 3;
        SyncRenderService.OnComplete = OnListUpdated;
        
        HomeService.TopUsersListView.OnListUpdated.
            Subscribe("home", () => SyncRenderService.Register(0));
        HomeService.LibraryRandomStateChanged
            .Subscribe("home", () => SyncRenderService.Register(1));
        AccountService.StateChanged.
            Subscribe("home", OnListUpdated);
        
        
        await HomeService.RequestRandomLibs();
    }
    
    private void OnListUpdated()
    {
        StateHasChanged();
    }

    private bool Ok;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) 
            return;
        await HomeService.TopUsersListView.First(1);
    }
}