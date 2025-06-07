using DiplomBeb.Services.Client;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Layout;

public partial class SideBar
{
    [Inject] 
    private AccountService AccountService { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        AccountService.StateChanged.Subscribe("sidebar", AccountServiceOnStateChanged);
    }

    private void AccountServiceOnStateChanged()
    {
        StateHasChanged();
    }
}