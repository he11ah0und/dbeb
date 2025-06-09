using DiplomBeb.Database.Model;
using DiplomBeb.Services.Client.Components;
using DiplomBeb.Utils;

namespace DiplomBeb.Services.Client;

public class MainService
{
    public UsersComponent Users { get; private set; }
    
    public MainService()
    {
        Users = new UsersComponent();
    }
}