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
        AccountService.StateChangedNoCall.Subscribe("docs", OnLoaded);
        ModerationService.OnRequestDocument.Subscribe("docs", OnDocument);
        StartText = "Приступить модерировать?";

        if (!await ModerationService.RequestDocumentTypes())
        {
            NotificationService.AddNotification("Ошибка получения информации о документах!");
            return;
        }
        SelectedDocumentTypeId = ulong.Parse(ModerationService.DocumentTypes.First().Get("id", "0")); ;
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
    private string StartText = "";
    private async Task Begin()
    {
        if (!await ModerationService.RequestDocumentForModeration())
        {
            NotificationService.AddNotification("Ошибка получения документа!");
            return;
        }

        Started = true;
        StateHasChanged();
    }

    private ulong SelectedDocumentTypeId { get; set; }
    private void OnDocumentTypeChange(ChangeEventArgs e)
    {
        SelectedDocumentTypeId = ulong.Parse(ModerationService.DocumentTypes
            .SingleOrDefault(dt => dt.Get("check") == (string)e.Value).Get("id", "0"));
    }

    private async Task OnDocument()
    {
        DocumentName = ModerationService.ProcessDocument.Get("documentName", "???");
        StateHasChanged();
    }

    private string DocumentName { get; set; } = "???";
    private async Task AcceptDocument(bool state)
    {
        if (!await ModerationService.AcceptDocumentFromModeration(SelectedDocumentTypeId, state))
        {
            NotificationService.AddNotification("Ошибка отправки состояния документа!");
            return;
        }
        StartText = "Продолжить модерировать?";
        Started = false;
        SelectedDocumentTypeId = ulong.Parse(ModerationService.DocumentTypes.First().Get("id", "0"));
        StateHasChanged();
    }
}