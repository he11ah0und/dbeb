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

    public AccountService()
    {
        Initialized = false;
        StateChanged = new Callbacker();
        Account = new User();
        UserRank = new UserRank();
        IsLogged = false;
    }

    public async Task Auth(
        BackendHttpClient http)
    {
        
        var res = await http.Request("Users/personal");
        IsLogged = res.IsOk;
        if (res.ResultAsJson<User>(out var user))
            Account = user;
            
        res = await http.Request($"Users/top/{Account.Id}");
        IsLogged = res.IsOk;
        if (res.ResultAsJson<UserRank>(out var rank))
            UserRank = rank;
        
        Initialized = true;
        StateHasChanged();
    }

    public async Task<bool> SignUp(
        SignUpData SignUpData,
        string Password,
        string ConfirmPassword,
        bool Confirmed,
        BackendHttpClient http)
    {
        if (string.IsNullOrEmpty(Password)
            || string.IsNullOrEmpty(ConfirmPassword)
            || Password != ConfirmPassword
            || !Confirmed)
            return false;

        SignUpData.Password = Encoding.Default.GetBytes(Password);
        if (!await http.Auth.TrySignUp(
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