using DiplomBeb.Services.Client;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages.Moderating;

public partial class Profiles : ComponentBase
{
    [Inject]
    private ModerationService ModerationService { get; set; }
    
    [Inject]
    private BackendHttpClient Http { get; set; }

    private ulong Page { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        
    }

    private void UsersStateChanged()
    {
        StateHasChanged();
    }

    private Task Update()
    {
        return Update(0);
    }

    private async Task Update(ulong page = 0)
    {
        if (page == 0)
        {
            Page = 1;
            page = Page;
        }

        await ModerationService.RequestUsers(Http, page);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        
        Page = 1;
        ModerationService.UsersStateChanged.Subscribe("users", UsersStateChanged);
        await Update();
    }
}