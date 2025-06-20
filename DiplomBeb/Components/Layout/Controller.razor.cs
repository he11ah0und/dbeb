using DiplomBeb.Services;
using DiplomBeb.Services.Client;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DiplomBeb.Components.Layout;

public partial class Controller : ComponentBase
{
    [Inject]
    private AccountService AccountService { get; set; }
    [Inject]
    private NotificationService NotificationService { get; set; }
    protected override async Task OnInitializedAsync()
    {
        
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        if (!await AccountService.Auth())
                NotificationService.AddNotification("Ошибка аутентификации.");
    }
}