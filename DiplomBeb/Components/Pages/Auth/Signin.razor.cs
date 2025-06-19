using System.Diagnostics;
using DiplomBeb.Services.Client;
using DiplomBeb.Utils.Networking;
using dshared.Utils;
using Microsoft.AspNetCore.Components;

namespace DiplomBeb.Components.Pages.Auth;

public partial class Signin
{
    [Inject]
    private BackendHttpClient Http { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    [Inject]
    private NotificationService NotificationService { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        Password = "";
    }

    private string Password;
    private string Email;
    
    private async Task HandleValidSubmit()
    {
        if (string.IsNullOrEmpty(Password))
        {
            NotificationService.AddNotification("Данные для входа пусты!");
            return;
        }
        var li = new ListElement();
        li.Set("email", Email);
        li.Set("password",Password);
        
        if (!await Http.Auth.TrySignIn(li))
        {
            NotificationService.AddNotification("Неудачная попытка входа.");
            return;
        }

        Navigation.NavigateTo("/account", true);
    }
}