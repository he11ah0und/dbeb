using System.Diagnostics;
using DiplomBeb.Utils.Networking;
using dshared.Utils;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages.Auth;

public partial class Signin
{
    [Inject]
    private BackendHttpClient Http { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    
    
    protected override async Task OnInitializedAsync()
    {
        Password = "";
    }

    private string Password;
    private string Email;
    
    private async Task HandleValidSubmit()
    {
        if (string.IsNullOrEmpty(Password))
            return;
        var li = new ListElement();
        li.Set("email", Email);
        li.Set("password",Password);
        
        if (!await Http.Auth.TrySignIn(li))
        {
            Debug.WriteLine("failed login");
            return;
        }

        Navigation.NavigateTo("/account", true);
    }
}