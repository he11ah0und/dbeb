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
    
    protected override async Task OnInitializedAsync()
    {
        
    }
    
    private void OnListUpdated()
    {
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        
        await UserService.Place.SelectPlace((ulong)PlaceId.Value);
        UserService.Place.ListView.OnListUpdated.Subscribe("place", OnListUpdated);
        
        await UserService.Place.ListView.First(1);
        
    }
}