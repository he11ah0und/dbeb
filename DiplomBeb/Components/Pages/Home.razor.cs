using DiplomBeb.Services.Client;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages;

public partial class Home
{
    [Inject]
    private BackendHttpClient Http { get; set; }
    [Inject] 
    private AccountService AccountService { get; set; }
    
    [Inject]
    private MainService MainService { get; set; }
    [Inject]
    private LibraryService LibraryService { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        AccountService.StateChanged.Subscribe("home", AccountServiceOnStateChanged);
        MainService.Users.UsersTopStateChanged.Subscribe("home", MainServiceOnUsersTopStateChanged);
        LibraryService.LibraryRandomStateChanged.Subscribe("home", MainServiceOnUsersTopStateChanged);
        LibraryService.RequestRandomLibs(Http);
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