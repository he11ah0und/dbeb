using System.Diagnostics;
using DiplomBeb.Services.Client;
using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace DiplomBeb.Components.Pages;

public partial class Library
{
    [Parameter]
    public long? Page { get; set; }
    
    [Parameter]
    public string[]? Filters { get; set; }
    
    [Inject]
    private LibraryService LibraryService { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    [Inject]
    private NotificationService NotificationService { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        
    }

    private void OnNavigate()
    {
        Navigation.NavigateTo($"/library/{LibraryService.LibraryListView.Page}", true);
    }

    

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        
        LibraryService.LibraryListView.OnPageSwitch.Subscribe("library", OnNavigate);
        
        if (!await LibraryService.LibraryListView.First((ulong)Page))
            NotificationService.AddNotification("Ошибка получения информации из библиотеки.");
        if (!await LibraryService.FiltersListView.First(1))
            NotificationService.AddNotification("Ошибка получения фильтров библиотеки.");
    }
}