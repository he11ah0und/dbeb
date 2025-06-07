using System.Net.Http.Headers;
using DiplomBeb.Database.Model;
using DiplomBeb.Services;
using DiplomBeb.Systems.Auth;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using DiplomBeb.Services;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Systems.Accounts;

public class AccountsEngineClient
{
    public static async Task<User> GetPersonal(
        BackendHttpClient http)
    {
        var res = await http.Request("Users/personal");
        if (!res.IsOk)
        {
            return null;
        }

        if (!res.ResultAsJson<User>(out var result))
            return null;
        
        return result;
    }
    public static async Task<UserRank> GetRank(ulong id,
        BackendHttpClient http)
    {
        var res = await http.Request($"Users/top/{id}");
        if (!res.IsOk)
        {
            return null;
        }
        
        if (!res.ResultAsJson<UserRank>(out var result))
            return null;
        
        return result;
    }
}