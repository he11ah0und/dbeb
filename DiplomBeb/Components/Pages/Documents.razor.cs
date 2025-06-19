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

    private void ClearUploader()
    {
        ImgData = "images/img-doc.svg";
        DocumentName = "";
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

    private Stream? OpenReadFileStream()
    {
        try
        {
            return SelectedFile.OpenReadStream(MaxFileSize);
        }
        catch
        {
            return null;
        }
    }

    private string ImgData = "images/img-doc.svg";

    private long MaxFileSize { get; set; } = 10 * 1024 * 1024;
    private async Task Callback(InputFileChangeEventArgs obj)
    {
        var file = obj.File;
        if (file.ContentType != "image/png"
            || file.Size > MaxFileSize)
            return;
        SelectedFile = file;
        
        var stream = OpenReadFileStream();
        if (stream is null)
        {
            ImgData = "images/img-doc.svg";
            return;
        }
        var buffer = new byte[stream.Length];
        await stream.ReadExactlyAsync(buffer);
        ImgData = $"data:image;base64,{Convert.ToBase64String(buffer)}";
    }

    private string DocumentName;
    private async Task UploadSelectedDocument()
    {
        var stream = OpenReadFileStream();
        
        using var fileStreamContent = new StreamContent(stream);
        fileStreamContent.Headers.ContentType =
            new MediaTypeHeaderValue(SelectedFile.ContentType);

        using var client = new HttpClient();
        using var formData = new MultipartFormDataContent();
        
        formData.Add(fileStreamContent, "File", SelectedFile.Name);
        formData.Add(new StringContent(DocumentName), "Name");
        
        var res = await _http.Request(
            "Documents/up",
            formData,
            HttpMethod.Post);
        if (!res.IsOk)
            return;
        
        ClearUploader();
        HideUploader();
        await AccountService.DocumentsService.DocumentsListView.Update();
    }
    
    // ERROR HANDLING
    [Inject]
    private NavigationManager Navigation { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AccountService.StateChanged.Subscribe("docs", OnLoaded);
        DocumentName = "";
    }
    private void OnLoaded()
    {
        if (ulong.TryParse(AccountService.Account?.Get("apirole_id"), out var val)
            && val >= 5)
            return;
        Navigation.NavigateTo($"/error/403");
    }
}