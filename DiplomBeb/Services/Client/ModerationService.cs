using DiplomBeb.Database.Model;
using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client;

public class ModerationService
{
    public ListView ListView { get; private set; }

    public ModerationService(BackendHttpClient http)
    {
        ListView = new ListView(http)
        {
            OnTotal = RequestUsersTotal,
            OnGet = RequestUsers,
            GetHeader = () => "Пользователи"
        };
        ListView.DataResolver.Props.AddRange(
            ["id", "avatar", "name", "middlename", "surname", "score", "webrole_name"]);
    }
    private Task<HttpResponseState> RequestUsers(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.Request($"Users/get/{skip}/{take}");
    }

    private Task<HttpResponseState> RequestUsersTotal(BackendHttpClient http)
    {
        return http.NativeRequest("Users/total");
    }
}