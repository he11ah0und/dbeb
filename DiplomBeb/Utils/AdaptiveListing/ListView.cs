using DiplomBeb.Components.Views.Samples;
using DiplomBeb.Utils;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Utils.AdaptiveListing;

public class ListView
{
    // private
    public bool CanNext => Page + 1 <= MaxPages;
    public bool CanPrevious => Page - 1 >= 1;
    public ulong Page { get; protected set; }
    public ulong MaxPages { get; private set; }
    public ulong Total { get; private set; }
    public Callbacker<Action> OnListUpdated { get; private set; }
    public Callbacker<Action> OnPageSwitch { get; private set; }
    public Callbacker<CallbackPropInElement> OnElementCallback { get; private set; }
    public DataResolver DataResolver { get; private set; }
    public ListElement[] Data { get; private set; }
    private BackendHttpClient _http { get; set; }
    
    // custom
    public Func<BackendHttpClient, ulong, ulong, Task<HttpResponseState>>? OnGet { get; set; }
    public Func<BackendHttpClient, Task<HttpResponseState>>? OnTotal { get; set; }
    public Func<string>? GetHeader { get; set; }
    public bool PageSwitchMode { get; set; }
    public ulong ElementsOnPage { get; set; }
    public bool HideHeading { get; set; }
    public bool HideTableHeader { get; set; }
    public bool HideFooter { get; set; }
    public bool HideRightFooterButtons { get; set; }
    public bool HideLeftFooterButtons { get; set; }
    public bool FitContent { get; set; }
    public bool SlimTableData { get; set; }
    
    public ListView(BackendHttpClient http, ulong page = 1, bool pageSwitchMode = false)
    {
        _http = http;
        Page = page;
        PageSwitchMode = pageSwitchMode;
        ElementsOnPage = 30;
        OnListUpdated = new Callbacker<Action>(false);
        OnPageSwitch = new Callbacker<Action>(false);
        OnElementCallback = new Callbacker<CallbackPropInElement>(false);
        Data = [];
        DataResolver = new DataResolver();
        
        DataResolver.ListProps.Add("name", new ListProp(
            (p) => p.Contains("surname") && p.Contains("middlename") ? "ФИО" : "Имя",
            typeof(NameSample)));
        DataResolver.ListProps.Add("avatar", new ListProp(
            (p) => "Аватар",
            typeof(AvatarSample)));
        DataResolver.ListProps.Add("id", new ListProp(
            (p) => "№",
            typeof(SimpleSample)));
        DataResolver.ListProps.Add("action", new ListProp(
            (p) => "Действие",
            typeof(SimpleSample)));
        DataResolver.ListProps.Add("time", new ListProp(
            (p) => "Время",
            typeof(SimpleSample)));
        DataResolver.ListProps.Add("score", new ListProp(
            (p) => "Звёзды",
            typeof(ScoreSample)));
        DataResolver.ListProps.Add("check", new ListProp(
            (p) => "_",
            typeof(CheckboxSample)));
        DataResolver.ListProps.Add("datetime", new ListProp(
            (p) => "Дата",
            typeof(DateTimeSample)));
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
        if (OnTotal is null) return true;
        var res = await OnTotal?.Invoke(_http);
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