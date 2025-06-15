using DiplomBeb.Components.Views;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;
using dshared.Utils;

namespace DiplomBeb.Services.Client.Components;

public class PlaceComponent
{
    public ListView ListView { get; private set; }
    public ListElement CurrentPlace { get; private set; }
    public bool CurrentInPlace { get; private set; }
    public ulong CurrentPlaceId { get; private set; }

    internal AccountService _accountService { get; private set; }
    private BackendHttpClient _http;
    public PlaceComponent(BackendHttpClient http,
        AccountService accountService)
    {
        _http = http;
        _accountService = accountService;
        ListView = new ListView(_http, typeof(TableContent), 1, false)
        {
            OnGet = RequestUsers,
            OnTotal = RequestUsersTotal,
            GetHeader = () => $"Участники {CurrentPlace?.Get("name")}"
        };
        ListView.DataResolver.Props.AddRange(
            ["id", "avatar", "name", "middlename", "surname", "score", "webrole_name"]);
    }
    
    private Task<HttpResponseState> RequestUsers(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.Request($"Place/users/{CurrentPlaceId}/{skip}/{take}");
    }

    private Task<HttpResponseState> RequestUsersTotal(BackendHttpClient http)
    {
        return http.Request($"Place/total/{CurrentPlaceId}");
    }

    public async Task SelectPlace(ulong id)
    {
        CurrentPlaceId = id;
        // place info
        var res = await _http.Request($"Place/{CurrentPlaceId}");
        if (!res.IsOk)
            return;
        
        if (res.ResultAsJson<ListElement>(out var result))
            CurrentPlace = result;
    }
}