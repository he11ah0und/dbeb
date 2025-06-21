using System.Security.Cryptography;
using System.Text;
using DiplomBeb.Utils.Networking;
using dshared.Utils;
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
        ListElement signInData, bool encryptPass = true)
    {
        // hashing password
        if (encryptPass)
        {
            using (var mySha256 = SHA256.Create())
            {
                var passStream = new MemoryStream(
                    Encoding.Default.GetBytes(signInData.Get("password")));
                signInData.Set("password", Convert.ToBase64String(await mySha256.ComputeHashAsync(passStream)));
            }
        }

        var res = await BHttp.NativeRequest("Auth/signin",
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
        ListElement signUpData)
    {
        // hashing password
        using (var mySHA256 = SHA256.Create())
        {
            var passStream = new MemoryStream(
                Encoding.Default.GetBytes(signUpData.Get("password")));
            signUpData.Set("password", Convert.ToBase64String(await mySHA256.ComputeHashAsync(passStream)));
        }

        var res = await BHttp.NativeRequest("Auth/signup",
            JsonContent.Create(signUpData),
            HttpMethod.Post);
        if (!res.IsOk || !res.ResultAsBool(out bool bOk))
            return false;

        return bOk;
    }

    public async Task LocalLogOut()
    {
        await BHttp.ProtectedLocalStore.DeleteAsync("auth");
    }
    
    public async Task<bool> LogOut()
    {
        var res = await BHttp.Request("Auth/logout");
        if (!res.IsOk)
            return false;

        await LocalLogOut();

        return true;
    }
}