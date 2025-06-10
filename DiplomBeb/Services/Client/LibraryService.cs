using DiplomBeb.Database.Model;
using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client;

public class LibraryService
{
    public ListView ListView { get; private set; }
    public LibraryService(BackendHttpClient http)
    {
        ListView = new ListView(http)
        {
            OnGet = RequestLibs,
            OnTotal = RequestLibsTotal,
            GetHeader = () => "Библиотека полезных ссылок"
        };
        ListView.DataResolver.Props.AddRange(
            ["id", "name", "link"]);
    }
    
    private Task<HttpResponseState> RequestLibs(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.NativeRequest($"Library/{skip}/{take}");
    }

    private Task<HttpResponseState> RequestLibsTotal(BackendHttpClient http)
    {
        return http.NativeRequest("Library/total");
    }
}