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
    private LibraryService LibraryService { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        
    }

    private void OnNavigate()
    {
        Navigation.NavigateTo($"/library/{LibraryService.ListView.Page}", true);
    }
    
    private void OnListUpdated()
    {
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        
        LibraryService.ListView.OnPageSwitch.Subscribe("library", OnNavigate);
        
        await LibraryService.ListView.First((ulong)Page);
    }
}