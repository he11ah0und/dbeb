using DiplomBeb.Services.Client;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;
using dshared.Utils;
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
    public ulong PlaceId { get; set; }
    public bool Selected { get; set; }
    

    [Inject]
    private AccountService AccountService { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    [Inject]
    private NotificationService NotificationService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AccountService.StateChanged.Subscribe("404", OnLoaded);
        
        if (!await ModerationService.RequestPlaces())
        {
            NotificationService.AddNotification("Ошибка получения информации о местах!");
            return;
        }
    }
    private void OnLoaded()
    {
        if (ulong.TryParse(AccountService.Account?.Get("apirole_id"), out var val)
            && val >= 5)
            return;
        Navigation.NavigateTo($"/error/403");
    }

    private async Task EditMode()
    {
        if (!InEdit)
        {
            InEdit = true;
        }
        else if (InEdit)
        {
            var selected = ModerationService.SelectedUser;
            var newLe = new ListElement();
            newLe.Set("phone", Phone);
            newLe.Set("email", Email);
            newLe.Set("name", Name);
            newLe.Set("surname", Surname);
            newLe.Set("middlename", Middlename);
            newLe.Set("place_id", PlaceId);
            
            if (!await AccountService.Update(newLe, ulong
                .Parse(selected.Get("internalId", "0"))))
                NotificationService
                    .AddNotification($"Ошибка обновления данных пользователя: " +
                                                    $"id: {selected.Get("id")} " +
                                                    $"{selected.Get("surname")} {selected.Get("name")} {selected.Get("middlename")}");
            InEdit = false;
        }
        StateHasChanged();
    }

    private void OnChangePlace(ChangeEventArgs e)
    {
        PlaceId = ulong.Parse((string)e.Value);
    }

    private void Load()
    {
        var selected = ModerationService.SelectedUser;
        if (selected is null)
        {
            Selected = false;
            return;
        }
        Surname = selected.Get("surname");
        Name = selected.Get("name");
        Middlename = selected.Get("middlename");
        Phone = selected.Get("phone", "");
        Email = selected.Get("email");
        PlaceId = ulong.Parse(selected.Get("place_id", "0"));
        Selected = true;
        InEdit = false;
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
    
    private bool InRequest { get; set; }
    
    private void HideRequester()
    {
        InRequest = false;
        StateHasChanged();
    }
    private void ClearRequester()
    {
        
    }
    private void ShowRequester()
    {
        
        InRequest = true;
        StateHasChanged();
    }
}