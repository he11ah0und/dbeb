using DiplomBeb.Services.Client;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages.Moderating;

public partial class Profiles : ComponentBase
{
    [Inject]
    private ModerationService ModerationService { get; set; }
    
    [Inject]
    private BackendHttpClient Http { get; set; }
    
    
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
        await ModerationService.ListView.First(1);
    }
}