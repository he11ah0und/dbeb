using DiplomBeb.Services.Client;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages;

public partial class Documents : ComponentBase
{
    [Inject] 
    private AccountService AccountService { get; set; }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        AccountService.DocumentsService.DocumentsListView.OnListUpdated.Subscribe("documents", DocumentsUpdated);
        await AccountService.DocumentsService.DocumentsListView.First(1);
    }
    
    private void DocumentsUpdated()
    {
        StateHasChanged();
    }
}