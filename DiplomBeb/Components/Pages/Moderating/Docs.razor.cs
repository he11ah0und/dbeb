using DiplomBeb.Services.Client;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages.Moderating;

public partial class Docs : ComponentBase
{
    [Inject]
    private AccountService AccountService { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    protected override async Task OnInitializedAsync()
    {
        AccountService.StateChanged.Subscribe("docs", OnLoaded);
        ModerationService.OnRequestDocument.Subscribe("docs", OnDocument);
    }
    private void OnLoaded()
    {
        if (ulong.TryParse(AccountService.Account?.Get("apirole_id"), out var val)
            && val >= 5)
            return;
        Navigation.NavigateTo($"/error/403");
    }

    [Inject]
    private NotificationService NotificationService { get; set; }
    [Inject]
    private ModerationService ModerationService { get; set; }
    private bool Started { get; set; }
    private async Task Begin()
    {
        Started = true;

        if (!await ModerationService.RequestDocumentForModeration())
            NotificationService.AddNotification("Ошибка получения документа!");
        
        StateHasChanged();
    }

    private async Task OnDocument()
    {
        StateHasChanged();
    }

    private async Task AcceptDocument(bool state)
    {
        
    }
}