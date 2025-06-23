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
    public Callbacker<Action> OnSelectedLog { get; private set; }
    public Callbacker<Action> OnSelectedUser { get; private set; }
    
    public byte[]? DocumentImage { get; private set; }
    public ListElement? ProcessDocument { get; private set; }
    public Callbacker<Func<Task>> OnRequestDocument { get; private set; }
    
    public ListElement[] DocumentTypes { get; private set; }
    public ListElement[] Places { get; private set; }
    
    private BackendHttpClient _http;
    public ModerationService(BackendHttpClient http)
    {
        _http = http;
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

        OnRequestDocument = new Callbacker<Func<Task>>(false);
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

    public async Task<bool> RequestDocumentTypes()
    {
        var res = await _http.Request("Documents/total_types");
        if (!res.IsOk
            || !res.ResultAsULong(out ulong total_types))
            return false;
        
        res = await _http.Request($"Documents/types/0/{total_types}");
        if (!res.IsOk
            || !res.ResultAsJson<ListElement[]>(out var types))
            return false;
        DocumentTypes = types;
        return true;
    }
    public async Task<bool> RequestPlaces()
    {
        var res = await _http.Request("Place/total");
        if (!res.IsOk
            || !res.ResultAsULong(out ulong total))
            return false;
        
        res = await _http.Request($"Place/get/0/{total}");
        if (!res.IsOk
            || !res.ResultAsJson<ListElement[]>(out var places))
            return false;
        Places = places;
        return true;
    }

    public async Task<bool> RequestDocumentForModeration()
    {
        var res = await _http.Request("Moder/document");
        if (!res.IsOk 
            || !res.ResultAsJson<ListElement>(out var result))
            return false;
        
        if (bool.Parse(result.Get("hasWork", "false")) && !await DownloadImage(result))
            return false;
        
        ProcessDocument = result;
        
        await OnRequestDocument.InvokeAllAsync();
        return true;
    }

    private async Task<bool> DownloadImage(ListElement element)
    {
        var url = $"Documents/down/{element.Get("documentId")}";
        var res = await _http.Request(url);
        if (!res.IsOk || !res.ReadAsBytes(out var data)) 
            return false;
        DocumentImage = data;
        return true;
    }
    
    public async Task<bool> AcceptDocumentFromModeration(ulong documentId, bool accepted)
    {
        var req = new ListElement();
        req.Set("state", accepted);
        req.Set("documentTypeId", documentId);
        var res = await _http
            .Request("Moder/document/accept",
                JsonContent.Create(req),
                HttpMethod.Post);
        if (!res.IsOk 
            || !res.ResultAsBool(out var result)
            || !result)
            return false;

        ProcessDocument = null;
        DocumentImage = null;
        
        await OnRequestDocument.InvokeAllAsync();
        return true;
    }
}