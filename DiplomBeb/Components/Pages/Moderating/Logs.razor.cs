using DiplomBeb.Services.Client;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages.Moderating;

public partial class Logs : ComponentBase
{
    [Inject]
    private ModerationService ModerationService { get; set; }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await ModerationService.LogsListView.First(1);
    }
}