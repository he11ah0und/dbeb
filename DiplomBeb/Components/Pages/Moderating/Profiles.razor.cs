using DiplomBeb.Services.Client;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages.Moderating;

public partial class Profiles : ComponentBase
{
    [Inject]
    private ModerationService ModerationService { get; set; }
    
    
    protected override async Task OnInitializedAsync()
    {
        
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await ModerationService.UsersListView.First(1);
    }
}