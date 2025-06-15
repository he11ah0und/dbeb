using DiplomBeb.Components.Views;
using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;
using dshared.Utils;

namespace DiplomBeb.Services.Client;

public class ModerationService
{
    public ListView UsersListView { get; private set; }
    public ListElement? SelectedUser { get; private set; }
    public ListView LogsListView { get; private set; }
    public ListElement? SelectedLog { get; private set; }
    public Callbacker<Action> OnSelectedLog { get; set; }
    public Callbacker<Action> OnSelectedUser { get; set; }
    public ModerationService(BackendHttpClient http)
    {
        UsersListView = new ListView(http, typeof(TableContent))
        {
            OnTotal = RequestUsersTotal,
            OnGet = RequestUsers,
            GetHeader = () => "Пользователи",
            Selectable = true,
        };
        UsersListView.DataResolver.Props.AddRange(
            ["id", "avatar", "name", "middlename", "surname", "score", "webrole_name"]);
        
        LogsListView = new ListView(http, typeof(TableContent))
        {
            OnTotal = RequestLogsTotal,
            OnGet = RequestLogs,
            GetHeader = () => "Логи",
            Selectable = true,
        };
        LogsListView.DataResolver.Props.AddRange(
            ["id", "name", "middlename", "surname", "action", "datetime", "webrole_name"]);
        
        OnSelectedLog = new Callbacker<Action>(false);
        LogsListView.OnRowCallback.Subscribe("moderation", (index, element) =>
        {
            SelectedLog = element;
            OnSelectedLog.InvokeAll();
        });
        
        OnSelectedUser = new Callbacker<Action>(false);
        UsersListView.OnRowCallback.Subscribe("moderation", (index, element) =>
        {
            SelectedUser = element;
            OnSelectedUser.InvokeAll();
        });
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