using DiplomBeb.Services.Client;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Views;

public partial class SideBar
{
    [Inject] 
    private AccountService AccountService { get; set; }
    
    [Inject]
    private ThemeService ThemeService { get; set; }
    
    public bool OpenedModeration { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        AccountService.StateChanged.Subscribe("sidebar", AccountServiceOnStateChanged);
    }

    public void SwitchModeration()
    {
        OpenedModeration = !OpenedModeration;
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