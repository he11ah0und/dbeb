using DiplomBeb.Database.Model;
using DiplomBeb.Utils;
using DiplomBeb.Database.Model;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client.Components;

public class PlaceComponent
{
    public Place CurrentPlace { get; private set; }
    public ulong TotalUsers { get; private set; }
    public bool CurrentInPlace { get; private set; }
    public ulong CurrentPlaceId { get; private set; }
    public User[] UsersInPlace { get; private set; }
    public Callbacker PlaceLoaded { get; private set; }

    internal AccountService _accountService { get; private set; }
    public PlaceComponent(AccountService accountService)
    {
        _accountService = accountService;
        PlaceLoaded = new Callbacker();
    }
    
    public async Task RequestPlace(BackendHttpClient http, long id)
    {
        // place info
        var res = await http.Request($"Place/{id}");
        if (!res.IsOk)
            return;
        
        if (res.ResultAsJson<Place>(out var result))
            CurrentPlace = result;
       
        // count of users in place
        res = await http.Request($"Place/count/{id}");
        if (!res.IsOk)
            return;
        
        if (res.ResultAsULong(out var totalUsers))
            TotalUsers = totalUsers;
        
        // users in place
        res = await http.Request($"Place/users/{id}");
        if (!res.IsOk)
            return;
        
        if (res.ResultAsJson<User[]>(out var Users))
            UsersInPlace = Users;
        
        res = await http.Request($"Place/{id}/{_accountService.Account.Id}");
        if (res.IsOk && res.ResultAsULong(out var userPos))
        {
            CurrentInPlace = userPos > 0;
            CurrentPlaceId = userPos;
        }
        
        PlaceLoaded.InvokeAll();
    }
}