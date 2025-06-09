using DiplomBeb.Services.Client;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Layout;

public partial class SideBar
{
    [Inject] 
    private AccountService AccountService { get; set; }
    
    [Inject]
    private ThemeService ThemeService { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        AccountService.StateChanged.Subscribe("sidebar", AccountServiceOnStateChanged);
    }

    private void SwitchTheme()
    {
        ThemeService.Change();
    }

    private void AccountServiceOnStateChanged()
    {
        StateHasChanged();
    }
}