using DiplomBeb.Database.Model;
using DiplomBeb.Utils;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client.Components;

public class UsersComponent
{
    public Callbacker UsersTopStateChanged { get; private set; }
    
    public ulong TotalUsers { get; private set; }
    public User[] UsersTop { get; private set; }

    public UsersComponent()
    {
        UsersTopStateChanged = new Callbacker();
        TotalUsers = 0;
        UsersTop = [];
    }
    
    public void RequestUsersTop(BackendHttpClient http)
    {
        var res = http.NativeRequest("Users/top");
        if (!res.IsOk)
        {
            return;
        }
        if (res.ResultAsJson<User[]>(out var result))
            UsersTop = result;

        res = http.NativeRequest("Users/total");
        if (!res.IsOk)
        {
            return;
        }
        
        if (res.ResultAsULong(out var total))
            TotalUsers = total;

        UsersTopStateChanged.InvokeAll();
    }
}