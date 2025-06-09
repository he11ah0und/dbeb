using DiplomBeb.Services.Client;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages;

public partial class Account
{
    [Inject] 
    private AccountService AccountService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AccountService.StateChanged.Subscribe("account", AccountServiceOnStateChanged);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        
    }

    private void AccountServiceOnStateChanged()
    {
        StateHasChanged();
    }
}