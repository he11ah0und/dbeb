using System.Diagnostics;
using System.Text;
using DiplomBeb.Components.Views;
using DiplomBeb.Services.Client.Components;
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
    public ListElement UserRank { get; private set; }
    public bool IsLogged { get; private set; }
    
    internal ListView MyListView { get; private set; }
    public Callbacker<Action> StateChanged { get; private set; }

    public DocumentsService DocumentsService { get; private set; }
    private BackendHttpClient _http;
    private IJSRuntime _jsRuntime;
    public AccountService(BackendHttpClient http, IJSRuntime jsRuntime)
    {
        _http = http;
        Initialized = false;
        StateChanged = new Callbacker<Action>(false);
        UserRank = new ListElement();
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
        DocumentsService = new DocumentsService(http, jsRuntime);
    }
    
    private Task<HttpResponseState> RequestUsers(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.Request($"Users/personal");
    }

    public async Task Auth()
    {
        await MyListView.First(1);
        if (IsLogged = MyListView.Data.LongLength > 0)
        {
            var res = await _http.Request($"Users/toppos/{Account.Get("id")}");
            IsLogged = res.IsOk;
            if (IsLogged = res.ResultAsJson<ListElement>(out var rank))
                UserRank = rank;
        }

        Initialized = true;
        StateHasChanged();
    }

    public async Task<bool> Update(ListElement li)
    {
        var res = await _http.Request(
            "Users/personal",
            JsonContent.Create(li),
            HttpMethod.Patch);
        return res.IsOk;
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
        {
            Debug.WriteLine("failed login");
            return false;
        }

        return true;
    }
    
    private void StateHasChanged()
    {
        StateChanged.InvokeAll();
    }
}