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

namespace DiplomBeb.Components.Pages;

public partial class Home
{
    [Inject]
    private BackendHttpClient Http { get; set; }
    [Inject] 
    private AccountService AccountService { get; set; }
    
    [Inject]
    private MainService MainService { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        AccountService.StateChanged.Subscribe("home", AccountServiceOnStateChanged);
        MainService.Users.UsersTopStateChanged.Subscribe("home", MainServiceOnUsersTopStateChanged);
        MainService.Library.Library4StateChanged.Subscribe("home", MainServiceOnUsersTopStateChanged);
        MainService.Library.RequestRandom4Libs(Http);
        Update();
    }
    private void Update()
    {
        MainService.Users.RequestUsersTop(Http);
    }

    private void MainServiceOnUsersTopStateChanged()
    {
        StateHasChanged();
    }

    private void AccountServiceOnStateChanged()
    {
        StateHasChanged();
    }

    private void OnUpdateClick()
    {
        Update();
    }
}