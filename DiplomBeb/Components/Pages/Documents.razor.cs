using DiplomBeb.Services.Client;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace DiplomBeb.Components.Pages;

public partial class Documents : ComponentBase
{
    [Inject] 
    private AccountService AccountService { get; set; }
    [Inject]
    private BackendHttpClient _http { get; set; }
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
    private IBrowserFile SelectedFile { get; set; }
    private long MaxFileSize { get; set; } = 10 * 1024 * 1024;
    private void Callback(InputFileChangeEventArgs obj)
    {
        var file = obj.File;
        if (file.ContentType != "image/png"
            || file.Size > MaxFileSize)
            return;
        SelectedFile = file;
    }
    private async Task UploadSelectedDocument()
    {
        var stream = SelectedFile.OpenReadStream(MaxFileSize);
        
        using var fileStreamContent = new StreamContent(stream);

        using var client = new HttpClient();
        using var formData = new MultipartFormDataContent();
        
        formData.Add(fileStreamContent, "File", SelectedFile.Name);
        
        var res = await _http.Request(
            "Documents/up",
            formData,
            HttpMethod.Post);
        if (!res.IsOk)
            return;
        
        
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