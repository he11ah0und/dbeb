using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DiplomBeb.Services.Client;

public class ThemeService
{
    [Inject]
    public ProtectedLocalStorage ProtectedLocalStore { get; set; }
    
    public bool InDark = false;
    public string ClassDark => InDark ? "dark" : "";
    
    public Action OnSwitch { get; set; }

    public void Load()
    {
        
    }
    
    public void Change()
    {
        InDark = !InDark;
        OnSwitch?.Invoke();
    }
}