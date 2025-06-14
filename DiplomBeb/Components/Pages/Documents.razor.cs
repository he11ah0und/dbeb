using DiplomBeb.Services.Client;
using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages;

public partial class Documents : ComponentBase
{
    [Inject] 
    private AccountService AccountService { get; set; }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await AccountService.DocumentsService.DocumentsListView.First(1);
    }
    
    private void DocumentsUpdated()
    {
        StateHasChanged();
    }

    private bool InUpload { get; set; }
    private void HideUploader()
    {
        InUpload = false;
        StateHasChanged();
    }

    private void ShowUploader()
    {
        InUpload = true;
        StateHasChanged();
    }
    
    
    // ERROR HANDLING
    [Inject]
    private NavigationManager Navigation { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AccountService.StateChanged.Subscribe("404", OnLoaded);
        
    }
    private void OnLoaded()
    {
        if (ulong.TryParse(AccountService.Account?.Get("apirole_id"), out var val)
            && val >= 5)
            return;
        Navigation.NavigateTo($"/error/403");
    }
}