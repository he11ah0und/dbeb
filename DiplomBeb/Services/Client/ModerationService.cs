using DiplomBeb.Components.Views;
using DiplomBeb.Database.Model;
using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client;

public class ModerationService
{
    public ListView UsersListView { get; private set; }
    public ListView LogsListView { get; private set; }

    public ModerationService(BackendHttpClient http)
    {
        UsersListView = new ListView(http, typeof(TableContent))
        {
            OnTotal = RequestUsersTotal,
            OnGet = RequestUsers,
            GetHeader = () => "Пользователи"
        };
        UsersListView.DataResolver.Props.AddRange(
            ["id", "avatar", "name", "middlename", "surname", "score", "webrole_name"]);
        
        LogsListView = new ListView(http, typeof(TableContent))
        {
            OnTotal = RequestLogsTotal,
            OnGet = RequestLogs,
            GetHeader = () => "Логи",
        };
        LogsListView.DataResolver.Props.AddRange(
            ["id", "name", "middlename", "surname", "action", "datetime", "webrole_name"]);
    }
    private Task<HttpResponseState> RequestUsers(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.Request($"Users/get/{skip}/{take}");
    }

    private Task<HttpResponseState> RequestUsersTotal(BackendHttpClient http)
    {
        return http.NativeRequest("Users/total");
    }
    
    private Task<HttpResponseState> RequestLogs(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.Request($"Logs/{skip}/{take}");
    }

    private Task<HttpResponseState> RequestLogsTotal(BackendHttpClient http)
    {
        return http.Request("Logs/total");
    }
}