using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DiplomBeb.Utils.Networking;

public class BackendHttpClient
{
    internal ProtectedLocalStorage ProtectedLocalStore { get; set; }
    public AuthWorker Auth { get; private set; }
    private HttpClient Http;

    public BackendHttpClient(ProtectedLocalStorage pls)
    {
        ProtectedLocalStore = pls;
        Http = new HttpClient();
        Http.BaseAddress = new Uri("http://localhost:5062/api/");
        Auth = new AuthWorker(this);
    }

    public async Task<HttpResponseState> Request(
        string url,
        HttpContent content = null,
        HttpMethod method = null)
    {
        HttpResponseState state = new HttpResponseState(false);
        ProtectedBrowserStorageResult<string>? token = await Auth.GetAuthToken();
        
        if (!token.HasValue || !token.Value.Success)
        {
            return state;
        }
        var tokenVal = token.Value.Value;
            
        Action<HttpRequestHeaders>? fnAuth = string.IsNullOrEmpty(tokenVal) ? null :
            (HttpRequestHeaders h) =>
        {
            h.Authorization = new AuthenticationHeaderValue("Bearer", tokenVal);
        };
        state = await NativeRequest(url,
            content,
            method,
            fnAuth);
        if (!state.IsOk)
        {
            Auth.LocalLogOut();
        }
        return state;
    }
    
    public async Task<HttpResponseState> NativeRequest(
        string url,
        HttpContent content = null,
        HttpMethod method = null,
        Action<HttpRequestHeaders> fnSetHeaders = null)
    {
        var state = new HttpResponseState(false);
        try
        {
            var mes = new HttpRequestMessage(method is null ? HttpMethod.Get : method, url);
            fnSetHeaders?.Invoke(mes.Headers);
            mes.Content = content;
            var res = state.Value = await Http.SendAsync(mes);
            
            state.IsOk = res.IsSuccessStatusCode;
            state.Value = res;
            
            return state;
        }
        catch (Exception ex)
        {
            state.Exception = ex;
            return state;
        }
    }
}