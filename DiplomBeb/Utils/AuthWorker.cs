using System.Security.Cryptography;
using DiplomBeb.Filters;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DiplomBeb.Utils;

public class AuthWorker
{
    private BackendHttpClient BHttp;

    public async ValueTask<ProtectedBrowserStorageResult<string>?> GetAuthToken()
    {
        try
        {
            return await BHttp.ProtectedLocalStore.GetAsync<string>("auth");
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public AuthWorker(BackendHttpClient client)
    {
        BHttp = client;
    }
    public async Task<bool> TrySignIn(
        SignInData signInData)
    {
        // hashing password
        using (var mySha256 = SHA256.Create())
        {
            var passStream = new MemoryStream(signInData.Password);
            signInData.Password = await mySha256.ComputeHashAsync(passStream);
        }

        var res = BHttp.NativeRequest("http://localhost:5062/api/Auth/signin",
            JsonContent.Create(signInData),
            HttpMethod.Post
            );
        if (!res.IsOk)
            return false;

        if (!res.ResultAsString(out var result))
            return false;
        
        await BHttp.ProtectedLocalStore.SetAsync("auth", result);
        return true;
    }
    
    public async Task<bool> TrySignUp(
        SignUpData signUpData)
    {
        // hashing password
        using (SHA256 mySHA256 = SHA256.Create())
        {
            var passstream = new MemoryStream(signUpData.Password);
            signUpData.Password = await mySHA256.ComputeHashAsync(passstream);
        }

        var res = BHttp.NativeRequest("Auth/signup",
            JsonContent.Create(signUpData),
            HttpMethod.Post);
        if (!res.IsOk || !res.ResultAsBool(out bool bOk))
            return false;

        return bOk;
    }

    public async void LocalLogOut()
    {
        await BHttp.ProtectedLocalStore.DeleteAsync("auth");
    }
    
    public async Task<bool> LogOut()
    {
        var res = await BHttp.Request("Auth/logout");
        if (!res.IsOk)
            return false;

        LocalLogOut();

        return true;
    }
}