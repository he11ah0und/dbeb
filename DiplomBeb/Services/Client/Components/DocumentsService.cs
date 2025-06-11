using DiplomBeb.Components.Pages;
using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client.Components;

public class DocumentsService
{
    public ListElement[] Documents { get; private set; }
    public Callbacker<Action> OnDocumentsUpdated { get; private set; }
    
    private BackendHttpClient _http;
    public DocumentsService(BackendHttpClient http)
    {
        _http = http;
        Documents = [];
        OnDocumentsUpdated = new Callbacker<Action>();
    }
    
    public async Task GetDocs()
    {
        var res = await _http.Request($"Users/docs");
        if (!res.IsOk || !res.ResultAsJson<ListElement[]>(out var les))
            return;

        Documents = les;
        OnDocumentsUpdated.InvokeAll();
    }
}