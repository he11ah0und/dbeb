using DiplomBeb.Services.Client;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages.Auth;

public partial class Profile
{
    [Inject] 
    private AccountService AccountService { get; set; }
    [Inject]
    private BackendHttpClient Http { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    
    private bool InEdit { get; set; }
    
    public string Surname { get; set; }
    public string Name { get; set; }
    public string Middlename { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        AccountService.StateChanged.Subscribe("profile", AccountServiceOnStateChanged);
        AccountService.DocumentsService.OnDocumentsUpdated.Subscribe("profile", DocumentsUpdated);
        await AccountService.DocumentsService.GetDocs();
    }

    private void Load()
    {
        Surname = AccountService.Account.Get("surname");
        Name = AccountService.Account.Get("name");
        Middlename = AccountService.Account.Get("middlename");
        Phone = AccountService.Account.Get("phone", "");
        Email = AccountService.Account.Get("email");
    }

    private void AccountServiceOnStateChanged()
    {
        Load();
        StateHasChanged();
    }

    private void DocumentsUpdated()
    {
        StateHasChanged();
    }

    private async Task EditMode()
    {
        if (!InEdit)
        {
            InEdit = true;
        }
        else if (InEdit)
        {
            var newLe = new ListElement();
            newLe.Set("phone", Phone);
            newLe.Set("email", Email);
            newLe.Set("name", Name);
            newLe.Set("surname", Surname);
            newLe.Set("middlename", Middlename);
            await AccountService.Update(newLe);
            InEdit = false;
        }
        StateHasChanged();
    }

    private void OpenPlace()
    {
        var place = AccountService?.Account.Get("place_id");
        
        if (string.IsNullOrEmpty(place))
            return;
        Navigation.NavigateTo($"/place/{place}");
    }

    private async Task OnLogOut()
    {
        await Http.Auth.LogOut();
        Navigation.NavigateTo("/account", true);
    }
}