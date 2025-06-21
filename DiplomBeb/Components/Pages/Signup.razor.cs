using System.Diagnostics;
using System.Net;
using System.Text;
using DiplomBeb.Services;
using DiplomBeb.Services.Client;
using DiplomBeb.Utils.Networking;
using dshared.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DiplomBeb.Components.Pages;

public partial class Signup : ComponentBase
{
    [Inject] 
    private AccountService AccountService { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    [Inject]
    private NotificationService NotificationService { get; set; }

    private string Surname;
    private string Name;
    private string Middlename;
    private string Email;
    private string Password;
    private string ConfirmPassword;
    private bool Confirmed;
    
    protected override async Task OnInitializedAsync()
    {
        Confirmed = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        
        AccountService.StateChanged.Subscribe("signup", AccountServiceOnStateChanged);
    }

    private void AccountServiceOnStateChanged()
    {
        StateHasChanged();
        if (!AccountService.IsLogged) return;
        Navigation.NavigateTo("/account");
    }

    private async void OnRegistrate()
    {
        var li = new ListElement();
        li.Set("surname", Surname);
        li.Set("name", Name);
        li.Set("middlename", Middlename);
        li.Set("email", Email);
        li.Set("password", Password);
        li.Set("confirmPassword", ConfirmPassword);
        li.Set("confirmed", Confirmed);
        if (!await AccountService.SignUp(li))
        {
            NotificationService.AddNotification("Ошибка регистрации.");
            return;
        }
        Navigation.NavigateTo("/account");
    }
}