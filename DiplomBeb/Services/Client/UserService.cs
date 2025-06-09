using DiplomBeb.Services.Client.Components;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client;

public class UserService
{
    public PlaceComponent Place { get; private set; }

    public UserService(BackendHttpClient http, AccountService accountService)
    {
        Place = new PlaceComponent(http, accountService);
    }
}