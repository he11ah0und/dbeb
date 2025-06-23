using System.Net.Http.Headers;
using DiplomBeb.Services.Client;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DiplomBeb.Components.Pages;

public partial class Documents : ComponentBase
{
    [Inject] 
    private AccountService AccountService { get; set; }
    [Inject] 
    private DocumentsService DocumentsService { get; set; }
    [Inject]
    private NotificationService NotificationService { get; set; }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        if (!await DocumentsService.DocumentsListView.First(1))
            NotificationService.AddNotification("Ошибка загрузки документов!");
        if (!await DocumentsService.FiltersDocumentsListView.First(1))
            NotificationService.AddNotification("Ошибка загрузки фильтров документов!");
        
        DocumentsService.DocumentUploader.OnUploaded.Subscribe("Documents", OnUploadDocument);
    }
    
    private async Task OnUploadDocument()
    {
        DocumentsService.DocumentUploader.DocumentName = "";
        HideUploader();
        await DocumentsService.DocumentsListView.Update();
    }

    private bool InUpload { get; set; }
    private void HideUploader()
    {
        InUpload = false;
        StateHasChanged();
    }
    private void ShowUploader()
    {
        InUpload = true;
        StateHasChanged();
    }

    private Task DownloadSelectedDocuments()
    {
        return DocumentsService.DownloadSelected();
    }
    private Task RemoveSelectedDocuments()
    {
        return DocumentsService.RemoveSelected();
    }
    
    
    // ERROR HANDLING
    [Inject]
    private NavigationManager Navigation { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AccountService.StateChanged.Subscribe("docs", OnLoaded);
    }
    private void OnLoaded()
    {
        if (ulong.TryParse(AccountService.Account?.Get("apirole_id"), out var val)
            && val >= 5)
            return;
        Navigation.NavigateTo($"/error/403");
    }
}