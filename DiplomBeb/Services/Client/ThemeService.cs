using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DiplomBeb.Services.Client;

public class ThemeService
{
    public ProtectedLocalStorage ProtectedLocalStore { get; set; }


    public const string DefaultTheme = "light-theme";
    public string CurrentTheme { get; set; } = DefaultTheme;

    public bool InDark
    {
        get => CurrentTheme == "dark-theme";
        set => CurrentTheme = value ? "dark-theme" : "light-theme";
    }
    public ThemeService(ProtectedLocalStorage protectedLocalStore)
    {
        ProtectedLocalStore = protectedLocalStore;
    }
    
    public Action OnSwitch { get; set; }

    public async Task Load()
    {
        var state = await ProtectedLocalStore.GetAsync<string>("theme");
        if (!state.Success)
            return;
        CurrentTheme = state.Value;
    }
    
    public async Task Change()
    {
        InDark = !InDark;
        await ProtectedLocalStore.SetAsync("theme", CurrentTheme);
        OnSwitch?.Invoke();
    }
}