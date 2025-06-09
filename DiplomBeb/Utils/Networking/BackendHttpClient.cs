using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DiplomBeb.Utils.Networking;

public class BackendHttpClient
{
    internal ProtectedLocalStorage ProtectedLocalStore { get; set; }
    public AuthWorker Auth { get; private set; }
    private HttpClient _http;

    public BackendHttpClient(ProtectedLocalStorage pls)
    {
        ProtectedLocalStore = pls;
        _http = new HttpClient();
        _http.BaseAddress = new Uri("http://localhost:5062/api/");
        Auth = new AuthWorker(this);
    }

    public async Task<HttpResponseState> Request(
        string url,
        HttpContent content = null,
        HttpMethod method = null)
    {
        HttpResponseState state = new HttpResponseState(false);
        var token = await Auth.GetAuthToken();

        if (!token.HasValue)
            return state;
            
        Action<HttpRequestHeaders>? fnAuth = string.IsNullOrEmpty(token.Value.Value) ? null :
            (HttpRequestHeaders h) =>
        {
            h.Authorization = new AuthenticationHeaderValue("Bearer", token.Value.Value);
        };
        state = await NativeRequest(url,
            content,
            method,
            fnAuth);
        if (!state.IsOk)
        {
            await Auth.LocalLogOut();
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
            var res = state.Value = await _http.SendAsync(mes);
            
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