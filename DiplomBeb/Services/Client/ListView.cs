using DiplomBeb.Utils;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client;

public class ListView<TObject>
{
    public ulong Page { get; private set; }
    public ulong MaxPages { get; private set; }
    public ulong ElementsOnPage { get; private set; }
    public ulong Total { get; private set; }
    public Callbacker OnListUpdated { get; private set; }
    public TObject[] Data { get; private set; }
    
    public Callbacker OnPageSwitch { get; private set; }
    
    public Func<BackendHttpClient, ulong, ulong,  HttpResponseState> OnGet { get; set; }
    public Func<BackendHttpClient, HttpResponseState> OnTotal { get; set; }
    
    public bool CanNext => Page + 1 <= MaxPages;
    public bool CanPrevious => Page - 1 >= 1;
    
    public bool PageSwitchMode { get; set; }

    private BackendHttpClient _http { get; set; }
    public ListView(BackendHttpClient http, ulong page = 1, bool pageSwitchMode = false)
    {
        _http = http;
        Page = page;
        PageSwitchMode = pageSwitchMode;
        ElementsOnPage = 30;
        OnListUpdated = new Callbacker();
        OnPageSwitch = new Callbacker(false);
        Data = [];
    }
    
    public void NextPage()
    {
        if (!CanNext) return; 
        Page++;

        _pageSwitch();
    }

    public void PreviousPage()
    {
        if (!CanPrevious) return;
        Page--;

        _pageSwitch();
    }

    private bool SetPage(ulong page)
    {
        if (page == 0 || page > MaxPages) return false;
        Page = page;
        return true;
    }

    public void SpecificPage(ulong page)
    {
        if (!SetPage(page)) return;

        _pageSwitch();
    }

    private void _pageSwitch()
    {
        if (PageSwitchMode)
        {
            OnPageSwitch.InvokeAll();
        }
        else
        {
            LoadData();
            OnListUpdated.InvokeAll();
        }
    }

    public bool LoadTotal()
    {
        var res = OnTotal?.Invoke(_http);
        if (!res.IsOk || !res.ResultAsULong(out var total))
            return false;
        
        Total = total;
        MaxPages = (ulong)Math.Round((double)Total/ (double)ElementsOnPage, MidpointRounding.ToPositiveInfinity);
        
        return true;
    }

    public bool First(ulong page)
    {
        Page = page;
        return LoadTotal() & LoadData();
    }

    public bool Update()
    {
        return LoadTotal() & LoadData();
    }
    
    public void UpdateThis()
    {
        Update();
    }

    public bool LoadData()
    {
        var skip = ElementsOnPage * (Page - 1);
        var res = OnGet?.Invoke(_http, skip, ElementsOnPage);
        if (!res.IsOk || !res.ResultAsJson<TObject[]>(out var data))
            return false;
        
        Data = data;
        
        OnListUpdated?.InvokeAll();

        return true;
    }
}