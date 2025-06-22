using DiplomBeb.Services.Client;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages.Moderating;

public partial class Docs : ComponentBase
{
    [Inject]
    private AccountService AccountService { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    
    private bool Started { get; set; }
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

    private void Begin()
    {
        Started = true;
        StateHasChanged();
    }
}