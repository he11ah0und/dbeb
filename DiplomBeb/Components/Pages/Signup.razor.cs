using System.Diagnostics;
using System.Net;
using System.Text;
using DiplomBeb.Filters;
using DiplomBeb.Services;
using DiplomBeb.Services.Client;
using DiplomBeb.Systems.Auth;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DiplomBeb.Components.Pages;

public partial class Signup : ComponentBase
{
    [Inject] 
    private AccountService AccountService { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }

    private SignUpData SignUpData;
    private string Password;
    private string ConfirmPassword;
    private bool Confirmed;
    
    protected override async Task OnInitializedAsync()
    {
        Confirmed = false;
        SignUpData = new SignUpData();
        
        AccountService.StateChanged.Subscribe("signup", AccountServiceOnStateChanged);
    }

    private void AccountServiceOnStateChanged()
    {
        StateHasChanged();
        if (!AccountService.IsLogged) return;
        Navigation.NavigateTo("/account");
    }

    private async void OnRegistrate()
    {
        if (await AccountService.SignUp(
                SignUpData,
                Password,
                ConfirmPassword,
                Confirmed))
        {
            Navigation.NavigateTo("/account", true);
        }
    }
}