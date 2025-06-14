using DiplomBeb.Components.Pages;
using DiplomBeb.Components.Views;
using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client.Components;

public class DocumentsService
{
    public ListView ProfileListView { get; private set; }
    public ListView DocumentsListView { get; private set; }
    public ListView FiltersDocumentsListView { get; private set; }
    public List<string> Filters { get; private set; }
    public List<string> SelectedDocuments { get; private set; }
    
    
    private BackendHttpClient _http;
    public DocumentsService(BackendHttpClient http)
    {
        _http = http;
        Filters = [];
        SelectedDocuments = [];
        
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
        DocumentsListView.OnElementCallback.Subscribe("libraryService", OnDocumentCallback);
        
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
        FiltersDocumentsListView.OnElementCallback.Subscribe("libraryService", OnFiltersCallback);
    }
    
    public void SetFilter(string filter)
    {
        if (Filters.Contains(filter)) return;
        Filters.Add(filter);
    }
    public void DelFilter(string filter) 
        => Filters.Remove(filter);
    public void ClearFilters()
        => Filters.Clear();
    
    private void OnFiltersCallback(string propName,
        ulong index,
        ListElement listElement,
        ObjectContainer args)
    {
        var state = (bool)args.Get("value");
        if (state)
            SetFilter(listElement.Get("name"));
        else
            DelFilter(listElement.Get("name"));

        DocumentsListView.Update();
    }
    
    
    public void SetDocument(string filter)
    {
        if (Filters.Contains(filter)) return;
        Filters.Add(filter);
    }
    public void DelDocument(string filter) 
        => Filters.Remove(filter);
    public void ClearDocuments()
        => Filters.Clear();
    private void OnDocumentCallback(string propName,
        ulong index,
        ListElement listElement,
        ObjectContainer args)
    {
        var state = (bool)args.Get("value");
        if (state)
            SetDocument(listElement.Get("id"));
        else
            DelDocument(listElement.Get("id"));
    }
    
    private Task<HttpResponseState> RequestDocuments(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.Request($"Documents/{skip}/{take}");
    }

    private Task<HttpResponseState> RequestDocumentsTotal(BackendHttpClient http)
    {
        return http.Request($"Documents/total/");
    }
    
    // filters
    private Task<HttpResponseState> RequestFilters(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.Request($"Library/filters/{skip}/{take}");
    }
    
    private Task<HttpResponseState> RequestFiltersTotal(BackendHttpClient http)
    {
        return http.Request("Library/total_filters");
    }
}