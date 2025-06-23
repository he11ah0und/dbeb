using DiplomBeb.Services.Client;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Views;

public partial class SideBar
{
    
    [Inject] 
    private AccountService AccountService { get; set; }
    
    [Inject]
    private ThemeService ThemeService { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    
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

    private bool LowViewState { get; set; }
    private void OnSwitchLowView()
    {
        LowViewState = !LowViewState;
        StateHasChanged();
    }

    private void AccountServiceOnStateChanged()
    {
        StateHasChanged();
    }

    private void GoToAccount() => Navigation.NavigateTo("/account");
    
    private void GoToMain() => Navigation.NavigateTo("/");
    private void GoToAbout() => Navigation.NavigateTo("/about");
    private void GoToLibrary() => Navigation.NavigateTo("/library/1");
    
    private void GoToModLogs() => Navigation.NavigateTo("/special/logs");
    private void GoToModProfiles() => Navigation.NavigateTo("/special/profiles");
    private void GoToModDocs() => Navigation.NavigateTo("/special/docs");
    private void GoToModRequests() => Navigation.NavigateTo("/special/requests");
}

