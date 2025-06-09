using System.Diagnostics;
using System.Text;
using DiplomBeb.Database.Model;
using DiplomBeb.Filters;
using DiplomBeb.Utils;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client;

public class AccountService
{
    public bool Initialized { get; private set; }
    public User Account { get; private set; }
    public UserRank UserRank { get; private set; }
    public bool IsLogged { get; private set; }
    
    public Callbacker StateChanged { get; private set; }

    private BackendHttpClient _http;
    public AccountService(BackendHttpClient http)
    {
        _http = http;
        Initialized = false;
        StateChanged = new Callbacker();
        Account = new User();
        UserRank = new UserRank();
        IsLogged = false;
    }

    public async Task Auth()
    {
        var res = await _http.Request("Users/personal");
        IsLogged = res.IsOk;
        if (IsLogged = res.ResultAsJson<User>(out var user))
            Account = user;

        if (IsLogged)
        {
            res = await _http.Request($"Users/toppos/{Account?.Id}");
            IsLogged = res.IsOk;
            if (IsLogged = res.ResultAsJson<UserRank>(out var rank))
                UserRank = rank;
        }

        Initialized = true;
        StateHasChanged();
    }

    public async Task<bool> Update(User user)
    {
        var res = await _http.Request("Users", JsonContent.Create(user), HttpMethod.Patch);
        return res.IsOk;
    }

    public async Task<bool> SignUp(
        SignUpData SignUpData,
        string Password,
        string ConfirmPassword,
        bool Confirmed)
    {
        if (string.IsNullOrEmpty(Password)
            || string.IsNullOrEmpty(ConfirmPassword)
            || Password != ConfirmPassword
            || !Confirmed)
            return false;

        SignUpData.Password = Encoding.Default.GetBytes(Password);
        if (!await _http.Auth.TrySignUp(
                SignUpData))
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