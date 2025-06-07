using System.Diagnostics;
using System.Text;
using DiplomBeb.Database.Model;
using DiplomBeb.Filters;
using DiplomBeb.Systems.Accounts;
using DiplomBeb.Systems.Auth;
using DiplomBeb.Utils;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

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
        var islogged = await http.Auth.Validate();
        
        IsLogged = islogged;
        if (IsLogged)
        {
            Account = await AccountsEngineClient.GetPersonal(
                http);
            UserRank = await AccountsEngineClient.GetRank(
                (ulong)Account.Id,
                http);
        }
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