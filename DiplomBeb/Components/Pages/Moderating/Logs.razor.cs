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
        ModerationService.OnSelectedLog.Subscribe("logs", OnSelected);
        await ModerationService.LogsListView.First(1);
    }

    private void OnSelected()
    {
        StateHasChanged();
    }
}