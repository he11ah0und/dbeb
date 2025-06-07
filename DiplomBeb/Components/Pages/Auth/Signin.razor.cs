using System.Diagnostics;
using System.Text;
using DiplomBeb.Database.Model;
using DiplomBeb.Filters;
using DiplomBeb.Systems.Auth;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DiplomBeb.Components.Pages.Auth;

public partial class Signin
{
    [Inject]
    private BackendHttpClient Http { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    
    private SignInData SignInData;
    protected override async Task OnInitializedAsync()
    {
        Password = "";
        SignInData = new SignInData();
    }

    private string Password;
    
    private async void HandleValidSubmit()
    {
        if (string.IsNullOrEmpty(Password))
            return;
        
        SignInData.Password = Encoding.Default.GetBytes(Password);
        if (!await Http.Auth.TrySignIn(SignInData))
        {
            Debug.WriteLine("failed login");
            return;
        }

        Navigation.NavigateTo("/account", true);
    }
}