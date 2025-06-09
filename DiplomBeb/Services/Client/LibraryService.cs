using DiplomBeb.Database.Model;
using DiplomBeb.Utils;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client;

public class LibraryService
{
    public Callbacker LibraryRandomStateChanged { get; private set; }
    public Library[] RandomLibs { get; private set; }
    public ListView<Library> LibraryListView { get; private set; }

    private BackendHttpClient _http { get; set; }
    public LibraryService(BackendHttpClient http)
    {
        _http = http;
        LibraryRandomStateChanged = new Callbacker();
        RandomLibs = [];
        LibraryListView = new ListView<Library>(_http)
        {
            OnGet = RequestLibs,
            OnTotal = RequestLibsTotal
        };
    }
    
    private HttpResponseState RequestLibs(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.NativeRequest($"Library/{skip}/{take}");
    }

    private HttpResponseState RequestLibsTotal(BackendHttpClient http)
    {
        return http.NativeRequest("Library/total");
    }
    
    public void RequestRandomLibs(BackendHttpClient http)
    {
        var res = http.NativeRequest("Library/random/4");
        if (!res.IsOk)
        {
            return;
        }
        if (res.ResultAsJson<Library[]>(out var result))
            RandomLibs = result;
        LibraryRandomStateChanged.InvokeAll();
    }
}