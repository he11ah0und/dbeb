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
    private BackendHttpClient Http { get; set; }
    [Inject]
    private UserService UserService { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        UserService.Place.PlaceLoaded.Subscribe("library", OnPlaceLoaded);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await Update();
    }

    private async Task Update()
    {
        await UserService.Place.RequestPlace(Http, PlaceId.Value);
    }

    private void OnPlaceLoaded()
    {
        StateHasChanged();
    }
}