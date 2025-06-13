using DiplomBeb.Services.Client;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages.Moderating;

public partial class Profiles : ComponentBase
{
    [Inject]
    private ModerationService ModerationService { get; set; }
    
    private bool InEdit { get; set; }
    
    public string Surname { get; set; }
    public string Name { get; set; }
    public string Middlename { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    
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
          //  await AccountService.Update(newLe);
            InEdit = false;
        }
        StateHasChanged();
    }

    private void Load()
    {
        Surname = ModerationService.SelectedUser.Get("surname");
        Name = ModerationService.SelectedUser.Get("name");
        Middlename = ModerationService.SelectedUser.Get("middlename");
        Phone = ModerationService.SelectedUser.Get("phone", "");
        Email = ModerationService.SelectedUser.Get("email");
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        ModerationService.OnSelectedUser.Subscribe("logs", OnSelected);
        await ModerationService.UsersListView.First(1);
    }
    
    private void OnSelected()
    {
        Load();
        StateHasChanged();
    }
}