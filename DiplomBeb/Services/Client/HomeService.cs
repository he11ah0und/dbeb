using DiplomBeb.Components.Views;
using DiplomBeb.Database.Model;
using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client;

public class HomeService
{
    public Callbacker<Action> LibraryRandomStateChanged { get; private set; }
    public Library[] RandomLibs { get; private set; }
    
    public ListView TopUsersListView { get; private set; }
    private BackendHttpClient _http { get; set; }
    public HomeService(BackendHttpClient http)
    {
        _http = http;
        TopUsersListView = new ListView(http, typeof(TableContent),1, false)
        {
            ElementsOnPage = 100,
            OnTotal = RequestUsersTotal,
            OnGet = RequestUsers,
            GetHeader = () => "Общая информация",
            HideRightFooterButtons = true,
        };
        TopUsersListView.DataResolver.Props.AddRange(
            ["id", "avatar", "name", "middlename", "surname", "score", "webrole_name"]);
        LibraryRandomStateChanged = new Callbacker<Action>();
        RandomLibs = [];
    }
    
    private Task<HttpResponseState> RequestUsers(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.NativeRequest($"Users/top/{skip}/{take}");
    }

    private Task<HttpResponseState> RequestUsersTotal(BackendHttpClient http)
    {
        return http.NativeRequest("Users/total");
    }
    
    public async Task RequestRandomLibs()
    {
        var res = await _http.NativeRequest("Library/random/4");
        if (!res.IsOk)
            return;
        if (res.ResultAsJson<Library[]>(out var result))
            RandomLibs = result;
        LibraryRandomStateChanged.InvokeAll();
    }
}