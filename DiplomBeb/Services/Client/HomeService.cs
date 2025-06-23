using DiplomBeb.Components.Views;
using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;
using dshared.Utils;

namespace DiplomBeb.Services.Client;

public class HomeService
{
    public Callbacker<Action> LibraryRandomStateChanged { get; private set; }
    public ListElement[] RandomLibs { get; private set; }
    
    public ListView TopUsersListView { get; private set; }
    private BackendHttpClient _http { get; set; }
    private AccountService _accountService { get; set; }
    
    public HomeService(BackendHttpClient http,  AccountService accountService)
    {
        _http = http;
        _accountService = accountService;
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
        var url = $"Users/top/{skip}/{take}";
        if (_accountService.IsLogged)
            return http.Request(url);
        else
            return http.NativeRequest(url);
    }

    private Task<HttpResponseState> RequestUsersTotal(BackendHttpClient http)
    {
        return http.NativeRequest("Users/total");
    }
    
    public async Task<bool> RequestRandomLibs()
    {
        var res = await _http.NativeRequest("Library/random/4");
        if (!res.IsOk)
            return false;
        if (!res.ResultAsJson<ListElement[]>(out var result))
            return false;
        RandomLibs = result;
        LibraryRandomStateChanged.InvokeAll();
        return true;
    }
}