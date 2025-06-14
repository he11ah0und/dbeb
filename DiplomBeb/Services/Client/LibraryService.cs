using DiplomBeb.Components.Views;
using DiplomBeb.Database.Model;
using DiplomBeb.Utils;
using DiplomBeb.Utils.AdaptiveListing;
using DiplomBeb.Utils.Networking;

namespace DiplomBeb.Services.Client;

public class LibraryService
{
    public ListView LibraryListView { get; private set; }
    public ListView FiltersListView { get; private set; }
    public List<string> Filters { get; private set; }
    public LibraryService(BackendHttpClient http)
    {
        Filters = [];
        LibraryListView = new ListView(http, typeof(TableContent))
        {
            OnGet = RequestLibs,
            OnTotal = RequestLibsTotal,
            GetHeader = () => "Библиотека полезных ссылок",
            PageSwitchMode = false,
        };
        LibraryListView.DataResolver.Props.AddRange(
            ["id", "name", "link"]);
        

        FiltersListView = new ListView(http, typeof(TableContent))
        {
            OnGet = RequestFilters,
            OnTotal = RequestFiltersTotal,
            HideFooter = true,
            HideHeading = true,
            HideTableHeader = true,
            SlimTableData = true,
        };
        FiltersListView.DataResolver.Props.AddRange(
            ["check"]);
        FiltersListView.OnElementCallback.Subscribe("libraryService", OnFiltersCallback);
    }
    
    private void OnFiltersCallback(string propName,
        ulong index,
        ListElement listElement,
        ObjectContainer args)
    {
        var state = (bool)args.Get("value");
        if (state)
            SetFilter(listElement.Get("name"));
        else
            DelFilter(listElement.Get("name"));

        LibraryListView.Update();
    }

    public void SetFilter(string filter)
    {
        if (Filters.Contains(filter)) return;
        Filters.Add(filter);
    }
    public void DelFilter(string filter) 
        => Filters.Remove(filter);
    public void ClearFilters()
        => Filters.Clear();
    
    private Task<HttpResponseState> RequestLibs(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.NativeRequest($"Library/{skip}/{take}", JsonContent.Create(Filters.ToArray()), HttpMethod.Post);
    }

    private Task<HttpResponseState> RequestLibsTotal(BackendHttpClient http)
    {
        return http.NativeRequest("Library/total", JsonContent.Create(Filters.ToArray()), HttpMethod.Post);
    }
    
    private Task<HttpResponseState> RequestFilters(BackendHttpClient http, ulong skip, ulong take)
    {
        return http.NativeRequest($"Library/filters/{skip}/{take}");
    }
    
    private Task<HttpResponseState> RequestFiltersTotal(BackendHttpClient http)
    {
        return http.NativeRequest("Library/total_filters");
    }
}