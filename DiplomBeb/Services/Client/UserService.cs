using DiplomBeb.Services.Client.Components;

namespace DiplomBeb.Services.Client;

public class UserService
{
    public PlaceComponent Place { get; private set; }

    public UserService(AccountService accountService)
    {
        Place = new PlaceComponent(accountService);
    }
}