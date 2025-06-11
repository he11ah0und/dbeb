using System.Diagnostics;
using DiplomBeb.Services.Client;
using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
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
        Navigation.NavigateTo($"/library/{LibraryService.LibraryListView.Page}", true);
    }

    private void OnFiltersCallback(string propName,
        ulong index,
        ListElement listElement,
        ObjectContainer args)
    {
        Debug.WriteLine("OnFiltersCallback");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        
        LibraryService.LibraryListView.OnPageSwitch.Subscribe("library", OnNavigate);
        LibraryService.FiltersListView.OnElementCallback.Subscribe("library", OnFiltersCallback);
        
        await LibraryService.LibraryListView.First((ulong)Page);
        await LibraryService.FiltersListView.First(1);
    }
}