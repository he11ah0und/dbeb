using DiplomBeb.Components.Views;
using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;
using Microsoft.JSInterop;

namespace DiplomBeb.Services.Client;

public class DocumentsService
{
    public ListView ProfileListView { get; private set; }
    public ListView DocumentsListView { get; private set; }
    public ListView FiltersDocumentsListView { get; private set; }
    public List<string> Filters { get; private set; }
    public Selector DocumentsSelector { get; private set; }
    public Selector DocumentTypesSelector { get; private set; }
    public Uploader DocumentUploader { get; private set; }
    
    private BackendHttpClient _http;
    private IJSRuntime _jsRuntime;
    public DocumentsService(BackendHttpClient http, NotificationService nots, IJSRuntime jsRuntime)
    {
        _http = http;
        _jsRuntime = jsRuntime;
        Filters = [];
        
        ProfileListView = new ListView(http, typeof(DivContent))
        {
            ElementsOnPage = 6,
            OnGet = RequestDocuments,
            OnTotal = RequestDocumentsTotal,
            GetHeader = () => "Документы",
        };
        ProfileListView.DataResolver.Props.AddRange(
            ["document"]);
        
        DocumentsListView = new ListView(http, typeof(DivContent))
        {
            ElementsOnPage = 6,
            OnGet = RequestDocuments,
            OnTotal = RequestDocumentsTotal,
            GetHeader = () => "Документы",
            HideHeading = true,
        };
        DocumentsListView.DataResolver.Props.AddRange(
            ["document"]);
        
        FiltersDocumentsListView = new ListView(http, typeof(TableContent))
        {
            OnGet = RequestFilters,
            OnTotal = RequestFiltersTotal,
            HideFooter = true,
            HideHeading = true,
            HideTableHeader = true,
            SlimTableData = true,
        };
        FiltersDocumentsListView.DataResolver.Props.AddRange(
            ["check"]);
        
        DocumentsSelector = new Selector("1", DocumentsListView, DocumentsListView, "id")
        {
            UpdateDestination = false
        };
        DocumentTypesSelector = new Selector("2", FiltersDocumentsListView, DocumentsListView, "name");

        DocumentUploader = new Uploader(http, nots, "images/img-doc.svg")
        {
            OnUpload = UploadDocument,
        };
    }
    
    private Task<HttpResponseState> UploadDocument(MultipartFormDataContent formData)
        => _http.Request("Documents/up", formData, HttpMethod.Post);
    
    private Task<HttpResponseState> RequestDocuments(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.Request($"Documents/{skip}/{take}", JsonContent.Create(DocumentTypesSelector.Values.ToArray()), HttpMethod.Post);
    }

    private Task<HttpResponseState> RequestDocumentsTotal(BackendHttpClient http)
    {
        return http.Request($"Documents/total/", JsonContent.Create(DocumentTypesSelector.Values.ToArray()), HttpMethod.Post);
    }
    
    // filters
    private Task<HttpResponseState> RequestFilters(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.Request($"Documents/types/{skip}/{take}");
    }
    
    private Task<HttpResponseState> RequestFiltersTotal(BackendHttpClient http)
    {
        return http.Request("Documents/total_types");
    }

    public async Task DownloadSelected()
    {
        var docs = DocumentsListView.Data
            .Where(d => DocumentsSelector.Values.Contains(d.Get("id")))
            .ToArray();
        foreach (var d in docs)
        {
            var res = await _http.Request($"Documents/down/{d.Get("id")}");
            if (!res.IsOk) continue;
            var s = await res.Value.Content.ReadAsStreamAsync();
            var ms = new MemoryStream();
            await s.CopyToAsync(ms);
            var data = ms.ToArray();
            await _jsRuntime.InvokeVoidAsync("saveImageWithDialog", $"{d.Get("document")}.png", data);
        }
    }
    
    public async Task RemoveSelected()
    {
        var res = await _http.Request("Documents/remove/",
            JsonContent.Create(DocumentsSelector.Values.ToArray()),
            HttpMethod.Post);
        if (!res.IsOk) return;

        await DocumentsListView.Update();
    }
}