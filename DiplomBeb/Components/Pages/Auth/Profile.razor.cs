using System.Diagnostics;
using DiplomBeb.Database.Model;
using DiplomBeb.Filters;
using DiplomBeb.Services;
using DiplomBeb.Services.Client;
using DiplomBeb.Systems;
using DiplomBeb.Systems.Accounts;
using DiplomBeb.Systems.Auth;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DiplomBeb.Components.Pages.Auth;

public partial class Profile
{
    [Inject] 
    private AccountService AccountService { get; set; }
    [Inject]
    private BackendHttpClient Http { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    
    public bool IsShouldRender { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        AccountService.StateChanged.Subscribe("profile", AccountServiceOnStateChanged);
    }

    private void AccountServiceOnStateChanged()
    {
        IsShouldRender = true;
        StateHasChanged();
    }

    protected override bool ShouldRender()
    {
        return IsShouldRender;
    }

    private void OnLogOut()
    {
        Http.Auth.LogOut();
        Navigation.NavigateTo("/account", true);
    }
}