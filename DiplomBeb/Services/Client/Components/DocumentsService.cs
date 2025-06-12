using DiplomBeb.Components.Pages;
using DiplomBeb.Components.Views;
using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client.Components;

public class DocumentsService
{
    public ListView ListView { get; private set; }
    
    private BackendHttpClient _http;
    public DocumentsService(BackendHttpClient http)
    {
        _http = http;
        ListView = new ListView(http, typeof(DivContent))
        {
            ElementsOnPage = 6,
            OnGet = RequestDocuments,
            OnTotal = RequestDocumentsTotal,
            GetHeader = () => "Документы",
        };
        ListView.DataResolver.Props.AddRange(
            ["document"]);
    }
    
    private Task<HttpResponseState> RequestDocuments(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.Request($"Documents/{skip}/{take}");
    }

    private Task<HttpResponseState> RequestDocumentsTotal(BackendHttpClient http)
    {
        return http.Request($"Documents/total/");
    }
}