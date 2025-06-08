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
    [Parameter]
    public long? Page { get; set; }
    
    [Inject]
    private BackendHttpClient Http { get; set; }
    [Inject]
    private MainService MainService { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        MainService.Library.LibraryStateChanged.Subscribe("library", MainServiceOnLibraryStateChanged);
        Update();
    }

    private void OpenPage(ulong page)
    {
        Navigation.NavigateTo($"/library/{page}", true);
    }

    public bool CanNext => (ulong)Page + 1 <= MainService.Library.TotalPages;
    public bool CanBack => (ulong)Page - 1 >= 1;

    private void BackPage()
    {
        var newpage = (ulong)Page - 1;
        if (newpage < 1)
            return;
        Navigation.NavigateTo($"/library/{newpage}", true);
    }

    private void NextPage()
    {
        var newpage = (ulong)Page + 1;
        if (newpage > MainService.Library.TotalPages)
            return;
        Navigation.NavigateTo($"/library/{newpage}", true);
    }
    
    private void Update()
    {
        MainService.Library.RequestLibsTotal(Http);
        MainService.Library.RequestLibsTotalPages(Http);
        MainService.Library.RequestLibs(Http, (ulong)Page);
    }
    
    private void MainServiceOnLibraryStateChanged()
    {
        StateHasChanged();
    }
}