using DiplomBeb.Components.Views;
using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;
using dshared.Utils;
using Microsoft.JSInterop;

namespace DiplomBeb.Services.Client;

public class AccountService
{
    public bool Initialized { get; private set; }
    public ListElement? Account => MyListView.Data.FirstOrDefault();
    public bool IsLogged { get; private set; }
    
    internal ListView MyListView { get; private set; }
    public Callbacker<Action> StateChanged { get; private set; }
    public Callbacker<Action> StateChangedNoCall { get; private set; }
    public Uploader AvatarUploader { get; private set; }
    
    private BackendHttpClient _http;
    
    public byte[]? AvatarImage { get; private set; }
    public AccountService(BackendHttpClient http, NotificationService nots, IJSRuntime jsRuntime)
    {
        _http = http;
        Initialized = false;
        StateChanged = new Callbacker<Action>();
        StateChangedNoCall = new Callbacker<Action>(false);
        IsLogged = false;
        MyListView = new ListView(http, typeof(TableContent),1, false)
        {
            ElementsOnPage = 3,
            HideFooter = true,
            HideRightFooterButtons = true,
            HideLeftFooterButtons = true,
            HideHeading = true,
            HideTableHeader = true,
            FitContent = true,
            OnGet = RequestUsers,
        };
        MyListView.DataResolver.Props.AddRange(
            ["id", "avatar", "name", "middlename", "surname", "score", "webrole_name"]);

        AvatarUploader = new Uploader(http, nots, "images/prof-foto-img.png")
        {

        };
    }
    
    private Task<HttpResponseState> RequestUsers(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.Request($"Users/personal");
    }

    public async Task<bool> Auth()
    {
        IsLogged = false;
        if (!await MyListView.First(1)
            || MyListView.Data.LongLength == 0)
            return End(false);
        
        IsLogged = true;
        return End(true);

        bool End(bool state)
        {
            Initialized = true;
            StateHasChanged();
            return state;
        }
    }

    public async Task<bool> SignIn(ListElement li, bool encryptPass = true)
    {
        if (!await _http.Auth.TrySignIn(li, encryptPass))
        {
            return false;
        }

        return await Auth();
    }

    public async Task<bool> Update(ListElement li, ulong id = 0)
    {
        var url = "Users/personal" + (id > 0 ? $"/{id}" : "");
        var res = await _http.Request(
            url,
            JsonContent.Create(li),
            HttpMethod.Patch);
        return res.IsOk;
    }

    public async Task<bool> LogOut()
    {
        if (!await _http.Auth.LogOut())
            return false;
        
        return await Auth();
    }

    public async Task<bool> SignUp(
        ListElement signUpData)
    {
        var password = signUpData.Get("password");
        var confirmPassword = signUpData.Get("confirmPassword");
        var confirmed = bool.Parse(signUpData.Get("confirmed"));
        if (string.IsNullOrEmpty(password)
            || string.IsNullOrEmpty(confirmPassword)
            || password != confirmPassword
            || !confirmed)
            return false;
        
        // remove confirmpassword from packet
        signUpData.Del("confirmPassword");
        
        if (!await _http.Auth.TrySignUp(
                signUpData))
            return false;
        
        return await SignIn(signUpData, false);
    }
    
    private void StateHasChanged()
    {
        StateChanged.InvokeAll();
        StateChangedNoCall.InvokeAll();
    }
}