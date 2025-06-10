using DiplomBeb.Utils;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Utils.AdaptiveListing;

public class ListView
{
    public ulong Page { get; protected set; }
    public ulong MaxPages { get; private set; }
    public ulong ElementsOnPage { get; set; }
    public ulong Total { get; private set; }
    
    public bool CanNext => Page + 1 <= MaxPages;
    public bool CanPrevious => Page - 1 >= 1;
    
    public bool PageSwitchMode { get; set; }
    
    public Callbacker OnListUpdated { get; private set; }
    public Callbacker OnPageSwitch { get; private set; }
    
    public Func<BackendHttpClient, ulong, ulong, Task<HttpResponseState>> OnGet { get; set; }
    public Func<BackendHttpClient, Task<HttpResponseState>> OnTotal { get; set; }
    
    public DataResolver DataResolver { get; private set; }
    
    private BackendHttpClient _http { get; set; }
    
    public ListElement[] Data { get; private set; }
    public Func<string>? GetHeader { get; set; }
    
    public ListView(BackendHttpClient http, ulong page = 1, bool pageSwitchMode = false)
    {
        _http = http;
        Page = page;
        PageSwitchMode = pageSwitchMode;
        ElementsOnPage = 30;
        OnListUpdated = new Callbacker(false);
        OnPageSwitch = new Callbacker(false);
        Data = [];
        DataResolver = new DataResolver();
        
        DataResolver.ListProps.Add("id", new ListProp(
            (p) => "№",
            (d, li, id, i) => $"<h4>{i}</h4>"));
        DataResolver.ListProps.Add("avatar", new ListProp(
            (p) => "Аватар",
            (d, li, id, i) => $"<div class=\"image\"></div>"));
        DataResolver.ListProps.Add("name", new ListProp(
            (p) => p.Contains("surname") && p.Contains("middlename") ? "ФИО" : "Имя",
            (d, li, id, i) =>
            {
                var hasLink = d.IsHave("link");
                var textTag = hasLink ? "a" : "h4";
                var str = 
                        $"<div class=\"name\">\n" +
                        $"   <{textTag} {(hasLink ? $"target=\"_blank\" href=\"{li.Get("link")}\"" : "")}>{(d.IsHave("surname", "middlename") ? $"{li.Get("surname")} {li.Get(id)} {li.Get("middlename")}" : li.Get(id))}</{textTag}>\n" +
                        $"   {(d.IsHave("webrole_name") ? $"<p>{li.Get("webrole_name")}</p>" : "")}\n" +
                        $"</div>";

                return str;
            }));
        DataResolver.ListProps.Add("action", new ListProp(
            (p) => "Действие",
            (d, li, id, i) => $"<h4>{li.Get(id)}</h4>"));
        DataResolver.ListProps.Add("time", new ListProp(
            (p) => "Время",
            (d, li, id, i) => $"<h4>{li.Get(id)}</h4>"));
        DataResolver.ListProps.Add("score", new ListProp(
            (p) => "Звёзды",
            (d, li, id, i) => 
                                $"<div class=\"stars-container star\">\n" +
                                $"  <h4>{li.Get(id)}</h4>\n" +
                                $"  <img src=\"/icons/star1-ico.svg\" alt=\"\">\n" +
                                $"</div>"));
    }
    
    public string Header => GetHeader is null ? "<HEADER>" : GetHeader();
    
    public async Task NextPage()
    {
        if (!CanNext) return; 
        Page++;

        await _pageSwitch();
    }

    public async Task PreviousPage()
    {
        if (!CanPrevious) return;
        Page--;

        await _pageSwitch();
    }

    private bool SetPage(ulong page)
    {
        if (page == 0 || page > MaxPages) return false;
        Page = page;
        return true;
    }

    public async Task SpecificPage(ulong page)
    {
        if (!SetPage(page)) return;

        await _pageSwitch();
    }

    private async Task _pageSwitch()
    {
        if (PageSwitchMode)
        {
            OnPageSwitch.InvokeAll();
        }
        else
        {
            await LoadData();
            OnListUpdated.InvokeAll();
        }
    }
    
    public async Task<bool> LoadTotal()
    {
        var res = await OnTotal?.Invoke(_http)!;
        if (!res.IsOk || !res.ResultAsULong(out var total))
            return false;
        
        Total = total;
        MaxPages = (ulong)Math.Round((double)Total/ (double)ElementsOnPage, MidpointRounding.ToPositiveInfinity);
        
        return true;
    }
    
    public Task<bool> First(ulong page)
    {
        Page = page;
        return Update();
    }

    public async Task<bool> Update()
    {
        return await LoadTotal() & await LoadData();
    }

    public async Task<bool> LoadData()
    {
        var skip = ElementsOnPage * (Page - 1);
        var res = await OnGet?.Invoke(_http, skip, ElementsOnPage)!;
        if (!res.IsOk || !res.ResultAsJson<ListElement[]>(out var data))
            return false;

        Data = data;
        OnListUpdated?.InvokeAll();

        return true;
    }
}