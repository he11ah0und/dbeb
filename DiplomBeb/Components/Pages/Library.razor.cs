using System.Diagnostics;
using System.Net;
using DiplomBeb.Database.Model;
using DiplomBeb.Services;
using DiplomBeb.Services.Client;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages;

public partial class Library
{
    [Inject]
    private BackendHttpClient Http { get; set; }
    [Inject]
    private MainService MainService { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        MainService.Library.LibraryStateChanged.Subscribe("library", MainServiceOnLibraryStateChanged);
        Update();
    }

    private void Update()
    {
        MainService.Library.RequestLibsTotal(Http);
        MainService.Library.RequestLibs(Http, 1);
    }
    
    private void MainServiceOnLibraryStateChanged()
    {
        StateHasChanged();
    }
}