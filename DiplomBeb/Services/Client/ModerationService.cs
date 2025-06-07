using DiplomBeb.Database.Model;
using DiplomBeb.Utils;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client;

public class ModerationService
{
    public Callbacker UsersStateChanged;
    
    public User[] Users { get; private set; }
    public async Task RequestUsers(BackendHttpClient http, ulong page)
    {
        var res = await http.Request($"Users/get30?page={page}");
        if (!res.IsOk)
            return;

        if (res.ResultAsJson<User[]>(out var result))
            Users = result;
        UsersStateChanged.InvokeAll();
    }
}