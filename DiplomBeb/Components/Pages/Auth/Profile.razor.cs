using DiplomBeb.Services.Client;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages.Auth;

public partial class Profile
{
    [Inject] 
    private AccountService AccountService { get; set; }
    [Inject]
    private BackendHttpClient Http { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    
    private bool InEdit { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        AccountService.StateChanged.Subscribe("profile", AccountServiceOnStateChanged);
    }

    private void AccountServiceOnStateChanged()
    {
        StateHasChanged();
    }

    private void EditMode()
    {
        if (!InEdit)
        {
            InEdit = true;
        }
        else if (InEdit)
        {
            
            InEdit = false;
        }
        StateHasChanged();
    }

    private void OpenPlace()
    {
        var place = AccountService?.Account.Get("place_id");
        
        if (string.IsNullOrEmpty(place))
            return;
        Navigation.NavigateTo($"/place/{place}");
    }

    private async Task OnLogOut()
    {
        await Http.Auth.LogOut();
        Navigation.NavigateTo("/account", true);
    }
}