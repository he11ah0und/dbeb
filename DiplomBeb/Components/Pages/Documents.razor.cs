using DiplomBeb.Services.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace DiplomBeb.Components.Pages;

public partial class Documents : ComponentBase
{
    [Inject] 
    private AccountService AccountService { get; set; }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await AccountService.DocumentsService.DocumentsListView.First(1);
        await AccountService.DocumentsService.FiltersDocumentsListView.First(1);
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
        return AccountService.DocumentsService.DownloadSelected();
    }
    private Task RemoveSelectedDocuments()
    {
        return AccountService.DocumentsService.RemoveSelected();
    }
    private byte[] SelectedFile { get; set; }
    [JSInvokable]
    public void ReceiveFileBytes(byte[] bytes)
    {
        SelectedFile = bytes;
        StateHasChanged();
    }
    private async Task UploadSelectedDocuments()
    {
        
    }
    
    // ERROR HANDLING
    [Inject]
    private NavigationManager Navigation { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AccountService.StateChanged.Subscribe("404", OnLoaded);
    }
    private void OnLoaded()
    {
        if (ulong.TryParse(AccountService.Account?.Get("apirole_id"), out var val)
            && val >= 5)
            return;
        Navigation.NavigateTo($"/error/403");
    }
}